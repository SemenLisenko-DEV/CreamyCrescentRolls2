using UnityEngine;
using UnityEngine.EventSystems;

public class DragInZone : MonoBehaviour, IDragHandler
{
    [SerializeField] private BoxCollider _moveArea;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector3 point = CameraController.Instance.MouseRayCastPoint;

        Move(point);
    }

    private void Move(Vector3 newPosition)
    {
        if (_moveArea == null)
        {
            Debug.LogWarning("[Worksheet] Worksheet move area not defined");
            return;
        }

        Vector3 areaPosition = _moveArea.transform.position;

        Vector3 halfSize = _moveArea.size * 0.5f;

        newPosition.x = Mathf.Clamp(newPosition.x, areaPosition.x - halfSize.x, areaPosition.x + halfSize.x);
        newPosition.y = Mathf.Clamp(newPosition.y, areaPosition.y - halfSize.y, areaPosition.y + halfSize.y);
        newPosition.z = Mathf.Clamp(newPosition.z, areaPosition.z - halfSize.z, areaPosition.z + halfSize.z);

        transform.position = newPosition;
    }
}
