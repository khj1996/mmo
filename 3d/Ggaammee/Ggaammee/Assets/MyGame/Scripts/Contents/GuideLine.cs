using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuideLine : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private LineRenderer lineRenderer; // 라인 렌더러

    [SerializeField] private NavMeshAgent navMeshAgent; // 플레이어의 NavMeshAgent
    [SerializeField] private Transform playerTransform; // 플레이어 Transform

    [Header("Settings")] [SerializeField] private float lineHeightOffset = 0.2f; // 가이드 라인의 높이 오프셋
    [SerializeField] private float destinationThreshold = 1.0f; // 목표지점 도착 판정 거리

    private Vector3 currentTargetPosition; // 현재 목표 위치
    private Coroutine drawGuideCoroutine; // 가이드 라인 코루틴 핸들

    /// <summary>
    /// 퀘스트 목표 위치를 설정하고 가이드를 시작합니다.
    /// </summary>
    public void StartGuide(Vector3 targetPosition)
    {
        currentTargetPosition = targetPosition;
        navMeshAgent.SetDestination(currentTargetPosition);

        if (drawGuideCoroutine != null)
        {
            StopCoroutine(drawGuideCoroutine);
        }

        lineRenderer.enabled = true;
        drawGuideCoroutine = StartCoroutine(DrawGuideLine());
    }

    /// <summary>
    /// 가이드를 중지합니다.
    /// </summary>
    public void StopGuide()
    {
        if (drawGuideCoroutine != null)
        {
            StopCoroutine(drawGuideCoroutine);
            drawGuideCoroutine = null;
        }

        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 가이드 라인을 그리는 코루틴
    /// </summary>
    private IEnumerator DrawGuideLine()
    {
        while (Vector3.Distance(playerTransform.position, currentTargetPosition) > destinationThreshold)
        {
            UpdateGuideLine();
            yield return null;
        }

        StopGuide();
    }

    /// <summary>
    /// 가이드 라인을 업데이트합니다.
    /// </summary>
    private void UpdateGuideLine()
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(currentTargetPosition, path);

        List<Vector3> finalPositions = new List<Vector3>();

        for (int i = 0; i < path.corners.Length; i++)
        {
            // 현재 포인트 추가
            finalPositions.Add(path.corners[i] + Vector3.up * lineHeightOffset);

            // 다음 포인트와의 높이 차이를 계산하여 보정 포인트 추가
            if (i < path.corners.Length - 1)
            {
                Vector3 current = path.corners[i];
                Vector3 next = path.corners[i + 1];

                float heightDifference = Mathf.Abs(next.y - current.y);

                if (heightDifference > 1.5f) // 높이 차이가 2 이상일 경우
                {
                    // 보정 포인트 계산 (X, Z는 그대로, Y만 높임)
                    Vector3 adjustedPoint = new Vector3(current.x, next.y, current.z);
                    finalPositions.Add(adjustedPoint + Vector3.up * lineHeightOffset);
                }
            }
        }

        // LineRenderer 업데이트
        lineRenderer.positionCount = finalPositions.Count;

        for (int i = 0; i < finalPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, finalPositions[i]);
        }
    }
}