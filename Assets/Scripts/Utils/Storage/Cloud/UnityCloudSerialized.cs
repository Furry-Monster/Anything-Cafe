using Cysharp.Threading.Tasks;

public class UnityCloudSerialized : CloudSerializedStorage
{
    public override UniTask Connect()
    {
        throw new System.NotImplementedException();
    }

    public override void Disconnect()
    {
        throw new System.NotImplementedException();
    }

    public override void Save(byte[] data, string path)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] Load(string path)
    {
        throw new System.NotImplementedException();
    }
}
