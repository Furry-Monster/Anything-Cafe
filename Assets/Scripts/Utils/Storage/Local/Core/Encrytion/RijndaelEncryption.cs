using System.IO;
using System.Security.Cryptography;

public class RijndaelEncryption : IEncryption, IStreamEncryption
{
    private const int IvSize = 16;
    private const int KeySize = 16;
    private const int PwIterations = 100;

    public byte[] Encrypt(byte[] plainText, string password, int bufferSize)
    {
        using var input = new MemoryStream(plainText);
        using var output = new MemoryStream();

        Encrypt(input, output, password, bufferSize);
        return output.ToArray();
    }

    public byte[] Decrypt(byte[] cipherText, string password, int bufferSize)
    {
        using var input = new MemoryStream(cipherText);
        using var output = new MemoryStream();

        Decrypt(input, output, password, bufferSize);
        return output.ToArray();
    }

    public void Encrypt(Stream input, Stream output, string password, int bufferSize)
    {
        input.Position = 0L;

        using var rijndael = Rijndael.Create();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.GenerateIV();
        var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, rijndael.IV, PwIterations);
        rijndael.Key = rfc2898DeriveBytes.GetBytes(KeySize);
        output.Write(rijndael.IV, 0, IvSize);

        using var encryptor = rijndael.CreateEncryptor();
        using var cryptoStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);
        CopyStream(input, cryptoStream, bufferSize);
    }

    public void Decrypt(Stream input, Stream output, string password, int bufferSize)
    {
        using var rijndael = Rijndael.Create();
        var buffer = new byte[IvSize];
        input.Read(buffer, 0, IvSize);
        rijndael.IV = buffer;
        var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, rijndael.IV, PwIterations);
        rijndael.Key = rfc2898DeriveBytes.GetBytes(KeySize);

        using var decryptor = rijndael.CreateDecryptor();
        using var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
        CopyStream(cryptoStream, output, bufferSize);

        output.Position = 0L;
    }

    public void CopyStream(Stream input, Stream output, int bufferSize)
    {
        var buffer = new byte[bufferSize];
        int count;

        while ((count = input.Read(buffer, 0, bufferSize)) > 0)
            output.Write(buffer, 0, count);
    }
}

