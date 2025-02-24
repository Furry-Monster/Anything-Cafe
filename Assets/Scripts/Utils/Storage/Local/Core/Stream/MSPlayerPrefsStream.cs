using System;
using System.IO;
using UnityEngine;

// MSPlayerPrefsStream ��̳��� MemoryStream�����ڴ��� PlayerPrefs �е�������
public class MSPlayerPrefsStream : MemoryStream
{
    private readonly string _path; // �洢·��
    private readonly bool _append; // �Ƿ�׷��ģʽ
    private readonly bool _isWriteStream; // �Ƿ�Ϊд����
    private bool _isDisposed; // �Ƿ����ͷ�

    // ���캯������ȡģʽ
    public MSPlayerPrefsStream(string path)
        : base(GetData(path))
    {
        _path = path;
        _append = false;
    }

    // ���캯����д��ģʽ
    public MSPlayerPrefsStream(string path, int bufferSize, bool append = false)
        : base(bufferSize)
    {
        _path = path;
        _append = append;
        _isWriteStream = true;
    }

    // ��ȡ���ݣ����·�����������׳� FileNotFoundException
    private static byte[] GetData(string path) =>
        PlayerPrefs.HasKey(path)
        ? Convert.FromBase64String(PlayerPrefs.GetString(path))
        : throw new FileNotFoundException("File \"" + path + "\" could not be found in PlayerPrefs");

    // �ͷ���Դ
    protected override void Dispose(bool disposing)
    {
        // ����Ƿ��Ѿ��ͷŹ���Դ
        if (_isDisposed)
            return;
        _isDisposed = true;

        // �����д����
        if (_isWriteStream && Length > 0L)
        {
            // �����׷��ģʽ
            if (_append)
            {
                // �� PlayerPrefs �л�ȡ�������ݲ�����Ϊ�ֽ�����
                var src = Convert.FromBase64String(PlayerPrefs.GetString(_path));
                // ��ȡ��ǰ���е�����
                var array = ToArray();
                // ����һ���µ��ֽ����飬����Ϊ�������ݺ͵�ǰ�����ݵ��ܺ�
                var numArray = new byte[src.Length + array.Length];
                // ���������ݸ��Ƶ��µ��ֽ�������
                Buffer.BlockCopy(src, 0, numArray, 0, src.Length);
                // ����ǰ�����ݸ��Ƶ��µ��ֽ�������
                Buffer.BlockCopy(array, 0, numArray, src.Length, array.Length);
                // ���ϲ�����ֽ��������Ϊ Base64 �ַ������洢�� PlayerPrefs ��
                PlayerPrefs.SetString(_path, Convert.ToBase64String(numArray));
                // ���� PlayerPrefs
                PlayerPrefs.Save();
            }
            else
            {
                // �������׷��ģʽ������ǰ�����ݱ���Ϊ Base64 �ַ������洢����ʱ·����
                PlayerPrefs.SetString(_path + ".tmp", Convert.ToBase64String(ToArray()));
            }
            // �洢��ǰʱ����� PlayerPrefs ��
            PlayerPrefs.SetString("timestamp_" + _path, DateTime.UtcNow.Ticks.ToString());
        }


        base.Dispose(disposing);
    }
}
