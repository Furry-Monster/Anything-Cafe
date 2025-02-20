public interface IEncryption
{
    public byte[] Encrypt(byte[] plainText, string password, int bufferSize);
    public byte[] Decrypt(byte[] cipherText, string password, int bufferSize);
}
