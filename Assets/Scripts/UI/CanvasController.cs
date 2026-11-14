using CreamyCrescentRolls.RoomGame.AdditionalSystems;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public const string noneActiveCanvas = "NONE";

    public struct RederectPair
    {
        public string activeCanvas;
        public string rederectCanvas;
    }

    public List<RederectPair> rederectPairs = new();

    public ConditionsHolder changeBlockConditions = new();

    public List<GameObject> canvases;

    public string defaultCanvas = string.Empty;

    public string ActiveCanvas
    {
        get
        {
            foreach (var c in canvases)
            {
                if (c.gameObject.activeSelf)
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
            
        DisableAllCanvases();

        string rederectPair = GetRederectPair(ActiveCanvas);
        if (rederectPair != noneActiveCanvas)
        {
            SetActiveCanvas(rederectPair);
            return;  
        }

        if (ActiveCanvas == name)
            return;

        foreach (var c in canvases)
        {
            if(c.name == name)
            {
                c.SetActive(true);
                return;
            }    
        }

        Debug.LogError($"[CanvasController] Canvas with name: {name} not found");
        throw new System.Exception();
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
