using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private UISoundMeta _soundMeta;

    private EventTrigger _trigger;

    private void Start()
    {
        // 如果没有EventTrigger组件，则添加一个
        _trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        if (_soundMeta == null)
        {
            Debug.LogWarning($"[ButtonSound] No sound meta data found for {gameObject.name}.");
            return;
        }

        AddEventTrigger(EventTriggerType.PointerClick, _soundMeta.ClickSound);
        AddEventTrigger(EventTriggerType.PointerEnter, _soundMeta.HoverSound);
    }

    /// <summary>
    /// Add an event trigger to the button.
    /// </summary>
    /// <param name="eventType"> The type of event to trigger. </param>
    /// <param name="sound"> The sound to play. </param>
    private void AddEventTrigger(EventTriggerType eventType, AudioClip sound)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(_ =>
            SoundManager.Instance
                .PlaySound(_soundMeta.SoundType, sound, false, _soundMeta.DefaultVolume));
        _trigger.triggers.Add(entry);
    }
}
