using CreamyCrescentRolls.RoomGame.Conditions;
using CreamyCrescentRolls.RoomGame.GameTime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasController : MonoBehaviour
{
    public const string noneActiveCanvas = "NONE";

    [Serializable]
    public struct RederectPair
    {
        public string activeCanvas;
        public string rederectCanvas;
    }

    public List<RederectPair> rederectPairs = new();

    public ConditionsHolder changeBlockConditions = new(ConditonCheckType.Any);

    public List<GameObject> canvases;

    public string defaultCanvas = string.Empty;

    [SerializeField] private UnityEvent _onActivateCanvas;
    [SerializeField] private UnityEvent _onDisableCanvases;

    public string ActiveCanvas
    {
        get
        {
            foreach (var c in canvases)
            {
                if (c.activeSelf)
                    return c.name;
            }

            return noneActiveCanvas;
        }
    }

    public void SetActiveCanvas(string name)
    {
        SetActiveCanvas(name, false);
    }

    public void SetActiveCanvas(string name, bool ignoreBlock)
    {
        if (changeBlockConditions.GetState() && !ignoreBlock)
        {
            Debug.Log("[CanvasController] Canvas change is blocked");
            return;
        }

        if (ActiveCanvas == name)
        {
            if (ActiveCanvas == defaultCanvas)
                return;

            DisableAllCanvases();
            return;
        }

        DisableAllCanvases();

        string rederectPair = GetRederectPair(ActiveCanvas);
        if (rederectPair != noneActiveCanvas)
        {
            SetActiveCanvas(rederectPair);
            return;
        }

        foreach (var c in canvases)
        {
            if (c.name == name)
            {
                SetCanvas(c);
                return;
            }
        }

        Debug.LogError($"[CanvasController] Canvas with name: {name} not found");
    }

    private void SetCanvas(GameObject canvas)
    {
        TimeManager.AddTimeStopper(gameObject);
        _onActivateCanvas.Invoke();
        canvas.SetActive(true);
    }

    public void DisableAllCanvases()
    {
        _onDisableCanvases.Invoke();

        if (changeBlockConditions.GetState())
        {
            Debug.Log("[CanvasController] Canvas change is blocked");
            return;
        }

        foreach (var canvas in canvases)
        {
            canvas.SetActive(false);
        }

        GameObject defaultCanvasObj = canvases.Find((x) => x.name == defaultCanvas);

        TimeManager.RemoveTimeStopper(gameObject);
    }

    public string GetRederectPair(string activeCanvas)
    {
        foreach(var pair in rederectPairs)
        {
            if (activeCanvas == pair.activeCanvas)
                return pair.rederectCanvas;
        }

        return noneActiveCanvas;
    }

    public void BlockChange()
    {
        changeBlockConditions.AddListener(() => true, "ALWAYSBLOCK");
    }

    public void UnBlockChange()
    {
        changeBlockConditions.DeleteListener("ALWAYSBLOCK");
    }

    public void SetWithDelay(string canvas_delay)
    {
        string canvas = canvas_delay.Split('_')[0];
        float delay = float.Parse(canvas_delay.Split('_')[1]);

        StartCoroutine(SetWithDelay_C(canvas, delay));
    }

    private IEnumerator SetWithDelay_C(string canvas, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        SetActiveCanvas(canvas, true);
    }

    private void OnDestroy()
    {
        TimeManager.RemoveTimeStopper(gameObject);
    }
}
