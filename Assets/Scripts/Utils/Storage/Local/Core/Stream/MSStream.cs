using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using UnityEditor.VersionControl;
using UnityEngine;

public static class MSStream
{
    public static Stream CreateStream(MSSettings settings, MSFileMode mode)
    {
        var isWrite = (mode != 0);
        Stream stream = null;

        try
        {
            switch (settings.DirectoryStrategy)
            {
                case DirectoryStrategy.UserDir or DirectoryStrategy.GameDir:
                    {
                        if (!isWrite && !IOHelper.FileExists(settings.FullPath)) return null;
                        stream = new MSFileStream(settings.FullPath, mode, settings.BufferSize, false);
                        break;
                    }
                case DirectoryStrategy.PlayerPrefs:
                    {
                        if (isWrite)
                        {
                            stream = new MSPlayerPrefsStream(settings.FullPath, settings.BufferSize, mode == MSFileMode.Append);
                        }
                        else
                        {
                            if (!PlayerPrefs.HasKey(settings.FullPath)) return null;
                            stream = new MSPlayerPrefsStream(settings.FullPath);
                        }

                        break;
                    }
            }

            stream = DecorateStream(stream, settings, mode);

            return stream;
        }
        catch (Exception)
        {
            stream?.Dispose();
            throw;
        }
    }

    public static Stream DecorateStream(Stream stream, MSSettings settings, MSFileMode mode)
    {
        try
        {
            var isWrite = (mode != 0);

            if (settings.Encryption != EncryptionMode.None)
            {
                IEncryption algorithm = settings.Encryption switch
                {
                    EncryptionMode.AES => new AESEncryption(),
                    EncryptionMode.Rinjdael => new RijndaelEncryption(),
                    _ => null
                };

                // TODO: wrap stream with encryption stream
            }

            if (settings.Compression != CompressionMode.None)
            {
                if (settings.Compression == CompressionMode.Gzip)
                {
                    stream = isWrite
                        ? new GZipStream(stream, System.IO.Compression.CompressionMode.Compress)
                        : new GZipStream(stream, System.IO.Compression.CompressionMode.Decompress);
                }
            }

            return stream;
        }
        catch (Exception ex)
        {
            stream?.Dispose();
            if (ex.GetType() == typeof(CryptographicException))
                throw new CryptographicException("Could not decrypt file. Are you sure the password is correct?");
            throw;
        }
    }

    public static void CopyTo(Stream source, Stream destination) => source.CopyTo(destination);
}
