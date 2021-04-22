using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "Ink Character Object", order = 1)]
public class InkCharacterObject : ScriptableObject {
    public string characterName;
    public GameObject portraitPrefab;
    public string progressBarName = "Affection";
}