public interface ISerializedStorage
{
    public void Save(byte[] data, string path);

    public byte[] Load(string path);
}
