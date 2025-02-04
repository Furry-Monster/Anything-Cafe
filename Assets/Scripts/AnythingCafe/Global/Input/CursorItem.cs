using System;
using UnityEngine;
#pragma warning disable UNT0013

[CreateAssetMenu(fileName = "CursorItem", menuName = "AnythingCafe/Cursor/CursorItem")]
public class CursorItem : ScriptableObject
{
    [SerializeField]
    public Texture2D[] CursorTextures = Array.Empty<Texture2D>();
    [SerializeField]
    public Vector2 CursorOffset = Vector2.zero;
    [SerializeField]
    public float FrameRate; // 使用动态cursor时，图集中每帧的刷新率

    public int FrameCount => CursorTextures.Length;
}