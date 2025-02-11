public struct DialogueData
{
    public string Talker { get; set; }
    public string AvatarName { get; set; }
    public string Text { get; set; }
    public ClickAction ClickAction { get; set; }

    public DialogueData(string talker, string text) : this(talker, "", text, ClickAction.None) { }

    public DialogueData(string talker, string avatarName, string text) : this(talker, avatarName, text, ClickAction.None) { }

    public DialogueData(string talker, string text, ClickAction clickAction) : this(talker, "", text, clickAction) { }

    public DialogueData(string talker, string avatarName, string text, ClickAction clickAction)
    {
        Talker = talker;
        AvatarName = avatarName;
        Text = text;
        ClickAction = clickAction;
    }
}
