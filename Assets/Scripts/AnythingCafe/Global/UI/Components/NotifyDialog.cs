using DG.Tweening;
using UnityEngine.UI;

public class NotifyDialog :
    ReactiveComponent, 
    IInitializable, 
    IHasDataTemplate<NotifyDialogModel>,
    ICanRequest
{
    private Text _text;
    private Button _closeButton;
    private Text _closeButtonText;

    private Sequence _sequence;
    private NotifyDialogModel _model;
    private NotifyDialogState _state;

    public void Init()
    {
        _closeButton.onClick.RemoveAllListeners();

    }

    public void Init(NotifyDialogModel model)
    {
    }

    private enum NotifyDialogState
    {
        Opening,
        Idling,
        Closing,
    }

    public void Request()
    {
        // «Î«Ûπÿ±’
        switch (_state)
        {
            case NotifyDialogState.Opening:
            case NotifyDialogState.Closing:
                break;
            case NotifyDialogState.Idling:
            default:
                _ = UIManager.Instance.CloseReactiveComponent(this);
                break;
        }
    }
}

public class NotifyDialogModel
{
    public string Message;
    public ButtonDataTemplate CloseButtonData;
}
