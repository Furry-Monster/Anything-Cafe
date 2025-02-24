using System;
using System.ComponentModel;
using System.Text;
using UnityEngine;

[Serializable]
public class MSSettings : ICloneable
{
    // 单例与原型
    private static MSDefaultSettings _defaultSettingsSO = null;
    private static MSSettings _defaultSettings;
    private const string DefaultSettingsPath = "MSDefaultSettings.asset";

    // 保存路径相关
    [Header("General")]
    [SerializeField]
    [Tooltip("Directory strategy for saving files")]
    private DirectoryStrategy _directoryStrategy;
    [Tooltip("File name for saving data")]
    public string SavePath = "SaveFile.ms";

    // 加密相关
    [Header("Encryption")]
    public EncryptionMode Encryption;
    public string EncryptionKey = "passw0rd";
    [Tooltip("Buffer size for encryption and decryption")]
    public int BufferSize = 2048;

    // 压缩相关
    [Header("Compression")]
    public CompressionMode Compression;

    // 格式相关
    [Header("Format")]
    public FormatMode Format;
    public Encoding Encoding = Encoding.UTF8;

    public static MSDefaultSettings DefaultSettingsSO
    {
        get
        {
            if (_defaultSettingsSO == null)
                Resources.Load<MSDefaultSettings>(DefaultSettingsPath);
            return _defaultSettingsSO;
        }
    }

    public static MSSettings DefaultSettings
    {
        get
        {
            if (_defaultSettings == null && DefaultSettingsSO != null)
                _defaultSettings = _defaultSettingsSO.DefaultSettings;
            return _defaultSettings;
        }
    }

    public DirectoryStrategy DirectoryStrategy
    {
        get =>
            _directoryStrategy == DirectoryStrategy.UserDir && (Application.platform == RuntimePlatform.WebGLPlayer ||
                                               Application.platform == RuntimePlatform.tvOS)
                ? DirectoryStrategy.PlayerPrefs
                : _directoryStrategy;

        set => _directoryStrategy = value;
    }

    public string FullPath
    {
        get
        {
            if (SavePath == null)
                throw new NullReferenceException(
                    "SavePath is null,which means there might be some problem with the loading of the default setting, please check the default setting file.");

            if (IsAbsolute(SavePath)) return SavePath;

            return DirectoryStrategy switch
            {
                DirectoryStrategy.UserDir => IOHelper.PersistentDataPath + "/" + SavePath,
                DirectoryStrategy.GameDir => IOHelper.DataPath + "/" + SavePath,
                DirectoryStrategy.PlayerPrefs => SavePath,
                _ => throw new Exception($"DirectoryStrategy {DirectoryStrategy} has not been implemented yet.")
            };
        }
    }

    #region Constructor

    #region 值构造
    public MSSettings(string path, MSSettings settings = null)
        : this(true)
    {
        settings?.CopyInto(this);
        if (path == null)
            return;
        SavePath = path;
    }

    public MSSettings(string path, EncryptionMode encryption, string encryptionKey, MSSettings settings)
        : this(path, settings)
    {
        Encryption = encryption;
        EncryptionKey = encryptionKey;
    }
    #endregion

    #region Enum构造

    public MSSettings(params Enum[] enums)
        : this(true)
    {
        foreach (var @enum in enums)
        {
            switch (@enum)
            {
                case DirectoryStrategy directoryStrategy:
                    DirectoryStrategy = directoryStrategy;
                    break;
                case EncryptionMode encryption:
                    Encryption = encryption;
                    break;
                case CompressionMode compression:
                    Compression = compression;
                    break;
                case FormatMode format:
                    Format = format;
                    break;
                default:
                    throw new ArgumentException($"Enum {@enum.GetType().Name} is not supported.");
            }
        }
    }

    public MSSettings(string path, params Enum[] enums)
        : this(enums)
    {
        if (path == null)
            return;
        SavePath = path;
    }

    #endregion

    // base constructor
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MSSettings(bool shouldApplyDefaults)
    {
        if (!shouldApplyDefaults || DefaultSettingsSO == null) return;
        DefaultSettingsSO.DefaultSettings.CopyInto(this);
    }
    #endregion

    #region Prototype Methods
    public object Clone()
    {
        var newSettings = new MSSettings((string)null);
        CopyInto(newSettings);
        return newSettings;
    }

    public void CopyInto(MSSettings newSettings)
    {
        newSettings._directoryStrategy = _directoryStrategy;
        newSettings.SavePath = SavePath;
        newSettings.Encryption = Encryption;
        newSettings.EncryptionKey = EncryptionKey;
        newSettings.Compression = Compression;
        newSettings.Format = Format;
    }
    #endregion

    private static bool IsAbsolute(string path)
    {
        // 检查路径是否为绝对路径
        return path.Length > 0 && (path[0] == '/' || path[0] == '\\') || path.Length > 1 && path[1] == ':';
    }
}

#region Enum
/// <summary>
/// 存储目录策略，决定了保存文件的位置
/// </summary>
public enum DirectoryStrategy
{
    UserDir,
    GameDir,
    PlayerPrefs
}

/// <summary>
/// 加密模式，决定了加密算法
/// </summary>
public enum EncryptionMode
{
    None,
    AES,
    Rinjdael
}

/// <summary>
/// 压缩模式，决定了压缩算法
/// </summary>
public enum CompressionMode
{
    None,
    Gzip
}

/// <summary>
/// 格式模式，决定了保存文件的格式
/// </summary>
public enum FormatMode
{
    Binary,
    JSON,
    XML
}
#endregion