using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayRoute : MonoBehaviour
{
    [SerializeField] private UnityEvent<DayRoute> _onDayStart;
    [SerializeField] private UnityEvent _onDayEnd;

    [Serializable]
    public class DayActivity
    {
        public float startDelay = 1f;
        public int deffectsCount = 1;
        public int worksheetCount = 5;
        public float dayTime = 60f * 10;
    }

    [SerializeField] private List<DayActivity> _activities = new();

    public class DayStats
    {
        public int correctCount = 0;
        public float dayTimer = 0;
        public bool routeActive = false;
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

        currentDay.dayTimer = GetCurrentActivity().dayTime;
        _onDayStart.Invoke(this);
        currentDay.routeActive = true;

        yield return new WaitForSeconds(GetCurrentActivity().startDelay);

        while(currentDay.dayTimer > 0)
        {
            currentDay.dayTimer -= Time.deltaTime;
            yield return null;
        }

        currentDay.routeActive = false;
        _onDayEnd.Invoke();
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
            Debug.LogError("[DayRoute] Day route is not started/alredy ended");
            return;
        }

        currentDay.correctCount++;
    }
}
