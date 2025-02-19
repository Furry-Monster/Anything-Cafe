using Cysharp.Threading.Tasks;

public interface ICloudStorage
{
    public UniTask Connect();

    public void Disconnect();
}
