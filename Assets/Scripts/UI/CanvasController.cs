using CreamyCrescentRolls.RoomGame.Conditions;
using CreamyCrescentRolls.RoomGame.GameCursor;
using CreamyCrescentRolls.RoomGame.GameTime;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        if (changeBlockConditions.GetState())
        {
            Debug.Log("[CanvasController] Canvas change is blocked");
            return;
        }

        if (ActiveCanvas == name)
        {
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
            if(c.name == name)
            {
                SetCanvas(c);
                return;
            }    
        }

        Debug.LogError($"[CanvasController] Canvas with name: {name} not found");
        throw new System.Exception();
    }

    private void SetCanvas(GameObject canvas)
    {
        CursorManager.AddVisibleHandler(gameObject);
        TimeManager.AddTimeStopper(gameObject);
        canvas.SetActive(true);
    }

    public void DisableAllCanvases()
    {
        if (changeBlockConditions.GetState())
        {
            Debug.Log("[CanvasController] Canvas change is blocked");
            return;
        }

        foreach (var canvas in canvases)
        {
            canvas.SetActive(false);
        }

        CursorManager.RemoveVisibleHandler(gameObject);
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
}
