using System;
using UnityEngine;

public class MonStore
{
    #region SaveAPI 保存

    #region 无类型参数
    public static void Save(string key, object value) => Save(key, value, new MSSettings((string)null));

    public static void Save(string key, object value, string filePath) => Save(key, value, new MSSettings(filePath));

    public static void Save(string key, object value, string filePath, MSSettings settings) => Save(key, value, new MSSettings(filePath, settings));

    public static void Save(string key, object value, MSSettings settings) => Save<object>(key, value, settings);
    #endregion

    #region 有类型参数
    public static void Save<T>(string key, T value) => Save(key, value, new MSSettings((string)null));

    public static void Save<T>(string key, T value, string filePath) => Save(key, value, new MSSettings(filePath));

    public static void Save<T>(string key, T value, string filePath, MSSettings settings) => Save(key, value, new MSSettings(filePath, settings));

    public static void Save<T>(string key, T value, MSSettings settings)
    {
        // TODO: Implement saving logic here
    }
    #endregion

    #endregion


    #region LoadAPI 加载

    #region 无类型参数
    public static object Load(string key) => Load<object>(key, new MSSettings((string)null));

    public static object Load(string key, string filePath) => Load<object>(key, new MSSettings(filePath));

    public static object Load(string key, string filePath, MSSettings settings) => Load<object>(key, new MSSettings(filePath, settings));

    public static object Load(string key, MSSettings settings) => Load<object>(key, settings);

    public static object Load(string key, object defaultValue) => Load<object>(key, defaultValue, new MSSettings((string)null));

    public static object Load(string key, string filePath, object defaultValue) => Load<object>(key, defaultValue, new MSSettings(filePath));

    public static object Load(string key, string filePath, object defaultValue, MSSettings settings) => Load<object>(key, defaultValue, new MSSettings(filePath, settings));

    public static object Load(string key, object defaultValue, MSSettings settings) => Load<object>(key, defaultValue, settings);
    #endregion

    #region 有类型参数
    public static T Load<T>(string key) => Load<T>(key, new MSSettings((string)null));

    public static T Load<T>(string key, string filePath) => Load<T>(key, new MSSettings(filePath));

    public static T Load<T>(string key, string filePath, MSSettings settings) => Load<T>(key, new MSSettings(filePath, settings));

    public static T Load<T>(string key, MSSettings settings)
    {
        // TODO: Implement loading logic here
        throw new System.NotImplementedException();
    }

    public static T Load<T>(string key, T defaultValue) => Load(key, defaultValue, new MSSettings((string)null));

    public static T Load<T>(string key, string filePath, T defaultValue) => Load(key, defaultValue, new MSSettings(filePath));

    public static T Load<T>(string key, string filePath, T defaultValue, MSSettings settings) => Load(key, defaultValue, new MSSettings(filePath, settings));

    public static T Load<T>(string key, T defaultValue, MSSettings settings)
    {
        // TODO: Implement loading logic here
        throw new System.NotImplementedException();
    }
    #endregion

    #endregion


    #region ExistsAPI 是否存在

    #region 键值对
    public static bool KeyExists(string key) => KeyExists(key, new MSSettings((string)null));

    public static bool KeyExists(string key, string filePath) => KeyExists(key, new MSSettings(filePath));

    public static bool KeyExists(string key, string filePath, MSSettings settings) => KeyExists(key, new MSSettings(filePath, settings));

    public static bool KeyExists(string key, MSSettings settings)
    {
        // TODO: Implement checking logic here
        throw new NotImplementedException();
    }
    #endregion

    #region 文件
    public static bool FileExists() => FileExists(new MSSettings((string)null));

    public static bool FileExists(string filePath) => FileExists(new MSSettings(filePath));

    public static bool FileExists(string filePath, MSSettings settings) => FileExists(new MSSettings(filePath, settings));

    public static bool FileExists(MSSettings settings)
    {
        // TODO: Implement checking logic here
        throw new NotImplementedException();
    }
    #endregion

    #region 文件夹
    public static bool DirectoryExists(string folderPath) => DirectoryExists(new MSSettings(folderPath));

    public static bool DirectoryExists(string folderPath, MSSettings settings) => DirectoryExists(new MSSettings(folderPath, settings));

