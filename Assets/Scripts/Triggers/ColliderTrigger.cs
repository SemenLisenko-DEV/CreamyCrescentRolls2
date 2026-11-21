using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            Debug.LogWarning("[ColliderTrigger] Collider, that entered trigger, equals null/fakeNull");
            return;
        }    

        onTriggerEnter.Invoke(other);
    }
}
