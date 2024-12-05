using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LadderSetup : MonoBehaviour
{
    public Transform startPoint; // 사다리 아래쪽 위치
    public Transform endPoint;   // 사다리 위쪽 위치

    private void Start()
    {
        /*// NavMeshLink를 동적으로 생성
        var navMeshLink = gameObject.AddComponent<NavMeshLink>();
        navMeshLink.startPoint = startPoint.localPosition;
        navMeshLink.endPoint = endPoint.localPosition;
        navMeshLink.width = 1.0f; // 사다리의 너비
        navMeshLink.costModifier = 1; // 이동 비용 설정
        navMeshLink.bidirectional = true; // 양방향 이동 가능
        //navMeshLink.AddLink();*/
    }
}