using CreamyCrescentRolls.RoomGame.WorksheetVars;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorksheetFolder : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
{
    [SerializeField] private Worksheet _worksheet;

    [SerializeField] private UnityEvent _onRightChoice;
    [SerializeField] private UnityEvent _onWrongChoice;

    private DayActivity _dayActivity;

    public void Inizialize(DayActivity dayActivity)
    {
        if (dayActivity == null)
        {
            Debug.LogError("[WorksheetFolder] Day activity is null");
            return;
        }

        _dayActivity = dayActivity;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            Debug.Log($"[WorksheetFolder] PointerEventData.InputButton: {eventData.button}");
            return;
        }

        if (_dayActivity == null)
        {
            Debug.LogError("[WorksheetFolder] Day activity is null");
            return;
        }

        if (_worksheet == null)
        {
            Debug.LogError("[WorksheetFolder] Worksheet object not defined");
            return;
        }

        if(_dayActivity.currentWorksheet >= _dayActivity.defects.Count)
        {
            Debug.LogWarning("[WorksheetFolder] Distributed defects are out of count");
            return;
        }

        bool isInizialized = _worksheet.Inizialize(_dayActivity.defects[_dayActivity.currentWorksheet], _dayActivity.allowedConstructs);

        if (!isInizialized)
        {
            Debug.LogWarning("[WorksheetFolder] Worksheet object failed inizialize");
            return;
        }

        eventData.pointerDrag = _worksheet.gameObject;
        eventData.eligibleForClick = false;
        eventData.useDragThreshold = false;

        ExecuteEvents.Execute(_worksheet.gameObject, eventData, ExecuteEvents.beginDragHandler);

        eventData.dragging = true;

        ExecuteEvents.Execute(_worksheet.gameObject, eventData, ExecuteEvents.dragHandler);

        _dayActivity.currentWorksheet++;
    }

    public void SubmitWorksheet(Collider collider)
    {
        if(collider == null)
        {
            Debug.LogWarning("[WorksheetFolder] Submit collider equals null/fakeNull");
            return;
        }

        if (!collider.TryGetComponent(out Worksheet worksheet))
        {
            Debug.LogWarning("[WorksheetFolder] Submit collider hasnt worksheet component");
            return;
        }

        if (!worksheet.Equals(_worksheet))
        {
            Debug.LogError($"[WorksheetFolder] Unexpected worksheet object, id: '{worksheet.GetInstanceID()}' and name: '{worksheet.name}'");
            return;
        }

        if (!_worksheet.Finish())
            _onRightChoice.Invoke();
        else
            _onWrongChoice.Invoke();
    }

    public void RejectWorksheet(Collider collider)
    {
        if (collider == null)
        {
            Debug.LogWarning("[WorksheetFolder] Reject collider equals null/fakeNull");
            return;
        }

        if (!collider.TryGetComponent(out Worksheet worksheet))
        {
            Debug.LogWarning("[WorksheetFolder] Reject collider hasnt worksheet component");
            return;
        }

        if (!worksheet.Equals(_worksheet))
        {
            Debug.LogError($"[WorksheetFolder] Unexpected worksheet object, id: '{worksheet.GetInstanceID()}' and name: '{worksheet.name}'");
            return;
        }

        if (_worksheet.Finish())
            _onRightChoice.Invoke();
        else
            _onWrongChoice.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
