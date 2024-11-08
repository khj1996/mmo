using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void Combine()
    {
        // 자식 오브젝트에서 모든 MeshFilter 컴포넌트를 가져옴
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        // CombineInstance 배열 생성
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;

        // 각 MeshFilter의 정보를 CombineInstance에 저장
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // 원본 메쉬 비활성화
            i++;
        }

        // 새로운 메쉬 생성 및 병합 적용
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // 현재 오브젝트에 병합된 메쉬 할당
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        // 병합 후 이 오브젝트 활성화
        gameObject.SetActive(true);
    }
}