    public static bool DirectoryExists(MSSettings settings)
    {
        // TODO: Implement checking logic here
        throw new NotImplementedException();
    }
    #endregion

    #endregion


    #region DeleteAPI 删除

    #region 键值对
    public static void DeleteKey(string key) => DeleteKey(key, new MSSettings((string)null));

    public static void DeleteKey(string key, string filePath) => DeleteKey(key, new MSSettings(filePath));

    public static void DeleteKey(string key, string filePath, MSSettings settings) => DeleteKey(key, new MSSettings(filePath, settings));

    public static void DeleteKey(string key, MSSettings settings)
    {
        // TODO: Implement deleting logic here
    }
    #endregion

    #region 文件
    public static void DeleteFile() => DeleteFile(new MSSettings((string)null));

    public static void DeleteFile(string filePath) => DeleteFile(new MSSettings(filePath));

    public static void DeleteFile(string filePath, MSSettings settings) => DeleteFile(new MSSettings(filePath, settings));

    public static void DeleteFile(MSSettings settings)
    {
        // TODO: Implement deleting logic here
    }
    #endregion

    #region 文件夹
    public static void DeleteDirectory(string directoryPath) => DeleteDirectory(new MSSettings(directoryPath));

    public static void DeleteDirectory(string directoryPath, MSSettings settings) => DeleteDirectory(new MSSettings(directoryPath, settings));

    public static void DeleteDirectory(MSSettings settings)
    {
        // TODO: Implement deleting logic here
    }
    #endregion

    #endregion


    #region GetAPI 获取

    #region 键值对
    public static string[] GetKeys() => GetKeys(new MSSettings((string)null));

    public static string[] GetKeys(string filePath) => GetKeys(new MSSettings(filePath));

    public static string[] GetKeys(string filePath, MSSettings settings) => GetKeys(new MSSettings(filePath, settings));

    public static string[] GetKeys(MSSettings settings)
    {
        // TODO: Implement getting keys logic here
        throw new NotImplementedException();
    }
    #endregion

    #region 文件
    public static string[] GetFiles() => GetFiles(new MSSettings((string)null));

    public static string[] GetFiles(string directoryPath) => GetFiles(new MSSettings(directoryPath));

    public static string[] GetFiles(string directoryPath, MSSettings settings) => GetFiles(new MSSettings(directoryPath, settings));

    public static string[] GetFiles(MSSettings settings)
    {
        // TODO: Implement getting files logic here
        throw new NotImplementedException();
    }
    #endregion

    #region 文件夹
    public static string[] GetDirectories() => GetDirectories(new MSSettings((string)null));

    public static string[] GetDirectories(string directoryPath) => GetDirectories(new MSSettings(directoryPath));

    public static string[] GetDirectories(string directoryPath, MSSettings settings) => GetDirectories(new MSSettings(directoryPath, settings));

    public static string[] GetDirectories(MSSettings settings)
    {
        return settings.DirectoryStrategy is DirectoryStrategy.UserDir or DirectoryStrategy.GameDir
            ? IOHelper.GetDirectories(settings.FullPath,
                false)
            : throw new NotSupportedException("GetDirectories can only be used when the location is set to File.");
    }
    #endregion

    #endregion


    #region CopyAPI 复制

    #region 文件
    public static void CopyFile(string oldFilePath, string newFilePath) => CopyFile(new MSSettings(oldFilePath), new MSSettings(newFilePath));

    public static void CopyFile(string oldFilePath, string newFilePath, MSSettings oldSettings, MSSettings newSettings) => CopyFile(new MSSettings(oldFilePath, oldSettings), new MSSettings(newFilePath, newSettings));

    public static void CopyFile(MSSettings oldSettings, MSSettings newSettings)
    {
        // TODO: Implement copying logic here
    }
    #endregion

    #region 文件夹

    public static void CopyDirectory(string oldDirectoryPath, string newDirectoryPath) => CopyDirectory(new MSSettings(oldDirectoryPath), new MSSettings(newDirectoryPath));

    public static void CopyDirectory(string oldDirectoryPath, string newDirectoryPath, MSSettings oldSettings, MSSettings newSettings) => CopyDirectory(new MSSettings(oldDirectoryPath, oldSettings), new MSSettings(newDirectoryPath, newSettings));

