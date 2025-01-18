using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuideLine : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform playerTransform;

    [Header("Settings")] [SerializeField] private float lineHeightOffset = 0.2f;
    [SerializeField] private float destinationThreshold = 1.0f;

    private Vector3 currentTargetPosition;
    private Coroutine drawGuideCoroutine;
    private NavMeshPath path;
    private List<Vector3> pathVector;

    private void Start()
    {
        path = new NavMeshPath();
        pathVector = new List<Vector3>();
    }


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

    public void StopGuide()
    {
        if (drawGuideCoroutine != null)
        {
            StopCoroutine(drawGuideCoroutine);
            drawGuideCoroutine = null;
        }

        lineRenderer.enabled = false;
    }

    private IEnumerator DrawGuideLine()
    {
        while (Vector3.Distance(playerTransform.position, currentTargetPosition) > destinationThreshold)
        {
            UpdateGuideLine();
            yield return null;
        }

        StopGuide();
    }

    private void UpdateGuideLine()
    {
        navMeshAgent.CalculatePath(currentTargetPosition, path);

        pathVector.Clear();

        for (int i = 0; i < path.corners.Length; i++)
        {
            pathVector.Add(path.corners[i] + Vector3.up * lineHeightOffset);

            if (i < path.corners.Length - 1)
            {
                Vector3 current = path.corners[i];
                Vector3 next = path.corners[i + 1];

                float heightDifference = Mathf.Abs(next.y - current.y);

                if (heightDifference > 1.5f)
                {
                    if (next.y > current.y)
                    {
                        Vector3 adjustedPoint = new Vector3(current.x, next.y, current.z);
                        pathVector.Add(adjustedPoint + Vector3.up * lineHeightOffset);
                    }
                    else
                    {
                        Vector3 adjustedPoint = new Vector3(next.x, current.y, next.z);
                        pathVector.Add(adjustedPoint + Vector3.up * lineHeightOffset);
                    }
                }
            }
        }

        lineRenderer.positionCount = pathVector.Count;
        for (int i = 0; i < pathVector.Count; i++)
        {
            lineRenderer.SetPosition(i, pathVector[i]);
        }
    }
}