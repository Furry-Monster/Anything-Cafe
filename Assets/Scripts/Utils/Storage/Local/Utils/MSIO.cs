using System;
using System.IO;
using UnityEngine;

/// <summary>
/// MonStore��IO�࣬��װ�˶��ļ��Ķ�д��Ŀ¼�Ĵ�����ɾ���Ȳ���
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
        // ���磺path = "Assets/Resources/Prefabs/Cube.prefab"
        // �򷵻� "Assets/Resources/Prefabs"
        // ע�⣺���·����û�зָ������򷵻ؿ��ַ���

        var ch = UsesForwardSlash(path) ? '/' : '\\';
        var length = path.LastIndexOf(ch); // ���һ���ָ�����λ��

        if (length == path.Length - 1)
            length = path[..length].LastIndexOf(ch); // �����ڶ����ָ�����λ��
        if (length == -1)
            MSDebugger.LogError("���õ�·����ʽ����ȷ������·���Ƿ�����ָ���(�ַ�'/'��'\\')");

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
        var directories = System.IO.Directory.GetDirectories(path);
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
                // ���Ǳ����ļ�
                DeleteFile(str2);
                CopyFile(settings.FullPath, str2);

                try
                {
                    // ����str1��settings.FullPath������
                    DeleteFile(settings.FullPath);
                    MoveFile(str1, settings.FullPath);
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
