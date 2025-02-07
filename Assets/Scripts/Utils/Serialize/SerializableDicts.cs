using System;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ButtonEventDictionary : SerializableDictionary<Button, UnityEvent> { }

[Serializable]
public class ToggleEventDictionary : SerializableDictionary<Toggle, UnityEvent<bool>> { }

[Serializable]
public class SliderEventDictionary : SerializableDictionary<Slider, UnityEvent<float>> { }

[Serializable]
public class InputFieldEventDictionary : SerializableDictionary<TMPro.TMP_InputField, UnityEvent<string>> { }