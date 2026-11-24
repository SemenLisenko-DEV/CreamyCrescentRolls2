using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using CreamyCrescentRolls.RoomGameWorksheetVars;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Worksheet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{ 
    [SerializeField] private BoxCollider _moveArea;

    private List<GameObject> _constructs = new();
    [SerializeField] private Transform _layout;

    //[SerializeField] private float _speed;

    private List<List<GameObject>> _resourcesConstructs = new();
    [SerializeField] private int _constructsTypesCount = 0;
    private bool _haveDeffects = false;

    //public GameObject help;

    public void LoadConstucts()
    {
        if (_resourcesConstructs == null || _resourcesConstructs?.Count == 0)
        {
            _resourcesConstructs = new();

            string log = "";
            for (int i = 0; i < _constructsTypesCount; i++)
            {
                _resourcesConstructs.Add(Resources.LoadAll<GameObject>("Worksheet/WorksheetConstucts/" + (i + 1).ToString()).ToList());
                log += $"Type {i + 1} loaded with total count of {_resourcesConstructs[i].Count} and path: Worksheet/WorksheetConstucts/{i + 1} \n";
            }

            Debug.Log($"[Worksheet] Worksheet loaded {_resourcesConstructs.Count} constructs types of constructs: \n" + log);
        }
    }

    public bool Inizialize(int defectCount, List<bool> allowedConstructs)
    {
        if (allowedConstructs == null)
        {
            Debug.LogWarning("[Worksheet] Worksheet trying to inizialize with allowed constructs list, that equals null");
            return false;
        }

        gameObject.SetActive(true);

        for(int i = 0; i < _resourcesConstructs.Count && i < allowedConstructs.Count; i++)
        {
            if (!allowedConstructs[i])
                continue;

            int j = UnityEngine.Random.Range(0, _resourcesConstructs[i].Count);
            _constructs.Add(Instantiate(_resourcesConstructs[i][j], _layout));
        }

        _haveDeffects = defectCount > 0;

        List<int> defects = ListRandomer.SplitInList(_constructs.Count, defectCount);

        for (int i = 0; i < _constructs.Count; i++)
            GenerateDefects(_constructs[i], defects[i]);

        return true;
    }

    private void GenerateDefects(GameObject target, int count)
    {
        if(!target.TryGetComponent(out IWorksheetConstruct worksheetConstruct))
        {
            Debug.Log("[Worksheet] Target object dosent contains IWorksheetConstruct to make defects in it");
            return;
        }

        worksheetConstruct.MakeDefect(count);
    }

    public bool Finish()
    {
        gameObject.SetActive(false);

        foreach (GameObject constuct in _constructs)
        {
            if (constuct == null)
                continue;

            Destroy(constuct);
        }

        _constructs.Clear();

        return _haveDeffects;
    }

    private void Move(Vector3 newPosition)
    {
        if (_moveArea == null)
        {
            Debug.LogWarning("[Worksheet] Worksheet move area not defined");
            return;
        }

        Vector3 areaPosition = _moveArea.transform.position;

        Vector3 halfSize = _moveArea.size * 0.5f;

        newPosition.x = Mathf.Clamp(newPosition.x, areaPosition.x - halfSize.x, areaPosition.x + halfSize.x);
        newPosition.y = Mathf.Clamp(newPosition.y, areaPosition.y - halfSize.y, areaPosition.y + halfSize.y);
        newPosition.z = Mathf.Clamp(newPosition.z, areaPosition.z - halfSize.z, areaPosition.z + halfSize.z);

        transform.position = newPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector3 point = CameraController.Instance.MouseRayCastPoint;
        //Vector3 offest = _speed * Time.deltaTime * new Vector3(eventData.delta.x, 0, eventData.delta.y);

        Move(point);

        //help.transform.position = point;
    }

}

