using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using CreamyCrescentRolls.RoomGame.WorksheetVars;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DayRoute : MonoBehaviour
{
    [SerializeField] private UnityEvent<DayActivity> _onDayStart;
    [SerializeField] private UnityEvent<DayStats> _onDayEnd;
    [SerializeField] private UnityEvent _onTryEndDay;
    [SerializeField] private UnityEvent _onFailedDay;
    [SerializeField] private UnityEvent _onLastWorksheet;
    [SerializeField] private UnityEvent<DayStats> _onLastActivity;
    [SerializeField] private TMP_Text _timerDisplay;

    [SerializeField] private List<DayActivity> _activities = new();

    public class DayStats
    {
        public int correctCount = 0;
        public float dayTimer = 0;
        public bool routeActive = false;
        public int answeredCount = 0;
    }

    public int currentActivitiy = -1;

    public DayStats currentDay = new();

    public void TryStartNewRoute()
    {
        if (currentActivitiy + 1 >= _activities.Count)
        {
            Debug.Log("[DayRoute] Routes ended");
            return;
        }

        if (currentDay.routeActive)
        {
            Debug.Log("[DayRoute] Route already started");
            return;
        }

        if (_activities[currentActivitiy + 1].Protect())
        {
            Debug.LogError("[DayRoute] Day activity has broken fields");
        }

        StartDayRoute();
    }

    private void StartDayRoute() => StartCoroutine(DayRoute_C());

    private IEnumerator DayRoute_C()
    {
        currentDay = new();

        currentActivitiy++;

        if(GetCurrentActivity() == null)
        {
            Debug.LogError("[DayRoute] Day route failed to start");
            yield break;
        }

        GetCurrentActivity().onStartActivity.Invoke();
        currentDay.dayTimer = GetCurrentActivity().dayTime;

        GetCurrentActivity().defects = ListRandomer.SplitInList(GetCurrentActivity().worksheetCount, GetCurrentActivity().deffectsCount);
        _onDayStart.Invoke(GetCurrentActivity());

        currentDay.routeActive = true;
        yield return new WaitForSeconds(GetCurrentActivity().startDelay);

        while(currentDay.dayTimer > 0)
        {
            currentDay.dayTimer -= Time.deltaTime;

            if(_timerDisplay != null)
                _timerDisplay.text = $"До конца дня: {(int)currentDay.dayTimer}";

            yield return null;
        }

        TryEndDay();
    }

    public DayActivity GetCurrentActivity()
    {
        if(currentActivitiy < 0 || currentActivitiy >= _activities.Count)
        {
            Debug.LogError($"[DayRoute] Current activity index '{currentActivitiy}' is out of activities range '{_activities.Count}'");
            return null;
        }

        return _activities[currentActivitiy];
    }

    public void MakeAnswer(bool isRight)
    {
        if (!currentDay.routeActive)
        {
            Debug.LogWarning("[DayRoute] Day route is not started/alredy ended");
            return;
        }

        currentDay.correctCount += isRight ? 1 : 0;

        currentDay.answeredCount++;

        if (currentDay.answeredCount >= GetCurrentActivity().worksheetCount)
        {
            _onLastWorksheet.Invoke();
        }
    }

    public void TryEndDay()
    {
        if (GetCurrentActivity() == null)
        {
            Debug.LogError("[DayRoute] Current day cant be ended, because havent inizialize current activity");
            _onTryEndDay.Invoke();
            return;
        }

        if(currentDay.dayTimer <= 0 && currentDay.routeActive && currentDay.answeredCount < GetCurrentActivity().worksheetCount)
        {
            Debug.Log("[DayRoute] Current day failed");
            _onFailedDay.Invoke();
            return;
        }

        if (currentDay.answeredCount < GetCurrentActivity().worksheetCount)
        {
            Debug.Log("[DayRoute] Current day cant be ended, because not all worksheet is checked");
            _onTryEndDay.Invoke();
            return;
        }

        StopAllCoroutines();

        GetCurrentActivity().onEndActivity.Invoke();

        currentDay.routeActive = false;

        if (_timerDisplay != null)
            _timerDisplay.text = "";

        if (currentActivitiy + 1 >= _activities.Count)
        {
            _onLastActivity.Invoke(currentDay);
        }
        else
        {
            _onDayEnd.Invoke(currentDay);
        }
    }

    [SerializeField] private TMP_Text _finalsTitleField;
    [SerializeField] private TMP_Text _correctCountField;
    [SerializeField] private TMP_Text _wrongCountField;

    public void FillDayFinals(DayStats dayStats)
    {
        _finalsTitleField.text = $"Финалы дня {currentActivitiy + 1}";
        _correctCountField.text = $"Правильно проверенно: {dayStats.correctCount}";
        _wrongCountField.text = $"Неверно проверенно: {GetCurrentActivity().worksheetCount - dayStats.correctCount}";
    }
}
