public struct MSData
{
    public MSType Type;
    public byte[] Bytes;

    public MSData(MSType type, byte[] bytes)
    {
        Type = type;
        Bytes = bytes;
    }
}
