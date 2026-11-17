using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensative = 200;
    [SerializeField, Min(0f)] private float _transitionTime = 1f;

    [SerializeField, Range(0f, 1f)] private float _deadZoneX = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _deadZoneY = 0.3f;

    private float _maxXAngle = 360f;
    private float _minXAngle = -360f;

    private float _maxYAngle = 360f;
    private float _minYAngle = -360f;

    private float _rotationX;
    private float _rotationY;

    private string _currentCameraPosition;

    [Serializable]
    private class CameraPosition
    {
        public string name;

        [Range(-360f, 360f)] public float _maxXAngle;
        [Range(-360f, 360f)] public float _minXAngle;

        [Range(-360f, 360f)] public float _maxYAngle;
        [Range(-360f, 360f)] public float _minYAngle;

        public Transform transform;
        public Transform lookTarget;

        public UnityEvent onSelected;
        public UnityEvent onDeselected;
    }

    [SerializeField] private List<CameraPosition> cameraPositions = new();

    private bool _transition = false;

    private void Awake()
    {
        Vector3 initialRotation = transform.eulerAngles;
        _rotationX = Mathf.DeltaAngle(0f, initialRotation.x);
        _rotationY = Mathf.DeltaAngle(0f, initialRotation.y);

        ChangeToPosition("Main");

        GetSettings();
        GameSettings.SettingsApplied += GetSettings;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || _transition)
            return;

        float mouseX = ApplyDeadZone(Input.mousePosition.x / Screen.width * 2f - 1f, _deadZoneX);
        float mouseY = ApplyDeadZone(Input.mousePosition.y / Screen.height * 2f - 1f, _deadZoneY);

        Vector3 vectorAxis = _sensative * Time.deltaTime * new Vector3(mouseY, mouseX, 0);

        _rotationX = Mathf.Clamp(_rotationX - vectorAxis.x, _minXAngle, _maxXAngle);

        float yRangeCenter = (_minYAngle + _maxYAngle) * 0.5f;
        _rotationY = yRangeCenter + Mathf.Repeat(_rotationY + vectorAxis.y - yRangeCenter + 180f, 360f) - 180f;
        _rotationY = Mathf.Clamp(_rotationY, _minYAngle, _maxYAngle);

        transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }

    private float ApplyDeadZone(float value, float deadZone)
    {
        return Mathf.Sign(value) * Mathf.InverseLerp(deadZone, 1f, Mathf.Abs(value));
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

    public void ChangeToPosition(string positionName) => StartCoroutine(ChangeToPosition_C(positionName));

    private IEnumerator ChangeToPosition_C(string positionName)
    {
        if (_transition)
            yield break;

        CameraPosition cameraPosition = cameraPositions?.Find(x => x != null && x.name == positionName);

        if (cameraPosition == null)
        {
            Debug.LogWarning($"[CameraController] Camera position {positionName} not found");
            yield break;
        }

        if (cameraPosition.name == _currentCameraPosition)
        {
            Debug.Log($"[CameraController] Camera position is already on {positionName}");
            yield break;
        }

        if (cameraPosition.transform == null)
        {
            Debug.LogWarning($"[CameraController] Camera position {positionName} has no transform");
            yield break;
        }

        float duration = _transitionTime;
        if (duration < 0f || float.IsNaN(duration) || float.IsInfinity(duration))
        {
            Debug.LogWarning("[CameraController] Transition time must be finite and non-negative");
            yield break;
        }

        static float ClampWorldAngle(float angle, float min, float max)
        {
            float rangeCenter = (min + max) * 0.5f;
            float equivalentAngle = angle + Mathf.Round((rangeCenter - angle) / 360f) * 360f;
            return Mathf.Clamp(equivalentAngle, min, max);
        }

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 targetPosition = cameraPosition.transform.position;
        Quaternion targetRotation = cameraPosition.transform.rotation;
        bool preserveLookPoint = cameraPosition.lookTarget == null;
        Vector3 previousLookPoint = startPosition;

        if (preserveLookPoint)
        {
            Vector3 initialDirection = startRotation * Vector3.forward;
            previousLookPoint = startPosition + initialDirection * 1000f;
            if (Physics.Raycast(startPosition, initialDirection, out RaycastHit hit,
                Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                previousLookPoint = hit.point;
            }

            targetRotation = startRotation;
            Vector3 lookDirection = previousLookPoint - targetPosition;
            if (lookDirection.sqrMagnitude > 0.000001f)
                targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        else
        {
            Vector3 lookDirection = cameraPosition.lookTarget.position - targetPosition;
            if (lookDirection.sqrMagnitude > 0.000001f)
                targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        Vector3 targetAngles = targetRotation.eulerAngles;
        float targetPitch = ClampWorldAngle(targetAngles.x,
            cameraPosition._minXAngle, cameraPosition._maxXAngle);
        float targetYaw = ClampWorldAngle(targetAngles.y,
            cameraPosition._minYAngle, cameraPosition._maxYAngle);
        targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);

        cameraPositions?.Find(x => x != null && x.name == _currentCameraPosition)?.onDeselected.Invoke();
        cameraPosition.onSelected.Invoke();

        _transition = true;
        try
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                Quaternion currentTargetRotation = targetRotation;

                if (preserveLookPoint)
                {
                    Vector3 lookDirection = previousLookPoint - currentPosition;
                    if (lookDirection.sqrMagnitude > 0.000001f)
                    {
                        Vector3 lookAngles = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles;
                        currentTargetRotation = Quaternion.Euler(
                            ClampWorldAngle(lookAngles.x, cameraPosition._minXAngle, cameraPosition._maxXAngle),
                            ClampWorldAngle(lookAngles.y, cameraPosition._minYAngle, cameraPosition._maxYAngle),
                            0f);
                    }
                    else
                    {
                        currentTargetRotation = transform.rotation;
                    }
                }

                transform.SetPositionAndRotation(
                    currentPosition,
                    Quaternion.Slerp(startRotation, currentTargetRotation, progress));

                yield return null;
                elapsed += Time.deltaTime;
            }

            transform.SetPositionAndRotation(targetPosition, targetRotation);

            _rotationX = targetPitch;
            _rotationY = targetYaw;
            _minXAngle = cameraPosition._minXAngle;
            _maxXAngle = cameraPosition._maxXAngle;
            _minYAngle = cameraPosition._minYAngle;
            _maxYAngle = cameraPosition._maxYAngle;
        }
        finally
        {
            _transition = false;
            _currentCameraPosition = cameraPosition.name;
        }
    }
}
