using System.IO;

// code from ES3FileStream.cs
public class MSFileStream : FileStream
{
    private bool _isDisposed;

    public MSFileStream(string path, MSFileMode fileMode, int bufferSize, bool useAsync)
        : base(GetPath(path, fileMode), GetFileMode(fileMode), GetFileAccess(fileMode), FileShare.None, bufferSize, useAsync)
    {
    }

    protected static string GetPath(string path, MSFileMode fileMode)
    {
        var directoryPath = IOHelper.GetDirectoryPath(path);
        if (fileMode != MSFileMode.Read && directoryPath != IOHelper.PersistentDataPath)
            IOHelper.CreateDirectory(directoryPath);
        return fileMode is not MSFileMode.Write or MSFileMode.Append or not MSFileMode.Write
            ? path
            : path + ".tmp";
    }

    protected static FileMode GetFileMode(MSFileMode fileMode) =>
        fileMode switch
        {
            MSFileMode.Read => FileMode.Open,
            MSFileMode.Write => FileMode.Create,
            _ => FileMode.Append
        };

    protected static FileAccess GetFileAccess(MSFileMode fileMode) =>
        fileMode == MSFileMode.Read ? FileAccess.Read : FileAccess.Write;

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;
        _isDisposed = true;
        base.Dispose(disposing);
    }
}
