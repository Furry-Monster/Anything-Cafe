using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel :
    GlobalComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Components")]
    [SerializeField]
    private Slider _audioSlider;
    [SerializeField]
    private Slider _soundEffectSlider;
    [SerializeField]
    private Slider _musicSlider;
    [SerializeField]
    private Slider _sexVolumeSlider;
    [SerializeField]
    private Toggle _screenModeToggle;

    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        // Set initial values
        foreach (OptionKey optionKey in Enum.GetValues(typeof(OptionKey)))
            UpdateOptionComponentView(optionKey);

        OptionManager.Instance.OnOptionChanged += UpdateOptionComponentView;

        gameObject.SetActive(false);
    }

    private void UpdateOptionComponentView(OptionKey key)
    {
        switch (key)
        {
            case OptionKey.GlobalVolume:
                _audioSlider.value = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.EroVolume:
                _sexVolumeSlider.value = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.MusicVolume:
                _musicSlider.value = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.AmbientVolume:
            case OptionKey.SFXVolume:
            case OptionKey.UIVolume:
                _soundEffectSlider.value = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.ScreenMode:
                _screenModeToggle.isOn = OptionManager.Instance.GetValue<ScreenMode>(key) == ScreenMode.FullScreen;
                break;
        }
    }

    public void OnAudioSliderChanged(float value) =>
        OptionManager.Instance.SetValue(OptionKey.GlobalVolume, value);

    public void OnSexVolumeSliderChanged(float value) =>
        OptionManager.Instance.SetValue(OptionKey.EroVolume, value);

    public void OnMusicSliderChanged(float value) =>
        OptionManager.Instance.SetValue(OptionKey.MusicVolume, value);

    public void OnSoundEffectSliderChanged(float value)
    {
        OptionManager.Instance.SetValue(OptionKey.SFXVolume, value);
        OptionManager.Instance.SetValue(OptionKey.AmbientVolume, value);
        OptionManager.Instance.SetValue(OptionKey.UIVolume, value);
    }

    public void OnScreenModeToggled()
    {
        var currentScreenMode = OptionManager.Instance.GetValue<ScreenMode>(OptionKey.ScreenMode);
        OptionManager.Instance.SetValue(OptionKey.ScreenMode, currentScreenMode == ScreenMode.FullScreen ? ScreenMode.Windowed : ScreenMode.FullScreen);
    }

    public void OnCloseClick()
    {
        _ = UIManager.Instance.CloseReactive(this);
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DropIn();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RiseBack();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence DropIn()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
        }).Append(transform.DOLocalMoveY(0, 0.5f));
    }

    private Sequence RiseBack()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        }).Append(transform.DOLocalMoveY(1700, 0.5f));
    }
}
