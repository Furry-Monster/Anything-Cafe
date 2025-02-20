using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MSReader:IDisposable
{
    public MSSettings Settings;

    public abstract void Dispose();
}
