using System.Collections.Generic;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.Conditions
{
    public enum ConditonCheckType
    {
        None = 0,
        All = 1,
        Any = 2
    }

    public delegate bool Conditon();

    public class ConditionsHolder
    {
        public ConditionsHolder()
        {
        }

        public ConditionsHolder(ConditonCheckType checkType)
        {
            this.checkType = checkType;
        }

        public ConditonCheckType checkType;

        private Dictionary<string, Conditon> _conditons = new();

        public bool GetState(bool logs = false)
        {
            int count = 0;

            foreach (var conditon in _conditons.Values)
            {
                count += conditon.Invoke() ? 1 : 0;
            }

            if (logs)
                Debug.Log($"[ConditionsHolder] Count of true conditions: {count}");

            switch(checkType)
            {
                case ConditonCheckType.None:
                    return count == 0;

                case ConditonCheckType.All:
                    return count == _conditons.Count;

                case ConditonCheckType.Any:
                    return count != 0;

            }

            Debug.LogError("[ConditionsHolder] GetState walked past checkType");
            throw new System.Exception();
        }

        public void AddListener(Conditon conditon, string id)
        {
            if(_conditons.ContainsKey(id))
            {
                Debug.LogWarning($"[ConditionsHolder] ConditionsHolder already contains condition with id: {id}");
                return;
            }

            _conditons.Add(id, conditon);
        }

        public void DeleteListener(string id)
        {
            if (!_conditons.ContainsKey(id))
            {
                Debug.LogWarning($"[ConditionsHolder] ConditionsHolder dosent contains condition with id: {id}");
                return;
            }

            _conditons.Remove(id);
        }
    }
}