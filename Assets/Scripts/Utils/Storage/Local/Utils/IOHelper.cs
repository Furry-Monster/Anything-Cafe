using System;
using System.IO;
using UnityEngine;

/// <summary>
/// MonStore��IO�࣬��װ�˶��ļ��Ķ�д��Ŀ¼�Ĵ�����ɾ���Ȳ���
/// </summary>
public class IOHelper
{
    internal static string PersistentDataPath = Application.persistentDataPath;
    internal static string DataPath = Application.dataPath;
    internal static string BackupSuffix = ".bak";
    internal static string TempFileSuffix = ".tmp";

    public static DateTime GetTimestamp(string filePath)
    {
        return !FileExists(filePath)
            ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            : File.GetLastWriteTimeUtc(filePath);
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

    public static void MoveDirectory(string sourcePath, string destPath) => Directory.Move(sourcePath, destPath);

    public static void CreateDirectory(string directoryPath) => Directory.CreateDirectory(directoryPath);

    public static bool DirectoryExists(string directoryPath) => System.IO.Directory.Exists(directoryPath);

    public static string GetDirectoryPath(string path, char separator = '/')
    {
        // ���磺path = "Assets/Resources/Prefabs/Cube.prefab"
        // �򷵻� "Assets/Resources/Prefabs"
        // ע�⣺���·����û�зָ������򷵻ؿ��ַ���

        var ch = UsesForwardSlash(path) ? '/' : '\\';
        var length = path.LastIndexOf(ch); // ���һ���ָ�����λ��

        if (length == path.Length - 1)
            length = path[..length].LastIndexOf(ch); // �����ڶ����ָ�����λ��
        if (length == -1)
        {
            MSDebugger.LogError("���õ�·����ʽ����ȷ������·���Ƿ�����ָ���(�ַ�'/'��'\\')");
            return string.Empty;
        }

        return path[..length]; // ·����һ��Ŀ¼
    }

    public static bool UsesForwardSlash(string path) => path.Contains("/");

    public static string CombinePathAndFilename(string directoryPath, string fileOrDirectoryName)
    {
        if (directoryPath[^1] != '/' && directoryPath[^1] != '\\')
            directoryPath += "/"; // ȷ��Ŀ¼·���Էָ�����β
        return directoryPath + fileOrDirectoryName; // �ϲ�Ŀ¼·�����ļ���Ŀ¼��
    }

    public static string[] GetDirectories(string path, bool getFullPaths = true)
    {
        var directories = Directory.GetDirectories(path);
        for (var index = 0; index < directories.Length; ++index)
        {
            if (!getFullPaths)
                directories[index] = Path.GetFileName(directories[index]);
            directories[index] = directories[index].Replace("\\", "/"); // ͳһʹ��'/'��Ϊ�ָ���
        }
        return directories;
    }

    public static void DeleteDirectory(string directoryPath)
    {
        if (!DirectoryExists(directoryPath))
            return;
        Directory.Delete(directoryPath, true);
    }

    public static string[] GetFiles(string path, bool getFullPaths = true)
    {
        var files = Directory.GetFiles(path);
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
        var tempFilePath = settings.FullPath + TempFileSuffix;

        switch (settings.DirectoryStrategy)
        {
            case DirectoryStrategy.UserDir:
            case DirectoryStrategy.GameDir:
                {
                    var backupFilePath = settings.FullPath + TempFileSuffix + BackupSuffix;
                    if (FileExists(settings.FullPath))
                    {
                        // ���Ǳ����ļ�
                        DeleteFile(backupFilePath);
                        CopyFile(settings.FullPath, backupFilePath);

                        try
                        {
                            // ����str1��settings.FullPath������
                            DeleteFile(settings.FullPath);
                            MoveFile(tempFilePath, settings.FullPath);
                        }
                        catch (Exception ex)
                        {
                            // ���Ի�ԭԭ�ļ�    
                            try
                            {
                                DeleteFile(settings.FullPath);
                            }
                            catch
                            {
                                MSDebugger.LogError("�޷�ɾ���ļ���" + settings.FullPath + "\n������Ϣ��" + ex.Message);
                            }

                            MoveFile(backupFilePath, settings.FullPath);
                            throw;
                        }

                        DeleteFile(backupFilePath);
                    }
                    else
                    {
                        MoveFile(tempFilePath, settings.FullPath);
                    }

                    break;
                }
            case DirectoryStrategy.PlayerPrefs:
                {
                    PlayerPrefs.SetString(settings.FullPath, PlayerPrefs.GetString(tempFilePath));
                    PlayerPrefs.DeleteKey(tempFilePath);
                    PlayerPrefs.Save();

                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
