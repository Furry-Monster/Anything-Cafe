using Cysharp.Threading.Tasks;

/// <summary>
/// ÔÆ¶Ë´æ´¢£¨steamµÈ£©
/// </summary>
public abstract class CloudSerializedStorage : ISerializedStorage, ICloudStorage
{
    public abstract UniTask Connect();

    public abstract void Disconnect();

    public abstract void Save(byte[] data, string path);

    public abstract byte[] Load(string path);
}
