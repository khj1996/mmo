using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")] public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;


    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private PlayerInput _playerInput;
    private InputSystem _input;
    private const float _threshold = 0.01f;

    private LockOn _lockOn;
    [SerializeField] private float cameraDirectionY = 0;
    public float cameraSmoothing = 5f;

    public GameObject _mainCamera;


    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<InputSystem>();
        _lockOn = GetComponent<LockOn>();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }


    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        if (_lockOn.isFindTarget && _lockOn.currentTarget != null)
        {
            Vector3 direction = (_lockOn.currentTarget.lockOnPos.transform.position - CinemachineCameraTarget.transform.position).normalized;

            direction.y = cameraDirectionY;

            CinemachineCameraTarget.transform.forward = Vector3.Lerp(CinemachineCameraTarget.transform.forward, direction, Time.deltaTime * cameraSmoothing);

            Vector3 camAngles = _mainCamera.transform.eulerAngles;

            _cinemachineTargetYaw = camAngles.y;
            _cinemachineTargetPitch = camAngles.x;
        }
        else
        {
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        Debug.Log(_cinemachineTargetPitch);
    }


    private static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;

        return Mathf.Clamp(angle, min, max);
    }
}