using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorksheetData", menuName = "Worksheet/Document")]
public class WorksheetData : ScriptableObject
{
    [Header("Gameplay changer")] public string type;
    [Min(0f)] public float weight = 1f;
}
