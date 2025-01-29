public interface IObservable
{
    bool Subscribe(Observer newObserver);

    bool Unsubscribe(string observerName);

    bool Unsubscribe(Observer observer);
}
