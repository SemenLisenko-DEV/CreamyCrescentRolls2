using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensative = 200;
    [SerializeField, Range(0f, 90f)] private float _maxXAngle = 80f;

    private float _rotationX;
    private float _rotationY;

    private void Awake()
    {
        Vector3 initialRotation = transform.localEulerAngles;
        _rotationX = Mathf.DeltaAngle(0f, initialRotation.x);
        _rotationY = initialRotation.y;

        GetSettings();
        GameSettings.SettingsApplied += GetSettings;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 vectorAxis = _sensative * Time.deltaTime * new Vector3(mouseY, mouseX, 0);
        float maxXAngle = Mathf.Clamp(_maxXAngle, 0f, 90f);

        _rotationX = Mathf.Clamp(_rotationX - vectorAxis.x, -maxXAngle, maxXAngle);
        _rotationY = Mathf.Repeat(_rotationY + vectorAxis.y, 360f);

        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }

    private void OnDestroy()
    {
        GameSettings.SettingsApplied -= GetSettings;
    }

    public void GetSettings()
    {
        try
        {
            _sensative = GameSettings.GetSetting<float>("Sensative");
        }
        catch(Exception e)
        {
            Debug.LogWarning($"[CameraController] Unexpected error while getting settings: {e} ");
        }
    }
}
