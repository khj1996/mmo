using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    public InputSystem input;
    public float lcokOnRadius = 5f;
    public float maxViewAngle = 70.0f;
    public float minViewAngle = -70.0f;
    public float lookAtSmoothing = 5f;
    public bool isFindTarget = false;
    public Vector3 currentTargetPosition;
    public MonsterController currentTarget;

    [SerializeField] public Transform lockOnImage;
    [SerializeField] public List<MonsterController> targetEnemy = new List<MonsterController>();

    private Camera _mainCamera;
    private bool isLockOn = false;
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
        HandleLockOnToggle();
        HandleTargetTracking();
    }

    private void HandleLockOnToggle()
    {
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

    private void HandleTargetTracking()
    {
        if (isFindTarget)
        {
            if (currentTarget == null || !currentTarget.isActiveAndEnabled)
            {
                ResetTarget();
                return;
            }

            if (IsTargetRange())
            {
                LookAtTarget();
            }
            else
            {
                ResetTarget();
            }
        }
    }

    private void FindLockOnTarget()
    {
        Collider[] findTarget = Physics.OverlapSphere(transform.position, lcokOnRadius, LayerData.MonsterLayer);

        if (findTarget.Length == 0) return;

        for (int i = 0; i < findTarget.Length; i++)
        {
            MonsterController target = findTarget[i].GetComponent<MonsterController>();

            if (target != null && IsTargetInView(target))
            {
                targetEnemy.Add(target);
            }
        }

        LockOnTarget();
    }

    private bool IsTargetInView(MonsterController target)
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float viewAngle = Vector3.Angle(targetDirection, _mainCamera.transform.forward);

        if (viewAngle > minViewAngle && viewAngle < maxViewAngle)
        {
            if (Physics.Linecast(transform.position, target.lockOnPos.transform.position, out _, LayerData.MonsterLayer))
            {
                return true;
            }
        }

        return false;
    }

    private void LockOnTarget()
    {
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i < targetEnemy.Count; i++)
        {
            if (targetEnemy[i] == null) continue;

            float distanceFromTarget = Vector3.Distance(transform.position, targetEnemy[i].transform.position);

            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                currentTarget = targetEnemy[i];
                currentIndex = i;
            }
        }

        if (currentTarget != null)
        {
            FindTarget();
        }
        else
        {
            ResetTarget();
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

        Vector3 direction = (currentTargetPosition - transform.position).normalized;
        direction.y = transform.forward.y;

        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * lookAtSmoothing);
    }

    private void ResetTarget()
    {
        isFindTarget = false;
        targetEnemy.Clear();
        lockOnImage.gameObject.SetActive(false);
    }

    public void CnangeIndex()
    {
        if (targetEnemy.Count == 0) return;

        currentIndex = (currentIndex + 1) % targetEnemy.Count;
        currentTarget = targetEnemy[currentIndex];
    }

    private bool IsTargetRange()
    {
        if (!isLockOn) return false;

        float distance = Vector3.Distance(transform.position, currentTargetPosition);
        return distance <= lcokOnRadius;
    }

    private void FindTarget()
    {
        isFindTarget = true;
        lockOnImage.gameObject.SetActive(true);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lcokOnRadius);
    }
#endif
}