using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class ContentDialog :
    ReactiveComponent,
    IInitializable,
    IHasDataTemplate<ContentDialogModel>,
    ICanRequest
{
    private Text _text;
    private Button _leftBtn;
    private Text _leftBtnText;
    private Button _rightBtn;
    private Text _rightBtnText;

    private Sequence _sequence;
    private ContentDialogModel _model;
    private ContentDialogState _state;

    public void Init() => gameObject.SetActive(false);

    public void Init(ContentDialogModel model)
    {
        _model = model;

        _text.text = model.Text;

        // 设置左按钮
        _leftBtnText.text = model.LeftButtonData.Text;
        _leftBtn.interactable = model.LeftButtonData.IsInteractable;
        _leftBtn.onClick.RemoveAllListeners();
        _leftBtn.onClick.AddListener(() =>
        {
            _model.LeftButtonData.OnClick?.Invoke();
            Close();
        });

        // 设置右按钮
        _rightBtnText.text = model.RightButtonData.Text;
        _rightBtn.interactable = model.RightButtonData.IsInteractable;
        _rightBtn.onClick.RemoveAllListeners();
        _rightBtn.onClick.AddListener(() =>
        {
            _model.RightButtonData.OnClick?.Invoke();
            Close();
        });

    }

    public override UniTask Open()
    {
        return base.Open();
    }

    public override UniTask Close()
    {
        return base.Close();
    }

    private enum ContentDialogState
    {
        Opening,
        Idling,
        Closing,
    }

    public void Request()
    {
        // 请求关闭
        switch (_state)
        {
            case ContentDialogState.Opening:
            case ContentDialogState.Closing:
                break;
            case ContentDialogState.Idling:
            default:
                _ = UIManager.Instance.CloseReactiveComponent(this);
                break;
        }
    }
}

public class ContentDialogModel
{
    public string Text;
    public ButtonDataTemplate LeftButtonData;
    public ButtonDataTemplate RightButtonData;
}