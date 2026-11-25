using System.Collections.Generic;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.RandomAlgorithm
{
    public class ListRandomer
    {
        public static List<int> SplitInList(int m, int n)
        {
            List<int> resultList = new();

            for (int i = 0; i < m; i++)
                resultList.Add(0);
            //Debug.Log($"[ListRandomer] Spliting in list with lenght: '{targetList.Count}' n: '{n}'");

            for (int i = 0; i < n; i++)
                resultList[UnityEngine.Random.Range(0, m)]++;

            return resultList;
        }

        public static void Shuffle(List<int> target)
        {
            for (int i = target.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (target[i], target[j]) = (target[j], target[i]);
            }
        }
    }
}
