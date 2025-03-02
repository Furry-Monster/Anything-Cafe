using UnityEngine.Events;

public interface ICanBeConditional
{
    void Show();

    void Hide();

    public UnityEvent OnConditionsFailed { get; }
}
