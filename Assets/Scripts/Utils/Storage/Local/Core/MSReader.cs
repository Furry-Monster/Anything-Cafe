using System;

public abstract class MSReader : IDisposable
{
    public MSSettings Settings;

    public abstract void Dispose();
}
