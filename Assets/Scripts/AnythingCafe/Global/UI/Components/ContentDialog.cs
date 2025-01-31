using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ContentDialog : ClosableComponent, IInitializable, IHasDataTemplate<ContentDialogModel>
{
    private Text _text;
    private Button _leftBtn;
    private Text _leftBtnText;
    private Button _rightBtn;
    private Text _rightBtnText;

    private Sequence _sequence;
    private ContentDialogModel _model;
    private ContentDialogState _state;


    public void Init()
    {

    }

    public void Init(ContentDialogModel model)
    {

    }

    private enum ContentDialogState
    {
        Opening,
        Idling,
        Closing,

    }
}

public class ContentDialogModel
{
    public string Text;
    public ButtonDataTemplate LeftButtonData;
    public ButtonDataTemplate RightButtonData;
}