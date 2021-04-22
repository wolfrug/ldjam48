using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

[System.Serializable]
public class InkUIIcon {
    public string InkVariable;
    public Sprite icon;
    [Tooltip("Leave empty to use the number on the InkVariable, or fill in to use this text instead.")]

    [Header("Text to be used together with the InkVariable number. Format: {Your text. {0}} where {0} -> the number")]
    public string textFormat = "{0}";
}

[CreateAssetMenu (fileName = "Data", menuName = "Ink UI Icon Data", order = 1)]
public class InkUIElementIconData : ScriptableObject {
    public InkUIIcon[] icons;
}