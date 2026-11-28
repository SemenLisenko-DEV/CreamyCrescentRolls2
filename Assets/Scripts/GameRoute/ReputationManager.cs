using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReputationManager : MonoBehaviour
{
    [Serializable]
    public class Final
    {
        public bool allCorect = false;
        public bool allWrong = false;
        public bool allClues = false;
        public bool anyClues = false;
        public bool sendedData = false;

        public UnityEvent onFinal;
    }

    private int _correctCount = 0;
    private int _wrongCount = 0;
    private int _clueCount = 0;
    [SerializeField] private int _maxClueCount = 0;
    private bool _sendedData = false;

    [SerializeField] private List<Final> _finals = new();

    public void AddDayStats(DayRoute.DayStats dayStats)
    {
        _correctCount += dayStats.correctCount;
        _wrongCount += dayStats.answeredCount - dayStats.correctCount;
    }

    public void AddClue()
    {
        _clueCount++;
    }

    public void AddData()
    {
        _sendedData = true;
    }

    public void ActivateFinal()
    {
        bool allCorrect = _correctCount > 0 && _wrongCount == 0;
        bool allWrong = _correctCount == 0 && _wrongCount > 0;
        bool allClues = _clueCount == _maxClueCount;
        bool anyClues = _clueCount > 0 && _clueCount < _maxClueCount;

        foreach (Final final in _finals)
        {
            if (final.allCorect == allCorrect &&
                final.allWrong == allWrong &&
                final.allClues == allClues &&
                final.anyClues == anyClues &&
                final.sendedData == _sendedData)
            {
                final.onFinal.Invoke();
                return;
            }


        }

        Debug.LogError($"[ReputationManager] No final found for the scenario:\n allCorrect:{allCorrect}\n  allWrong:{allWrong}\n allClues:{allClues}\n anyClues:{anyClues}\n");
    }
}
