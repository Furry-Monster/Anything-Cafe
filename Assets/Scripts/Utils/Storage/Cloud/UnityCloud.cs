using Cysharp.Threading.Tasks;

public class UnityCloud : CloudStorage
{
    public override UniTask Connect()
    {
        throw new System.NotImplementedException();
    }

    public override void Disconnect()
    {
        throw new System.NotImplementedException();
    }

    public override void Save(string fileName, string content)
    {
        throw new System.NotImplementedException();
    }

    public override string Load(string fileName)
    {
        throw new System.NotImplementedException();
    }
}
