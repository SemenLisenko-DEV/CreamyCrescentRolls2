using CreamyCrescentRolls.RoomGame.WorksheetVars;
using System.Collections.Generic;
using UnityEngine;

public class WorksheetWatermark : MonoBehaviour, IWorksheetConstruct
{
    public void MakeDefect(int count, List<GameObject> constructs)
    {
        if (count == 0)
            return;

        gameObject.SetActive(false);
    }
}
