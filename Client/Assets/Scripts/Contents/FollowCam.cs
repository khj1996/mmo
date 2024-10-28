using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]private CinemachineConfiner2D confiner;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        confiner.m_BoundingShape2D = Managers.Map.CameraClamp;
        confiner.InvalidateCache();
    }

    public void SetNewBoundingShape(BoxCollider2D _collider2D)
    {
        confiner.m_BoundingShape2D = _collider2D;
        confiner.InvalidateCache();
    }
}