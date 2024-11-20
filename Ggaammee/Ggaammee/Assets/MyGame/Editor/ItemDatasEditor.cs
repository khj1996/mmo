using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomEditor(typeof(ItemDatas))]
public class ItemDatasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector GUI 표시
        base.OnInspectorGUI();

        // ScriptableObject 캐스팅
        ItemDatas itemDatas = (ItemDatas)target;

        // 버튼 추가
        if (GUILayout.Button("Add ItemData in Same Folder"))
        {
            AddItemDataInSameFolder(itemDatas);
        }
    }

    private void AddItemDataInSameFolder(ItemDatas itemDatas)
    {
        // 현재 ItemDatas의 경로 가져오기
        string path = AssetDatabase.GetAssetPath(itemDatas);
        string folder = Path.GetDirectoryName(path);

        // 같은 폴더의 모든 ItemData 검색
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { folder });

        // 검색된 ItemData 추가
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);

            if (!itemDatas.itemDatas.Contains(itemData))
            {
                itemDatas.itemDatas.Add(itemData);
            }
        }

        // 변경사항 저장
        EditorUtility.SetDirty(itemDatas);
        AssetDatabase.SaveAssets();

        Debug.Log($"Added {guids.Length} ItemData(s) to the list.");
    }
}