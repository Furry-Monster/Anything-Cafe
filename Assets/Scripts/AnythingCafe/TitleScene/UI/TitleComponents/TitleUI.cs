using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleUI :
    TitleComponent,
    IInitializable
{
    public void Init() => gameObject.SetActive(true);

    public void OnThanksClick()
    {

    }

    public void OnSupportClick()
    {

    }

    public void OnNoticeClick()
    {

    }

    public void OnAboutClick() => Application.OpenURL("https://www.google.com");

    public void OnNewGameClick()
    {

    }

    public void OnContinueClick()
    {

    }

    public void OnGalleryClick()
    {

    }

    public void OnSettingClick()
    {

    }

    public void OnExitClick()
    {
        // TODO: 打开提示框，询问是否退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
