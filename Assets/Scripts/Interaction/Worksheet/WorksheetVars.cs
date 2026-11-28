using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CreamyCrescentRolls.RoomGame.WorksheetVars
{
    public interface IWorksheetConstruct
    {
        public void MakeDefect(int count, List<GameObject> othersConstructs);
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

        public bool Protect()
        {
            bool haveDefects = false;

            if (startDelay < 0)
            {
                haveDefects = true;
                startDelay = 0;
            }

            if (deffectsCount < 0)
            {
                haveDefects = true;
                deffectsCount = 0;
            }

            if (worksheetCount <= 0)
            {
                haveDefects = true;
                worksheetCount = 1;
            }

            if (dayTime < 0)
            {
                haveDefects = true;
                dayTime = 0;
            }

            return haveDefects;
        }
    }

}