    public static void CopyDirectory(MSSettings oldSettings, MSSettings newSettings)
    {
        // TODO: Implement copying logic here
    }

    #endregion

    #endregion


    #region RenameAPI 重命名

    #region 文件
    public static void RenameFile(string oldFilePath, string newFilePath) => RenameFile(new MSSettings(oldFilePath), new MSSettings(newFilePath));

    public static void RenameFile(string oldFilePath, string newFilePath, MSSettings oldSettings, MSSettings newSettings) => RenameFile(new MSSettings(oldFilePath, oldSettings), new MSSettings(newFilePath, newSettings));

    public static void RenameFile(MSSettings oldSettings, MSSettings newSettings)
    {
        // TODO: Implement renaming logic here
    }
    #endregion

    #region 文件夹
    public static void RenameDirectory(string oldDirectoryPath, string newDirectoryPath) => RenameDirectory(new MSSettings(oldDirectoryPath), new MSSettings(newDirectoryPath));

    public static void RenameDirectory(string oldDirectoryPath, string newDirectoryPath, MSSettings oldSettings, MSSettings newSettings) => RenameDirectory(new MSSettings(oldDirectoryPath, oldSettings), new MSSettings(newDirectoryPath, newSettings));

    public static void RenameDirectory(MSSettings oldSettings, MSSettings newSettings)
    {
        // TODO: Implement renaming logic here
    }
    #endregion

    #endregion


    #region UtilAPI 辅助函数

    #region EncrypyAPI 加密
    public static byte[] EncryptBytes(byte[] bytes, string password = null)
    {
        if (string.IsNullOrEmpty(password))
            password = MSSettings.Instance.EncryptionKey;
        return new AESEncryption().Encrypt(bytes, password, MSSettings.DefaultSettings.MainSettings.BufferSize);
    }

    public static byte[] DecryptBytes(byte[] bytes, string password = null)
    {
        if (string.IsNullOrEmpty(password))
            password = MSSettings.Instance.EncryptionKey;
        return new AESEncryption().Decrypt(bytes, password, MSSettings.DefaultSettings.MainSettings.BufferSize);
    }

    public static string EncryptString(string str, string password = null)
    {
        return Convert.ToBase64String(EncryptBytes(MSSettings.DefaultSettings.MainSettings.Encoding.GetBytes(str), password));
    }

    public static string DecryptString(string str, string password = null)
    {
        return MSSettings.DefaultSettings.MainSettings.Encoding.GetString(DecryptBytes(Convert.FromBase64String(str), password));
    }
    #endregion

    #region CompressionAPI 压缩
    public static byte[] CompressBytes(byte[] bytes)
    {
        // TODO: Implement compressing logic here
        throw new NotImplementedException();
    }

    public static byte[] DecompressBytes(byte[] bytes)
    {
        // TODO: Implement decompressing logic here
        throw new NotImplementedException();
    }

    public static string CompressString(string str)
    {
        return Convert.ToBase64String(CompressBytes(MSSettings.DefaultSettings.MainSettings.Encoding.GetBytes(str)));
    }

    public static string DecompressString(string str)
    {
        return MSSettings.DefaultSettings.MainSettings.Encoding.GetString(DecompressBytes(Convert.FromBase64String(str)));
    }
    #endregion

    #region TimeStampAPI 时间戳
    public static DateTime GetTimestamp() => GetTimestamp(new MSSettings((string)null));

    public static DateTime GetTimestamp(string filePath) => GetTimestamp(new MSSettings(filePath));

    public static DateTime GetTimestamp(string filePath, MSSettings settings) => GetTimestamp(new MSSettings(filePath, settings));

    public static DateTime GetTimestamp(MSSettings settings)
    {
        return settings.DirectoryStrategy switch
        {
            DirectoryStrategy.UserDir or DirectoryStrategy.GameDir => IOHelper.GetTimestamp(settings.FullPath),
            DirectoryStrategy.PlayerPrefs => new DateTime(
                long.Parse(PlayerPrefs.GetString("timestamp_" + settings.FullPath, "0")), DateTimeKind.Utc),
            _ => throw new NotSupportedException(
                "GetTimestamp can only be used when the location is set to File or PlayerPrefs.")
        };
    }
    #endregion

    #endregion
}
