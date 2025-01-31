using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotifyDialog : ClosableComponent, IInitializable, IHasDataTemplate<NotifyDialogModel>
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
}

public class NotifyDialogModel
{
    public string Message;
    public ButtonDataTemplate CloseButtonData;
}
