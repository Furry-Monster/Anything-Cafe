using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class MSSettings : ICloneable
{
    private static MSSettings _default;
    private static MSDefaultSettings _defaultSettings;
    private const string DefaultSettingsPath = "MSDefaultSettings.asset";
    private static readonly string[] SupportedExtensions = {
        ".txt",
        ".htm",
        ".html",
        ".xml",
        ".bytes",
        ".json",
        ".csv",
        ".yaml",
        ".fnt"
    };
    [SerializeField] private DirectoryStrategy _directoryStrategy;
    public string SavePath = "SaveFile.ms";
    public EncryptionMode Encryption;
    public string EncryptionKey = "passw0rd";
    public CompressionMode Compression;
    public FormatMode Format;
    public Encoding Encoding = Encoding.UTF8;

    public static MSDefaultSettings DefaultSettings
    {
        get
        {
            if (_defaultSettings == null)
                _defaultSettings = Resources.Load<MSDefaultSettings>(DefaultSettingsPath);
            return _defaultSettings;
        }
    }

    public static MSSettings Default
    {
        get
        {
            if (_default == null && _defaultSettings != null)
                _default = _defaultSettings.SerializableSettings;
            return _default;
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

            switch (DirectoryStrategy)
            {
                case DirectoryStrategy.UserDir:
                    return MSIO.PersistentDataPath + "/" + SavePath;
                case DirectoryStrategy.GameDir:
                    return MSIO.DataPath + "/" + SavePath;
                case DirectoryStrategy.PlayerPrefs:
                    {
                        var extension = MSIO.GetExtension(SavePath);

                        var flag = SupportedExtensions
                            .Any(supportedExtension => extension == supportedExtension);

                        if (!flag)
                            throw new ArgumentException(
                                "Extension of file in Resources must be .json, .bytes, .txt, .csv, .htm, .html, .xml, .yaml or .fnt, but path given was \"" +
                                SavePath + "\"");

                        return SavePath.Replace(extension, ""); // 去掉扩展名
                    }
                default:
                    throw new NotImplementedException($"DirectoryStrategy {DirectoryStrategy} has not been implemented yet.");
            }
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
        if (!shouldApplyDefaults || DefaultSettings == null)
            return;
        Default.CopyInto(this);
    }
    #endregion

    #region Prototype Methods
    public object Clone()
    {
    }

    public void CopyInto(MSSettings newSettings)
    {
        newSettings._directoryStrategy = _directoryStrategy;
        newSettings.SavePath = SavePath;
        newSettings.Encryption = Encryption;
        newSettings.EncryptionKey = EncryptionKey;
        newSettings.Compression = Compression;
        newSettings.Format = Format;
        newSettings.Encoding = Encoding;
    }
    #endregion

    /// <summary>
    /// 检查路径是否为绝对路径
    /// </summary>
    private static bool IsAbsolute(string path)
    {
        return path.Length > 0 && (path[0] == '/' || path[0] == '\\') || path.Length > 1 && path[1] == ':';
    }
}

public enum DirectoryStrategy
{
    UserDir,
    GameDir,
    PlayerPrefs
}

public enum EncryptionMode
{
    None,
    AES,
    Rinjdael
}

public enum CompressionMode
{
    None,
    Gzip
}

public enum FormatMode
{
    JSON,
    Binary,
    XML
}