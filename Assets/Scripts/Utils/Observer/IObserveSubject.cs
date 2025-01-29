using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserveSubject
{
    bool Notify();

    void Reset();
}
