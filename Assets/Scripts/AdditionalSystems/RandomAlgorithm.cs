using System.Collections.Generic;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.RandomAlgorithm
{
    public class ListRandomer
    {
        public static void SplitInList(List<int> targetList, int n)
        {
            if (targetList == null)
            {
                Debug.LogError($"[ListRandomer] Target list equals null");
                return;
            }

            Debug.Log($"[ListRandomer] Spliting in list with lenght: '{targetList.Count}' n: '{n}'");

            for (int i = 0; i < n; i++)
                targetList[UnityEngine.Random.Range(0, targetList.Count)]++;
        }
    }
}
