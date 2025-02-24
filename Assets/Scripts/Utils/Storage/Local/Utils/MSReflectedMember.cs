using System;
using System.Reflection;

/// <summary>
/// 使用反射操作字段或属性的包装结构体。
/// </summary>
public struct MSReflectedMember
{
    private readonly FieldInfo _fieldInfo;
    private readonly PropertyInfo _propertyInfo;

    /// <summary>
    /// 指示当前成员是否为属性（否则为字段）。
    /// </summary>
    public bool IsProperty;

    /// <summary>
    /// 判断被封装的成员是否为空。
    /// </summary>
    public bool IsNull => _fieldInfo == null && _propertyInfo == null;

    /// <summary>
    /// 获取字段或属性的名称。
    /// </summary>
    public string Name => !IsProperty ? _fieldInfo.Name : _propertyInfo.Name;

    /// <summary>
    /// 获取字段或属性的数据类型。
    /// </summary>
    public Type MemberType => !IsProperty ? _fieldInfo.FieldType : _propertyInfo.PropertyType;

    /// <summary>
    /// 判断字段或属性是否为公共的。
    /// </summary>
    public bool IsPublic
    {
        get
        {
            if (!IsProperty)
                return _fieldInfo.IsPublic;
            // 对于属性，需要检查Getter和Setter是否均为公共方法
            var getter = _propertyInfo.GetGetMethod(true);
            var setter = _propertyInfo.GetSetMethod(true);
            return (getter != null && getter.IsPublic) && (setter != null && setter.IsPublic);
        }
    }

    /// <summary>
    /// 判断字段或属性是否具有保护级别。
    /// </summary>
    public bool IsProtected => !IsProperty
        ? _fieldInfo.IsFamily
        : _propertyInfo.GetGetMethod(true)?.IsFamily ?? false;

    /// <summary>
    /// 判断字段或属性是否为静态的。
    /// </summary>
    public bool IsStatic => !IsProperty
        ? _fieldInfo.IsStatic
        : _propertyInfo.GetGetMethod(true)?.IsStatic ?? false;

    /// <summary>
    /// 使用给定的字段或属性信息实例化一个MSReflectedMember对象。
    /// </summary>
    /// <param name="fieldPropertyInfo">字段或属性的反射信息对象。</param>
    public MSReflectedMember(object fieldPropertyInfo)
    {
        switch (fieldPropertyInfo)
        {
            case PropertyInfo propertyInfo:
                IsProperty = true;
                _propertyInfo = propertyInfo;
                _fieldInfo = null;
                break;
            case FieldInfo fieldInfo:
                IsProperty = false;
                _fieldInfo = fieldInfo;
                _propertyInfo = null;
                break;
            default:
                _fieldInfo = null;
                _propertyInfo = null;
                IsProperty = false;
                break;
        }
    }

    /// <summary>
    /// 为指定对象设置该字段或属性的值。
    /// </summary>
    /// <param name="obj">包含该成员的对象。</param>
    /// <param name="value">要设置的值。</param>
    public void SetValue(object obj, object value)
    {
        if (IsProperty)
            _propertyInfo.SetValue(obj, value, null);
        else
            _fieldInfo.SetValue(obj, value);
    }

    /// <summary>
    /// 获取指定对象中该字段或属性的值。
    /// </summary>
    /// <param name="obj">包含该成员的对象。</param>
    /// <returns>返回读取到的值。</returns>
    public object GetValue(object obj)
    {
        return IsProperty
            ? _propertyInfo.GetValue(obj, null)
            : _fieldInfo.GetValue(obj);
    }
}
