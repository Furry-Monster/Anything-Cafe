public class DialogueNode
{
    public int NodeId { get; set; }
    public string Text { get; set; }
    public NodeAction NodeAction { get; set; }
    public int NextNodeId { get; set; }

    public bool IsPlayed { get; set; } = false;

    public DialogueNode(int nodeId = -1, string text = "", NodeAction nodeAction = NodeAction.None, int nextNodeId = -1)
    {
        NodeId = nodeId;
        Text = text;
        NodeAction = nodeAction;
        NextNodeId = nextNodeId;
    }
}
