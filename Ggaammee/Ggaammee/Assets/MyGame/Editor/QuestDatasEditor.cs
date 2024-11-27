using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomEditor(typeof(QuestDatas))]
public class QuestDatasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector GUI 표시
        base.OnInspectorGUI();

        // ScriptableObject 캐스팅
        QuestDatas questDatas = (QuestDatas)target;

        // 버튼 추가
        if (GUILayout.Button("Add QuestData in Same Folder"))
        {
            AddquestDataInSameFolder(questDatas);
        }
    }

    private void AddquestDataInSameFolder(QuestDatas questDatas)
    {
        // 현재 questDatas의 경로 가져오기
        string path = AssetDatabase.GetAssetPath(questDatas);
        string folder = Path.GetDirectoryName(path);

        // 같은 폴더의 모든 questData 검색
        string[] guids = AssetDatabase.FindAssets("t:questData", new[] { folder });

        // 검색된 questData 추가
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            QuestData questData = AssetDatabase.LoadAssetAtPath<QuestData>(assetPath);

            if (!questDatas.questDatas.Contains(questData))
            {
                questDatas.questDatas.Add(questData);
            }
        }

        // 변경사항 저장
        EditorUtility.SetDirty(questDatas);
        AssetDatabase.SaveAssets();

        Debug.Log($"Added {guids.Length} questData(s) to the list.");
    }
}