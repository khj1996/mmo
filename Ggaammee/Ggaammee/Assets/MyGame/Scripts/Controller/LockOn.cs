using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    public InputSystem input;

    public float lcokOnRadius = 5f;

    public Camera _mainCamera;
    public float minViewAngle = -70.0f;
    public float maxViewAngle = 70.0f;
    public float lookAtSmoothing = 5f;

    [SerializeField] public Transform lockOnImage;
    [SerializeField] public List<MonsterController> targetEnemy = new List<MonsterController>();

    private bool isLockOn = false;
    public bool isFindTarget = false;
    public Vector3 currentTargetPosition;

    public MonsterController currentTarget;

    private int currentIndex = 0;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        input = GetComponent<InputSystem>();
        lockOnImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isFindTarget)
        {
            if (isTargetRange())
            {
                LookAtTarget();
            }
            else
            {
                ResetTarget();
            }
        }

        if (input.lockOn)
        {
            isLockOn = !isLockOn;
            if (isLockOn)
            {
                FindLockOnTarget();
            }

            input.lockOn = false;
        }
    }


    private void FindLockOnTarget()
    {
        Collider[] findTarget = Physics.OverlapSphere(transform.position, lcokOnRadius, LayerData.MonsterLayer);

        if (findTarget.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < findTarget.Length; i++)
        {
            MonsterController target = findTarget[i].GetComponent<MonsterController>();

            if (target != null)
            {
                Vector3 targetDirection = target.transform.position - transform.position;

                float viewAngle = Vector3.Angle(targetDirection, _mainCamera.transform.forward);

                if (viewAngle > minViewAngle && viewAngle < maxViewAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(transform.position, target.lockOnPos.transform.position, out hit, LayerData.MonsterLayer))
                    {
                        targetEnemy.Add(target);
                    }
                }
            }
        }

        LockOnTarget();
    }

    public void CnangeIndex()
    {
        if (currentIndex < targetEnemy.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }

        currentTarget = targetEnemy[currentIndex]; 
    }

    private void LockOnTarget()
    {
        float shortDistance = Mathf.Infinity;

        for (int i = 0; i < targetEnemy.Count; i++)
        {
            if (targetEnemy != null)
            {
                float distanceFromTarget = Vector3.Distance(transform.position, targetEnemy[i].transform.position);
                if (distanceFromTarget < shortDistance)
                {
                    shortDistance = distanceFromTarget;
                    currentTarget = targetEnemy[i];
                    currentIndex = i;
                }
            }
            else
            {
                ResetTarget();
            }
        }

        if (currentTarget != null)
        {
            FindTarget();
        }
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        currentTargetPosition = currentTarget.lockOnPos.transform.position;
        lockOnImage.position = _mainCamera.WorldToScreenPoint(currentTargetPosition);

        Vector3 dir = (currentTargetPosition - transform.position).normalized;
// dir.y = 0.0f; // y축을 0으로 고정해서 xz 평면상에서만 회전하도록 설정

// 또는 현재 y 값을 유지하는 방식
        dir.y = transform.forward.y; 

        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * lookAtSmoothing);

    }

    private void FindTarget()
    {
        isFindTarget = true;
        lockOnImage.gameObject.SetActive(true);
    }

    private void ResetTarget()
    {
        isFindTarget = false;
        targetEnemy.Clear();
        lockOnImage.gameObject.SetActive(false);
    }

    private bool isTargetRange()
    {
        if (!isLockOn)
            return false;
        
        float distance = (transform.position - currentTargetPosition).magnitude;

        return !(distance > lcokOnRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, lcokOnRadius);
    }
}