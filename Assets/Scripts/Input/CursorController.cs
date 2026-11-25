using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
    public Vector2 hotspot = Vector2.zero;

    public List<Texture2D> cursors = new();

    private void Awake()
    {
        ExecuteEvents.GloabalEvent += OnGlobalEvent;
    }

    private void OnDestroy()
    {
        ExecuteEvents.GloabalEvent -= OnGlobalEvent;
    }
    private void OnGlobalEvent(GameObject target, Type type)
    {
        //Debug.Log($"[CursorController] Current event type: '{type}'");

        if (target == null)
        {
            SetCursor("Default");
            return;
        }

        if (!target.activeInHierarchy)
        {
            SetCursor("Default");
            return;
        }

        if (type == typeof(IPointerEnterHandler) || type == typeof(IPointerDownHandler) || type == typeof(IPointerUpHandler))
        {
            SetCursor("PointerEnter");
        }
        else if (type == typeof(IBeginDragHandler) || type == typeof(IDragHandler))
        {
            SetCursor("Drag");
        }
        else
        {
            SetCursor("Default");
        }
    }

    public void SetCursor(string cursorName) => Cursor.SetCursor(cursors.Find((x) => x.name == cursorName), hotspot, CursorMode.Auto);
}