using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;

public class Worksheet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{ 
    [SerializeField] private BoxCollider _moveArea;

    private List<GameObject> _constructs = new();
    [SerializeField] private Transform _layout;

    [SerializeField] private float _speed;

    [SerializeField] private List<char> _randomDefectsChars = new();

    private List<List<GameObject>> _resourcesConstructs = new();
    [SerializeField] private int _constructsTypesCount = 0;
    private bool _haveDeffects = false;

    public bool Inizialize(WorksheetData data, int defectCount)
    {
        if (data == null)
        {
            Debug.LogWarning("[Worksheet] Worksheet trying to inizialize with worksheet data, that equals null");
            return false;
        }

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

        _haveDeffects = defectCount > 0;

        List<int> defects = new();
        for (int i = 0; i < _resourcesConstructs.Count; i++)
            defects.Add(0);

        ListRandomer.SplitInList(defects, defectCount);

        gameObject.SetActive(true);

        for(int i = 0; i < _resourcesConstructs.Count; i++)
        {
            int j = UnityEngine.Random.Range(0, _resourcesConstructs[i].Count);
            _constructs.Add(Instantiate(_resourcesConstructs[i][j], _layout));
        }

        for(int i = 0; i < _constructs.Count; i++)
        {
            GenerateDefects(_constructs[i], defects[i]);
        }

        return true;
    }

    private void GenerateDefects(GameObject target, int count)
    {
        if(target.TryGetComponent(out TMP_Text tmp_text))
        {
            tmp_text.text = MakeDefectInText(tmp_text.text, count);
        }
    }

    private string MakeDefectInText(string text, int count)
    {
        if(text.Length == 0 || count == 0)
            return text;

        char[] chars = text.ToCharArray();
        int[] shakedIndexes = new int[chars.Length];

        for(int i = 0; i < shakedIndexes.Length ;i++)
        {
            shakedIndexes[i] = i;
        }

        for (int i = 0; i < shakedIndexes.Length; i++)
        {
            int j = UnityEngine.Random.Range(i, shakedIndexes.Length);
            (shakedIndexes[i], shakedIndexes[j]) = (shakedIndexes[j], shakedIndexes[i]);
        }

        for (int i = 0; i < count && i < chars.Length; i++)
        {
            int j = UnityEngine.Random.Range(0, _randomDefectsChars.Count);
            chars[shakedIndexes[i]] = _randomDefectsChars[j];
        }

        return new(chars);
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

    private void Move(Vector3 offest)
    {
        if (_moveArea == null)
        {
            Debug.LogWarning("[Worksheet] Worksheet move area not defined");
            return;
        }

        Vector3 areaPosition = _moveArea.transform.position;

        Vector3 halfSize = _moveArea.size * 0.5f;

        Vector3 newPosition = transform.position + offest;

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

        Vector3 point = CameraController.Instance.RayCastPoint;
        Vector3 offest = _speed * Time.deltaTime * new Vector3(eventData.delta.x, 0, eventData.delta.y);

        Move(offest);
    }

}

