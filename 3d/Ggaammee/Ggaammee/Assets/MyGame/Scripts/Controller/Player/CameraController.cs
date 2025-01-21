using System;
using Cinemachine;
using EasyButtons;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    public GameObject CinemachineCameraTarget;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;
    [SerializeField] private float CameraAngleOverride = 0.0f;
    [SerializeField] private bool LockCameraPosition = false;


    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private PlayerInput _playerInput;
    private InputSystem _input;
    private const float _threshold = 0.01f;

    private PlayerLockOn _playerLockOn;
    [SerializeField] private float cameraDirectionY = 0;
    public float cameraSmoothing = 5f;

    private Camera _mainCamera;
    [SerializeField] private Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;


    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            cinemachine3RdPersonFollow = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<InputSystem>();
        _playerLockOn = GetComponent<PlayerLockOn>();
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

        if (_playerLockOn.isFindTarget && _playerLockOn.currentTarget != null)
        {
            Vector3 direction = (_playerLockOn.currentTarget.lockOnPos.transform.position - CinemachineCameraTarget.transform.position).normalized;

            direction.y = cameraDirectionY;

            Vector3 currentForward = CinemachineCameraTarget.transform.forward;
            if (!Mathf.Approximately(Vector3.Angle(currentForward, direction), 0))
            {
                CinemachineCameraTarget.transform.forward = Vector3.Lerp(currentForward, direction, Time.deltaTime * cameraSmoothing);
            }

            Vector3 camAngles = _mainCamera.transform.eulerAngles;

            _cinemachineTargetYaw = camAngles.y;
            _cinemachineTargetPitch = camAngles.x;
        }
        else
        {
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
    }


    private float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;

        return Mathf.Clamp(angle, min, max);
    }

    public void ChangeViewDistance(float dis)
    {
        cinemachine3RdPersonFollow.CameraDistance = dis;
    }
}