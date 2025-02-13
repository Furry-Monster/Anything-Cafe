using JetBrains.Annotations;
using System;

public class ButtonDataTemplate : IDataTemplate
{
    public string Text;
    public string Tooltip;
    public bool IsInteractable;
    public Func<bool> OnClick;

    public ButtonDataTemplate(string text, [CanBeNull] string tooltip, bool isInteractable, Action onClick = null)
    {
        Text = text;
        Tooltip = tooltip;
        IsInteractable = isInteractable;
        OnClick = () =>
        {
            onClick?.Invoke();
            return true;
        };
    }

    public ButtonDataTemplate(string text, [CanBeNull] string tooltip, bool isInteractable, Func<bool> onClick)
    {
        Text = text;
        Tooltip = tooltip;
        IsInteractable = isInteractable;
        OnClick = onClick;
    }
}
