using CreamyCrescentRolls.RoomGame.RandomAlgorithm;
using CreamyCrescentRolls.RoomGame.WorksheetVars;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worksheet : MonoBehaviour
{ 
    private List<GameObject> _constructs = new();
    [SerializeField] private Transform _layout;

    private List<List<GameObject>> _resourcesConstructs = new();
    [SerializeField] private int _constructsTypesCount = 0;
    private bool _haveDeffects = false;

    public bool inProcess = false;

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

        if (inProcess)
        {
            Debug.LogWarning("[Worksheet] Worksheet already in process!");
            return false;
        }

        inProcess = true;
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

        worksheetConstruct.MakeDefect(count, _constructs);
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

        inProcess = false;
        _constructs.Clear();

        return _haveDeffects;
    }
}

