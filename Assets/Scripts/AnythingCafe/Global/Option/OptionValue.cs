public abstract class OptionValue
{
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract object GetDefaultValue();
}

public class OptionValue<T> : OptionValue
{
    private T _value;
    private readonly T _defaultValue;

    public OptionValue(T defaultValue)
    {
        _defaultValue = defaultValue;
        _value = defaultValue;
    }

    public override object GetValue() => _value;
    public override void SetValue(object value) => _value = (T)value;
    public override object GetDefaultValue() => _defaultValue;

    public T Value
    {
        get => _value;
        set => _value = value;
    }
}