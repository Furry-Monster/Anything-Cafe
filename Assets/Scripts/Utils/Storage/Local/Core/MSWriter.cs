using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MSWriter : IDisposable
{
    public MSSettings Settings;

    public abstract void Dispose();
}
