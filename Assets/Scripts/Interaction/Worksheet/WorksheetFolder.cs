using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorksheetFolder : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
{
    [SerializeField] private Worksheet _worksheet;

    [SerializeField] private UnityEvent _onRightChoice;

    private List<WorksheetData> _worksheetDatas = new();

    private class Route
    {
        public List<int> defects = new();
        public List<WorksheetData> choisedWorksheetDatas = new();
        public int worksheetCount = 0;
        public int worksheetIndex = 0;
    }

    private Route _route = new();

    public void Inizialize(DayRoute dayRoute)
    {
        if(dayRoute == null)
        {
            Debug.LogError("[WorksheetFolder] Day route is null/fakeNull");
            return;
        }

        if (dayRoute.GetCurrentActivity() == null)
        {
            Debug.LogError("[WorksheetFolder] Current day activity is null/fakeNull");
            return;
        }

        _route = new();

        _route.worksheetCount = dayRoute.GetCurrentActivity().worksheetCount;

        for (int i = 0; i < _route.worksheetCount; i++)
        {
            _route.choisedWorksheetDatas.Add(GetData());
        }

        for (int i = 0; i < _route.worksheetCount; i++)
            _route.defects.Add(0);

        ListRandomer.SplitInList(_route.defects, _route.worksheetCount);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            Debug.Log($"[WorksheetFolder] PointerEventData.InputButton: {eventData.button}");
            return;
        }

        if (_worksheet == null)
        {
            Debug.LogWarning("[WorksheetFolder] Worksheet object not defined");
            return;
        }

        if (_route.choisedWorksheetDatas.Count == 0)
        {
            Debug.LogWarning("[WorksheetFolder] Choised worksheets data count equals zero");
            return;
        }

        if (_worksheet.gameObject.activeSelf)
        {
            Debug.LogWarning("[WorksheetFolder] Worksheet object is active");
            return;
        }

        if (_route.choisedWorksheetDatas.Count <= _route.worksheetIndex)
        {
            Debug.LogWarning("[WorksheetFolder] Day route already ended, because havent any worksheet for work");
            return;
        }

        var choisedWorksheetData = _route.choisedWorksheetDatas[_route.worksheetIndex];
        if (choisedWorksheetData == null)
        {
            Debug.LogError("[WorksheetFolder] ChoisedWorksheetData equals null/fakeNull");
            return;
        }    

        if (_route.defects == null)
        {
            Debug.LogError("[WorksheetFolder] Deffects equals null");
            return;
        }

        bool isInizialized = _worksheet.Inizialize(choisedWorksheetData, _route.defects[_route.worksheetIndex]);

        if (!isInizialized)
        {
            Debug.LogWarning("[WorksheetFolder] Worksheet object failed inizialize");
            return;
        }

        _worksheet.transform.position = transform.position;

        eventData.pointerDrag = _worksheet.gameObject;
        eventData.eligibleForClick = false;
        eventData.useDragThreshold = false;

        ExecuteEvents.Execute(_worksheet.gameObject, eventData, ExecuteEvents.beginDragHandler);

        eventData.dragging = true;

        ExecuteEvents.Execute(_worksheet.gameObject, eventData, ExecuteEvents.dragHandler);

        _route.worksheetIndex++;
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
        {
            _onRightChoice.Invoke();
        }
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
        {
            _onRightChoice.Invoke();
        }
    }

    private WorksheetData GetData()
    {
        if (_worksheetDatas == null || _worksheetDatas?.Count == 0)
        {
            _worksheetDatas = Resources.LoadAll<WorksheetData>("Worksheet/WorksheetDatas").ToList();

            if (_worksheetDatas.Count == 0)
            {
                Debug.LogWarning("[WorksheetFolder] No WorksheetData assets were loaded from Resources/Worksheet/WorksheetDatas");
                return null;
            }
        }

        float sum = 0;

        foreach (var data in _worksheetDatas)
        {
            sum += data.weight;
        }

        sum *= UnityEngine.Random.value;
        WorksheetData last = null;

        foreach (var data in _worksheetDatas)
        {
            sum -= data.weight;
            last = data;

            if (sum <= 0)
                return data;
        }

        return last;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
