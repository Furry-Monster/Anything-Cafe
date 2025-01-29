using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservable
{
    bool Subscribe(Observer newObserver);

    bool Unsubscribe(string observerName);

    bool Unsubscribe(Observer observer);
}
