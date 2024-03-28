using System;
using Cinemachine;
using NetDive.NetForm;
using UnityEngine;

namespace NetDive.Player
{
    public class TpCameraController : MonoBehaviour
    {
        private const float LookThreshold = 0.05f;

        [field: SerializeField] public Transform CameraTarget { get; set; }
        [field: SerializeField] public Vector2 PitchRange { get; set; } = new(-10f, 70f);
        [field: SerializeField] public Vector2 FocusedPitchRange { get; set; } = new(-30f, 70f);
        [field: SerializeField] private CinemachineVirtualCamera focusCamera;
        public Vector2 LookInput { get; set; }

        private float _yaw;
        private float _pitch;

        private void OnEnable()
        {
            if (CameraTarget == null) Debug.LogError("Expected CameraTarget to be assigned in the inspector");

            var euler = CameraTarget.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x;
        }

        private void Update()
        {
            if (LookInput.sqrMagnitude <= LookThreshold) return;

            _yaw = ClampAngle(_yaw + LookInput.x, float.MinValue, float.MaxValue);
            _pitch += LookInput.y;
            _pitch = !focusCamera.enabled
                ? ClampAngle(_pitch, PitchRange.x, PitchRange.y)
                : ClampAngle(_pitch, FocusedPitchRange.x, FocusedPitchRange.y);
            CameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360f;
            if (angle > 360) angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }

        public void ChangeFocus(NetFormSource source)
        {
            focusCamera.enabled = source != null;
        }
    }
}