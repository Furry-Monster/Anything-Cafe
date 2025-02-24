using System;
using System.IO;
using UnityEngine;

// MSPlayerPrefsStream 类继承自 MemoryStream，用于处理 PlayerPrefs 中的数据流
public class MSPlayerPrefsStream : MemoryStream
{
    private readonly string _path; // 存储路径
    private readonly bool _append; // 是否追加模式
    private readonly bool _isWriteStream; // 是否为写入流
    private bool _isDisposed; // 是否已释放

    // 构造函数，读取模式
    public MSPlayerPrefsStream(string path)
        : base(GetData(path))
    {
        _path = path;
        _append = false;
    }

    // 构造函数，写入模式
    public MSPlayerPrefsStream(string path, int bufferSize, bool append = false)
        : base(bufferSize)
    {
        _path = path;
        _append = append;
        _isWriteStream = true;
    }

    // 获取数据，如果路径不存在则抛出 FileNotFoundException
    private static byte[] GetData(string path) =>
        PlayerPrefs.HasKey(path)
        ? Convert.FromBase64String(PlayerPrefs.GetString(path))
        : throw new FileNotFoundException("File \"" + path + "\" could not be found in PlayerPrefs");

    // 释放资源
    protected override void Dispose(bool disposing)
    {
        // 检查是否已经释放过资源
        if (_isDisposed)
            return;
        _isDisposed = true;

        // 如果是写入流
        if (_isWriteStream && Length > 0L)
        {
            // 如果是追加模式
            if (_append)
            {
                // 从 PlayerPrefs 中获取现有数据并解码为字节数组
                var src = Convert.FromBase64String(PlayerPrefs.GetString(_path));
                // 获取当前流中的数据
                var array = ToArray();
                // 创建一个新的字节数组，长度为现有数据和当前流数据的总和
                var numArray = new byte[src.Length + array.Length];
                // 将现有数据复制到新的字节数组中
                Buffer.BlockCopy(src, 0, numArray, 0, src.Length);
                // 将当前流数据复制到新的字节数组中
                Buffer.BlockCopy(array, 0, numArray, src.Length, array.Length);
                // 将合并后的字节数组编码为 Base64 字符串并存储到 PlayerPrefs 中
                PlayerPrefs.SetString(_path, Convert.ToBase64String(numArray));
                // 保存 PlayerPrefs
                PlayerPrefs.Save();
            }
            else
            {
                // 如果不是追加模式，将当前流数据编码为 Base64 字符串并存储到临时路径中
                PlayerPrefs.SetString(_path + ".tmp", Convert.ToBase64String(ToArray()));
            }
            // 存储当前时间戳到 PlayerPrefs 中
            PlayerPrefs.SetString("timestamp_" + _path, DateTime.UtcNow.Ticks.ToString());
        }


        base.Dispose(disposing);
    }
}
