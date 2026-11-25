using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        transform.LookAt(CameraController.Instance.MouseRayCastPoint);
    }
}
