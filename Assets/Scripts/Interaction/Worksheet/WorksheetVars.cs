using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CreamyCrescentRolls.RoomGameWorksheetVars
{
    public interface IWorksheetConstruct
    {
        public void MakeDefect(int count);
    }

    [Serializable]
    public class DayActivity
    {
        public float startDelay = 1f;
        public int deffectsCount = 1;
        public int worksheetCount = 5;
        public float dayTime = 60f * 10;
        public List<bool> allowedConstructs = new();
        public UnityEvent onStartActivity;
        public UnityEvent onEndActivity;

        [HideInInspector] public List<int> defects = new();
        [HideInInspector] public int currentWorksheet = new();
    }

}
