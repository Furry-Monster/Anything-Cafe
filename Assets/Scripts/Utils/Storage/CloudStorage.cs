using Cysharp.Threading.Tasks;

/// <summary>
/// �ƶ˴洢��steam�ȣ�
/// </summary>
public abstract class CloudStorage : IStorageUtility, ICloudStorage
{
    public abstract UniTask Connect();

    public abstract void Disconnect();

    public abstract void Save(string fileName, string content);

    public abstract string Load(string fileName);
}
