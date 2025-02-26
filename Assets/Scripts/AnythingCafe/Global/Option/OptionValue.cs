public abstract class OptionValue
{
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract object GetDefaultValue();
    public abstract bool Validate(object value);
    public abstract OptionGroup Group { get; }
}

public class OptionValue<T> : OptionValue
{
    private T _value;
    private readonly T _defaultValue;
    private readonly System.Func<T, bool> _validator;
    private readonly OptionGroup _group;

    public OptionValue(T defaultValue, System.Func<T, bool> validator = null, OptionGroup group = OptionGroup.Other)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
        _validator = validator;
        _group = group;
    }

    public override object GetValue() => _value;

    public override void SetValue(object value)
    {
        var typedValue = (T)value;
        if (_validator != null && !_validator(typedValue))
            throw new System.ArgumentException($"Invalid value: {value}");
        _value = typedValue;
    }

    public override object GetDefaultValue() => _defaultValue;

    public override bool Validate(object value)
    {
        if (value is not T typedValue) return false;
        return _validator?.Invoke(typedValue) ?? true;
    }

    public override OptionGroup Group => _group;

    public T Value
    {
        get => _value;
        set
        {
            if (_validator != null && !_validator(value))
                throw new System.ArgumentException($"Invalid value: {value}");
            _value = value;
        }
    }
}