using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] private bool _checkSelfActive = false;
    [SerializeField] private UnityEvent _onEnable;

    private void OnEnable()
    {
        if (_checkSelfActive)
        {
            if (gameObject.activeSelf)
            {
                _onEnable.Invoke();
            }

            return;
        }

        _onEnable.Invoke();
    }
}