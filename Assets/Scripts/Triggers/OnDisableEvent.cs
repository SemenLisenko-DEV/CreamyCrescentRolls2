using UnityEngine;
using UnityEngine.Events;

public class OnDisableEvent : MonoBehaviour
{
    [SerializeField] private bool _checkSelfActive = false;
    [SerializeField] private UnityEvent _onDisable;

    private void OnDisable()
    {
        if (_checkSelfActive)
        {
            if(!gameObject.activeSelf)
            {
                _onDisable.Invoke();
            }

            return;
        }

        _onDisable.Invoke();
    }
}
