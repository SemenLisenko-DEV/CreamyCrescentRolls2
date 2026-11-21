using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestController : MonoBehaviour
{
    private List<string> _items = new();

    [SerializeField] private UnityEvent<string> onTryCompliteQuest;

    [Serializable]
    public class Quest
    {
        public string name;
        public List<string> requredItems = new();
        public UnityEvent onComplite;
    }

    [SerializeField] private List<Quest> _quests;

    public void TryComplite(string name)
    {
        Quest quest = _quests.Find((x) => x.name == name);

        if (quest == null)
        {
            Debug.LogWarning($"[QuestController] Quest with name '{name}' arent exists");
            return;
        }

        if(!CheckItems(quest, out List<string> missingItems))
        {
            string msg = "Недостаточно предметов, нужны: ";

            foreach (string item in missingItems)
            {
                msg += item + ", ";
            }

            msg = msg[..^2]; // .. - range operator

            onTryCompliteQuest.Invoke(msg);
            Debug.Log($"[QuestController] Quest with name '{name}' cant be complited. Missing items lenght: {missingItems.Count}");
            return;
        }

        quest.onComplite.Invoke();
    }

    public bool CheckItems(Quest quest, out List<string> missingItems)
    {
        missingItems = new();

        foreach (var requredItem in quest.requredItems)
        {
            if (_items.Contains(requredItem))
                continue;

            missingItems.Add(requredItem);
        }

        if (missingItems.Count > 0)
        {
            return false;
        }

        return true;
    }

    public void AddItem(string name)
    {
        if (_items.Contains(name))
        {
            Debug.LogWarning($"[QuestController] Item '{name}' already in item list");
            return;
        }

        _items.Add(name);
    }
}
