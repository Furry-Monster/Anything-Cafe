using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialoguePlayer
{
    private readonly List<DialogueNode> _dialogueNodes;
    private DialogueNode _currentNode;

    private event Action OnDialoguePlay;
    private event Action OnDialogueEnd;

    #region Builder
    private DialoguePlayer(List<DialogueNode> dialogueNodes)
    {
        _dialogueNodes = dialogueNodes;
    }

    /// <summary>
    /// 创建对话播放器
    /// </summary>
    /// <param name="dialogueNodes"> 对话节点列表 </param>
    /// <returns> DialoguePlayer </returns>
    public static DialoguePlayer CreatePlayer(List<DialogueNode> dialogueNodes)
    {
        return new DialoguePlayer(dialogueNodes);
    }

    /// <summary>
    /// 添加一个对话播放的回调
    /// </summary>
    /// <param name="onPlayCallback"> 回调 </param>
    /// <returns> DialoguePlayer </returns>
    public DialoguePlayer OnPlay(Action onPlayCallback)
    {
        OnDialoguePlay += onPlayCallback;
        return this;
    }

    /// <summary>
    /// 添加一个对话结束的回调
    /// </summary>
    /// <param name="onEndCallback"> 回调 </param>
    /// <returns> DialoguePlayer </returns>
    public DialoguePlayer OnEnd(Action onEndCallback)
    {
        OnDialogueEnd += onEndCallback;
        return this;
    }
    #endregion

    #region API
    /// <summary>
    /// 开始播放对话
    /// </summary>
    /// <returns> UniTask </returns>
    public async UniTask StartDialogue()
    {
        OnDialoguePlay?.Invoke();
        OnDialoguePlay = null;

        _currentNode = _dialogueNodes[0]; // 默认从第一个节点开始播放

        var model = new DialogueUIModel()
        {
            Text = "",
            NextBtnData = new ButtonDataTemplate("Next", null, true, UpdateDialogue),
        };
        await UIManager.Instance.OpenReactive<DialogueUI, DialogueUIModel>(model);
    }

    /// <summary>
    /// 更新对话内容
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> 节点类型不支持 </exception>
    public void UpdateDialogue()
    {
        _currentNode.IsPlayed = true;
        _currentNode = _dialogueNodes.Find(node => node.NodeId == _currentNode.NextNodeId);

        switch (_currentNode.NodeAction)
        {
            case NodeAction.Choice:
                // TODO: 处理选择选项
                break;
            case NodeAction.Input:
                // TODO: 处理输入选项
                break;
            case NodeAction.Branch:
                // TODO: 处理分支选项
                break;
            case NodeAction.End:
                _ = DialogueManager.Instance.StopDialogue();
                break;
            case NodeAction.None:
            case NodeAction.Chat:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var model = new DialogueUIModel()
        {
            Text = _currentNode.Text,
            NextBtnData = new ButtonDataTemplate("Next", null, true, UpdateDialogue),
        };
        UIManager.Instance.LoadDataReactive<DialogueUI, DialogueUIModel>(model);
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    /// <returns> UniTask </returns>
    public async UniTask EndDialogue()
    {
        await UIManager.Instance.CloseReactive<DialogueUI>();

        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }
    #endregion
}
