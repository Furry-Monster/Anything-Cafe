using System;
using System.Reflection;

/// <summary>
/// ʹ�÷�������ֶλ����Եİ�װ�ṹ�塣
/// </summary>
public struct MSReflectedMember
{
    private readonly FieldInfo _fieldInfo;
    private readonly PropertyInfo _propertyInfo;

    /// <summary>
    /// ָʾ��ǰ��Ա�Ƿ�Ϊ���ԣ�����Ϊ�ֶΣ���
    /// </summary>
    public bool IsProperty;

    /// <summary>
    /// �жϱ���װ�ĳ�Ա�Ƿ�Ϊ�ա�
    /// </summary>
    public bool IsNull => _fieldInfo == null && _propertyInfo == null;

    /// <summary>
    /// ��ȡ�ֶλ����Ե����ơ�
    /// </summary>
    public string Name => !IsProperty ? _fieldInfo.Name : _propertyInfo.Name;

    /// <summary>
    /// ��ȡ�ֶλ����Ե��������͡�
    /// </summary>
    public Type MemberType => !IsProperty ? _fieldInfo.FieldType : _propertyInfo.PropertyType;

    /// <summary>
    /// �ж��ֶλ������Ƿ�Ϊ�����ġ�
    /// </summary>
    public bool IsPublic
    {
        get
        {
            if (!IsProperty)
                return _fieldInfo.IsPublic;
            // �������ԣ���Ҫ���Getter��Setter�Ƿ��Ϊ��������
            var getter = _propertyInfo.GetGetMethod(true);
            var setter = _propertyInfo.GetSetMethod(true);
            return (getter != null && getter.IsPublic) && (setter != null && setter.IsPublic);
        }
    }

    /// <summary>
    /// �ж��ֶλ������Ƿ���б�������
    /// </summary>
    public bool IsProtected => !IsProperty
        ? _fieldInfo.IsFamily
        : _propertyInfo.GetGetMethod(true)?.IsFamily ?? false;

    /// <summary>
    /// �ж��ֶλ������Ƿ�Ϊ��̬�ġ�
    /// </summary>
    public bool IsStatic => !IsProperty
        ? _fieldInfo.IsStatic
        : _propertyInfo.GetGetMethod(true)?.IsStatic ?? false;

    /// <summary>
    /// ʹ�ø������ֶλ�������Ϣʵ����һ��MSReflectedMember����
    /// </summary>
    /// <param name="fieldPropertyInfo">�ֶλ����Եķ�����Ϣ����</param>
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
    /// Ϊָ���������ø��ֶλ����Ե�ֵ��
    /// </summary>
    /// <param name="obj">�����ó�Ա�Ķ���</param>
    /// <param name="value">Ҫ���õ�ֵ��</param>
    public void SetValue(object obj, object value)
    {
        if (IsProperty)
            _propertyInfo.SetValue(obj, value, null);
        else
            _fieldInfo.SetValue(obj, value);
    }

    /// <summary>
    /// ��ȡָ�������и��ֶλ����Ե�ֵ��
    /// </summary>
    /// <param name="obj">�����ó�Ա�Ķ���</param>
    /// <returns>���ض�ȡ����ֵ��</returns>
    public object GetValue(object obj)
    {
        return IsProperty
            ? _propertyInfo.GetValue(obj, null)
            : _fieldInfo.GetValue(obj);
    }
}
