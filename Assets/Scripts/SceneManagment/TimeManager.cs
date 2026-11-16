using System.Collections.Generic;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.GameTime
{
    public class TimeManager
    {
        private static List<GameObject> _stoppers = new();

        public static void AddTimeStopper(GameObject gameObject)
        {
            RemoveAllFakeNulls();

            if (_stoppers.Contains(gameObject))
            {
                Debug.LogWarning($"[TimeManager] Gameobject {gameObject.name} already in time stoppers list");
                return;
            }

            _stoppers.Add(gameObject);

            CheckTimeStoppers();
        }

        public static void RemoveTimeStopper(GameObject gameObject)
        {
            RemoveAllFakeNulls();

            if (!_stoppers.Contains(gameObject))
            {
                Debug.LogWarning($"[TimeManager] Gameobject {gameObject.name} not in time stoppers list");
                return;
            }

            _stoppers.Remove(gameObject);

            CheckTimeStoppers();
        }

        private static void RemoveAllFakeNulls()
        {
            for (int i = 0; i < _stoppers.Count; i++)
            {
                if(_stoppers[i] == null)
                {
                    _stoppers.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void CheckTimeStoppers()
        {
            if (_stoppers.Count > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}