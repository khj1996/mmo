using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private CinemachineConfiner2D confiner;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        confiner.m_BoundingShape2D = Managers.Map.CameraClamp;
        confiner.InvalidateCache();
        
        Managers.Map.MapChangeAction -= SetNewBoundingShape;
        Managers.Map.MapChangeAction += SetNewBoundingShape;
    }

    public void SetNewBoundingShape()
    {
        confiner.m_BoundingShape2D = Managers.Map.CameraClamp;
        confiner.InvalidateCache();
    }
}