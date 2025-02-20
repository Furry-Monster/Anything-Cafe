using System;
using System.IO;
using UnityEngine;

/// <summary>
/// MonStore的IO类，封装了对文件的读写、目录的创建、删除等操作
/// </summary>
public class MSIO
{
    internal static string PersistentDataPath = Application.persistentDataPath;
    internal static string DataPath = Application.dataPath;
    internal static string BackupSuffix = ".bak";
    internal static string TempFileSuffix = ".tmp";

    public static DateTime GetTimestamp(string filePath)
    {
        return !FileExists(filePath)
            ? new DateTime(1970,
                1,
                1,
                0,
                0,
                0,
                0,
                DateTimeKind.Utc)
            : File.GetLastWriteTime(filePath)
                .ToUniversalTime();
    }

    public static string GetExtension(string path) => Path.GetExtension(path);

    public static void DeleteFile(string filePath)
    {
        if (!FileExists(filePath))
            return;
        File.Delete(filePath);
    }

    public static bool FileExists(string filePath) => File.Exists(filePath);

    public static void MoveFile(string sourcePath, string destPath) => File.Move(sourcePath, destPath);

    public static void CopyFile(string sourcePath, string destPath) => File.Copy(sourcePath, destPath);

    public static void MoveDirectory(string sourcePath, string destPath) => System.IO.Directory.Move(sourcePath, destPath);

    public static void CreateDirectory(string directoryPath) => System.IO.Directory.CreateDirectory(directoryPath);

    public static bool DirectoryExists(string directoryPath) => System.IO.Directory.Exists(directoryPath);

    public static string GetDirectoryPath(string path, char separator = '/')
    {
        // 例如：path = "Assets/Resources/Prefabs/Cube.prefab"
        // 则返回 "Assets/Resources/Prefabs"
        // 注意：如果路径中没有分隔符，则返回空字符串

        var ch = UsesForwardSlash(path) ? '/' : '\\';
        var length = path.LastIndexOf(ch); // 最后一个分隔符的位置

        if (length == path.Length - 1)
            length = path[..length].LastIndexOf(ch); // 倒数第二个分隔符的位置
        if (length == -1)
            MSDebugger.LogError("设置的路径格式不正确，请检查路径是否包含分隔符(字符'/'或'\\')");

        return path[..length]; // 路径上一级目录
    }

    public static bool UsesForwardSlash(string path) => path.Contains("/");

    public static string CombinePathAndFilename(string directoryPath, string fileOrDirectoryName)
    {
        if (directoryPath[^1] != '/' && directoryPath[^1] != '\\')
            directoryPath += "/"; // 确保目录路径以分隔符结尾
        return directoryPath + fileOrDirectoryName; // 合并目录路径和文件或目录名
    }

    public static string[] GetDirectories(string path, bool getFullPaths = true)
    {
        var directories = System.IO.Directory.GetDirectories(path);
        for (var index = 0; index < directories.Length; ++index)
        {
            if (!getFullPaths)
                directories[index] = Path.GetFileName(directories[index]);
            directories[index] = directories[index].Replace("\\", "/"); // 统一使用'/'作为分隔符
        }
        return directories;
    }

    public static void DeleteDirectory(string directoryPath)
    {
        if (!DirectoryExists(directoryPath))
            return;
        System.IO.Directory.Delete(directoryPath, true);
    }

    public static string[] GetFiles(string path, bool getFullPaths = true)
    {
        var files = System.IO.Directory.GetFiles(path);
        if (!getFullPaths)
        {
            for (var index = 0; index < files.Length; ++index)
                files[index] = Path.GetFileName(files[index]);
        }
        return files;
    }

    public static byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

    public static void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

    public static void CommitBackup(MSSettings settings)
    {
        MSDebugger.Log("Committing backup for " + settings.SavePath + " in " + settings.DirectoryStrategy);
        var str1 = settings.FullPath + ".tmp";
        if (settings.DirectoryStrategy == DirectoryStrategy.UserDir)
        {
            var str2 = settings.FullPath + ".tmp.bak";
            if (FileExists(settings.FullPath))
            {
                // 覆盖备份文件
                DeleteFile(str2);
                CopyFile(settings.FullPath, str2);

                try
                {
                    // 保存str1到settings.FullPath并覆盖
                    DeleteFile(settings.FullPath);
                    MoveFile(str1, settings.FullPath);
                }
                catch (Exception ex)
                {
                    // 尝试还原原文件
                    try
                    {
                        DeleteFile(settings.FullPath);
                    }
                    catch
                    {
                        MSDebugger.LogError("无法删除文件：" + settings.FullPath + "\n错误信息：" + ex.Message);
                    }

                    MoveFile(str2, settings.FullPath);
                    throw;
                }

                DeleteFile(str2);
            }
            else
                MoveFile(str1, settings.FullPath);
        }
        else
        {
            if (settings.DirectoryStrategy != DirectoryStrategy.PlayerPrefs)
                return;
            PlayerPrefs.SetString(settings.FullPath, PlayerPrefs.GetString(str1));
            PlayerPrefs.DeleteKey(str1);
            PlayerPrefs.Save();
        }
    }
}
