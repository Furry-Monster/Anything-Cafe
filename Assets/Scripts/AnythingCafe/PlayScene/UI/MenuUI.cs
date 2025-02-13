using Cysharp.Threading.Tasks;

public class MenuUI : PlaySceneComponent, IInitializable
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

    public void OnContinueClick()
    {

    }

    public void OnMainMenuClick()
    {

    }

    public void OnSettingClick()
    {

    }
}
