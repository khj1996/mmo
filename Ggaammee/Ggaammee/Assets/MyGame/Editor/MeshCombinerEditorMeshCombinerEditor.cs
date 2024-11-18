using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector 표시
        DrawDefaultInspector();

        // MeshCombiner 인스턴스 가져오기
        MeshCombiner meshCombiner = (MeshCombiner)target;

        // 버튼 추가
        if (GUILayout.Button("Combine Meshes"))
        {
            meshCombiner.Combine();
        }
    }
}