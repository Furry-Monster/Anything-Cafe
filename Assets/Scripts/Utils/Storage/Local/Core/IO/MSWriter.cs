using System;

public abstract class MSWriter : IDisposable
{
    public MSSettings Settings;

    public abstract void Dispose();
}
