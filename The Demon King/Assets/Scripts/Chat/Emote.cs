using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Emote/New Emote")]
public class Emote : ScriptableObject
{
    public int Reference;
    public Sprite emoteImage;
}
