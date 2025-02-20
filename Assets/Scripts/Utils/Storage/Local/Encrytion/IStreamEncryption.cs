using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IStreamEncryption
{
    public void Encrypt(Stream input, Stream output, string password, int bufferSize);
    public void Decrypt(Stream input, Stream output, string password, int bufferSize);

    public void CopyStream(Stream input, Stream output, int bufferSize);
}
