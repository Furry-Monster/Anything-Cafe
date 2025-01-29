using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable : IObservable, IObserveSubject
{
    public bool Subscribe(Observer newObserver)
    {
        throw new System.NotImplementedException();
    }

    public bool Unsubscribe(string observerName)
    {
        throw new System.NotImplementedException();
    }

    public bool Unsubscribe(Observer observer)
    {
        throw new System.NotImplementedException();
    }

    public bool Notify()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}
