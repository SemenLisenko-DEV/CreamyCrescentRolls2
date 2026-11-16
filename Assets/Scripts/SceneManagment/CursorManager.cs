using System.Collections.Generic;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.GameCursor
{
    public class CursorManager
    {
        private static List<GameObject> _cursorVisibleHandlers = new();

        public static void AddVisibleHandler(GameObject gameObject)
        {
            RemoveAllFakeNulls();

            if (_cursorVisibleHandlers.Contains(gameObject))
            {
                Debug.LogWarning($"[CursorManager] Gameobject {gameObject.name} already in visible handlers list");
                return;
            }

            _cursorVisibleHandlers.Add(gameObject);

            CheckCursorVisibleHandlers();
        }

        public static void RemoveVisibleHandler(GameObject gameObject)
        {
            RemoveAllFakeNulls();

            if (!_cursorVisibleHandlers.Contains(gameObject))
            {
                Debug.LogWarning($"[CursorManager] Gameobject {gameObject.name} not in visible handlers list");
                return;
            }

            _cursorVisibleHandlers.Remove(gameObject);

            CheckCursorVisibleHandlers();
        }

        private static void RemoveAllFakeNulls()
        {
            for (int i = 0; i < _cursorVisibleHandlers.Count; i++)
            {
                if (_cursorVisibleHandlers[i] == null)
                {
                    _cursorVisibleHandlers.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void CheckCursorVisibleHandlers()
        {
            if (_cursorVisibleHandlers.Count > 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}