using Cysharp.Threading.Tasks;

public class CoffeeUI : PlaySceneComponent, IInitializable
{
    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public override UniTask Open()
    {
        return base.Open();
    }

    public override UniTask Close()
    {
        return base.Close();
    }

    public async UniTask OnTrashClick()
    {
        var data = new ContentDialogModel
        {
            LeftButtonData = new ButtonDataTemplate("Yes", null, true),
            RightButtonData = new ButtonDataTemplate("No", null, false)
        };
        await UIManager.Instance.OpenGlobal<ContentDialog, ContentDialogModel>(data);
    }

    public void OnLetHerDrinkClick()
    {
        
    }

    public void OnDrinkYourselfClick()
    {

    }
}
