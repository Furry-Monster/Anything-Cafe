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
        Stream stream = null;
        var fileInfo = new FileInfo(settings.SavePath);

        try
        {
            if (settings.DirectoryStrategy is DirectoryStrategy.UserDir or DirectoryStrategy.GameDir)
            {

            }
            else if (settings.DirectoryStrategy == DirectoryStrategy.PlayerPrefs)
            {

            }


            stream = DecorateStream(stream, settings, mode);

            return stream;
        }
        catch (Exception ex)
        {
            stream?.Dispose();
            throw;
        }
    }

    public static Stream DecorateStream(Stream stream, MSSettings settings, MSFileMode mode)
    {
        try
        {
            throw new NotImplementedException();
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
