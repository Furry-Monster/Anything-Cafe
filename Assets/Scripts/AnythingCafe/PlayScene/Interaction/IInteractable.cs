using UnityEngine.Events;

public interface IInteractable
{
    bool Interactable { get; set; }
    bool IsInteracting { get; }

    UnityEvent OnInteractionStart { get; }
    UnityEvent OnInteractionEnd { get; }

    void OnInteract();
    void OnInteractionBegin();
    void OnInteractionComplete();
    void OnInteractionCancel();
}
