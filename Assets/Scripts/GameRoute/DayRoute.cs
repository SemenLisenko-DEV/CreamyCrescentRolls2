using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DayRoute : MonoBehaviour
{
    [SerializeField] private UnityEvent<DayRoute> _onDayStart;
    [SerializeField] private UnityEvent<DayStats> _onDayEnd;
    [SerializeField] private UnityEvent _onTryEndDay;
    [SerializeField] private UnityEvent _onFailedDay;
    [SerializeField] private UnityEvent _onLastWorksheet;
    [SerializeField] private TMP_Text _timerDisplay;

    [Serializable]
    public class DayActivity
    {
        public float startDelay = 1f;
        public int deffectsCount = 1;
        public int worksheetCount = 5;
        public float dayTime = 60f * 10;
        public UnityEvent onStartActivity;
        public UnityEvent onEndActivity;
    }

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
        if (currentActivitiy + 1 == _activities.Count)
        {
            Debug.Log("[DayRoute] Routes ended");
            return;
        }

        if (currentDay.routeActive)
        {
            Debug.Log("[DayRoute] Route already started");
            return;
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
        _onDayStart.Invoke(this);
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

    public void AddToCorrect()
    {
        if(!currentDay.routeActive)
        {
            Debug.LogWarning("[DayRoute] Day route is not started/alredy ended");
            return;
        }

        currentDay.correctCount++;
    }

    public void MakeAnswer()
    {
        currentDay.answeredCount++;

        if (currentDay.answeredCount >= GetCurrentActivity().worksheetCount)
        {
            _onLastWorksheet.Invoke();
        }
    }

    public void TryEndDay()
    {
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

        _onDayEnd.Invoke(currentDay);
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
