using Cysharp.Threading.Tasks;

/// <summary>
/// ÔÆ¶Ë´æ´¢£¨steamµÈ£©
/// </summary>
public abstract class CloudStorage : IStorageUtility, ICloudStorage
{
    public abstract UniTask Connect();

    public abstract void Disconnect();

    public abstract void Save(string fileName, string content);

    public abstract string Load(string fileName);
}
