public interface IStorageUtility
{
    public void Save(string fileName, string content);

    public string Load(string fileName);
}
