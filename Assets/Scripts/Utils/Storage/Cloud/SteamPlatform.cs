using Cysharp.Threading.Tasks;

public class SteamPlatform : CloudSerializedStorage
{
    public const uint AppId = 123456; // Dummy Steam AppId

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
