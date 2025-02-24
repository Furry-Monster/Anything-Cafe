using System;
using System.Reflection;

public struct MSReflectedMember
{
    private readonly FieldInfo _fieldInfo;
    private readonly PropertyInfo _propertyInfo;

    public bool IsProperty;

    public bool IsNull => _fieldInfo == null && _propertyInfo == null;

    public string Name => !IsProperty
        ? _fieldInfo.Name
        : _propertyInfo.Name;

    public Type MemberType => !IsProperty
        ? _fieldInfo.FieldType
        : _propertyInfo.PropertyType;

    public bool IsPublic
    {
        get
        {
            if (!IsProperty)
                return _fieldInfo.IsPublic;
            return _propertyInfo.GetGetMethod(true).IsPublic && _propertyInfo.GetSetMethod(true).IsPublic;
        }
    }

    public bool IsProtected => !IsProperty
        ? _fieldInfo.IsFamily
        : _propertyInfo.GetGetMethod(true).IsFamily;

    public bool IsStatic => !IsProperty
        ? _fieldInfo.IsStatic
        : _propertyInfo.GetGetMethod(true).IsStatic;

    public MSReflectedMember(object fieldPropertyInfo)
    {
        // Check if the object is a FieldInfo or a PropertyInfo 
        if (fieldPropertyInfo == null)
        {
            _propertyInfo = null;
            _fieldInfo = null;
            IsProperty = false;
        }
        else
        {
            IsProperty = MSReflection.IsAssignableFrom(typeof(PropertyInfo), fieldPropertyInfo.GetType());

            if (IsProperty)
            {
                _propertyInfo = (PropertyInfo)fieldPropertyInfo;
                _fieldInfo = null;
            }
            else
            {
                _fieldInfo = (FieldInfo)fieldPropertyInfo;
                _propertyInfo = null;
            }
        }
    }

    public void SetValue(object obj, object value)
    {
        if (IsProperty)
            _propertyInfo.SetValue(obj, value, null);
        else
            _fieldInfo.SetValue(obj, value);
    }

    public object GetValue(object obj)
    {
        return IsProperty
            ? _propertyInfo.GetValue(obj, null)
            : _fieldInfo.GetValue(obj);
    }
}
