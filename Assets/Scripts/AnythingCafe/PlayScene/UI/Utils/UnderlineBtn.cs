using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnderlineBtn : MonoBehaviour
{
    [SerializeField]
    private Button _btn;

    private EventTrigger _trigger;
    private TextMeshProUGUI _text;

    private void Start()
    {
        _btn ??= GetComponent<Button>();
        _trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        AddEventTrigger(EventTriggerType.PointerEnter, UnderlineText);
        AddEventTrigger(EventTriggerType.PointerExit, RemoveUnderline);
    }

    /// <summary>
    /// Add an event trigger to the button.
    /// </summary>
    /// <param name="eventType"> The type of event to trigger. </param>
    /// <param name="action"> The action to perform on event trigger. </param>
    private void AddEventTrigger(EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        _trigger.triggers.Add(entry);
    }


    private void UnderlineText(BaseEventData eventData)
    {
        _text.fontStyle = FontStyles.Italic | FontStyles.Underline;
    }

    private void RemoveUnderline(BaseEventData eventData)
    {
        _text.fontStyle = FontStyles.Italic;
    }
}
