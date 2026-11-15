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
        public ConditonCheckType checkType;

        private Dictionary<string, Conditon> conditons;

        public bool GetState()
        {
            int count = 0;

            foreach (var conditon in conditons.Values)
            {
                count += conditon.Invoke() ? 1 : 0;
            }

            switch(checkType)
            {
                case ConditonCheckType.None:
                    return count == 0;

                case ConditonCheckType.All:
                    return count == conditons.Count;

                case ConditonCheckType.Any:
                    return count != 0;

            }

            Debug.LogError("[ConditionsHolder] GetState walked past checkType");
            throw new System.Exception();
        }

        public void AddListener(Conditon conditon, string id)
        {
            if(conditons.ContainsKey(id))
            {
                Debug.LogWarning("[ConditionsHolder] ConditionsHolder already contains condition with id: " + id);
                return;
            }    

            conditons.Add(id, conditon);
        }

        public void DeleteListener(string id)
        {
            if (!conditons.ContainsKey(id))
            {
                Debug.LogWarning("[ConditionsHolder] ConditionsHolder dosent contains condition with id: " + id);
                return;
            }

            conditons.Remove(id);
        }
    }
}