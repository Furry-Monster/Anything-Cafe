public struct DialogueNode
{
    public string AvatarName { get; set; }
    public string Text { get; set; }
    public NodeAction NodeAction { get; set; }

    public DialogueNode(string text) : this("", text, NodeAction.None) { }

    public DialogueNode(string avatarName, string text) : this(avatarName, text, NodeAction.None) { }

    public DialogueNode(string text, NodeAction nodeAction) : this("", text, nodeAction) { }

    public DialogueNode(string avatarName, string text, NodeAction nodeAction)
    {
        AvatarName = avatarName;
        Text = text;
        NodeAction = nodeAction;
    }
}
