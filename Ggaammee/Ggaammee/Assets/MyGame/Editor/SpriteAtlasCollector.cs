using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteAtlasCollector : EditorWindow
{
    private SpriteAtlas targetAtlas;

    [MenuItem("Tools/Add Sprites to Atlas")]
    public static void ShowWindow()
    {
        GetWindow<SpriteAtlasCollector>("Add Sprites to Atlas");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Sprites to Sprite Atlas", EditorStyles.boldLabel);

        // 타겟 스프라이트 아틀라스 선택
        targetAtlas = (SpriteAtlas)EditorGUILayout.ObjectField("Target Atlas", targetAtlas, typeof(SpriteAtlas), false);

        if (GUILayout.Button("Collect and Add Sprites"))
        {
            if (targetAtlas == null)
            {
                Debug.LogError("Please assign a target Sprite Atlas.");
                return;
            }

            AddSpritesToAtlas();
        }
    }

    private void AddSpritesToAtlas()
    {
        // 현재 씬에서 사용 중인 모든 스프라이트 가져오기
        Image[] images = FindObjectsOfType<Image>();
        HashSet<Sprite> sprites = new HashSet<Sprite>();

        foreach (var image in images)
        {
            if (image.sprite != null)
                sprites.Add(image.sprite);
        }

        // Asset 경로로 변환
        List<Object> spriteAssets = new List<Object>();
        foreach (var sprite in sprites)
        {
            string path = AssetDatabase.GetAssetPath(sprite);
            if (!string.IsNullOrEmpty(path))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null)
                    spriteAssets.Add(asset);
            }
        }

        // 스프라이트 아틀라스에 추가
        AddToSpriteAtlas(spriteAssets);

        Debug.Log($"Added {spriteAssets.Count} sprites to the Sprite Atlas.");
    }

    private void AddToSpriteAtlas(List<Object> spriteAssets)
    {
        // 기존 아틀라스에 등록된 에셋 가져오기
        Object[] existingAssets = targetAtlas.GetPackables();

        // 중복 제거
        HashSet<Object> assetsToAdd = new HashSet<Object>(spriteAssets);
        foreach (var existingAsset in existingAssets)
        {
            assetsToAdd.Remove(existingAsset);
        }

        // 새 에셋 추가
        if (assetsToAdd.Count > 0)
        {
            Object[] finalAssets = new Object[assetsToAdd.Count];
            assetsToAdd.CopyTo(finalAssets);
            targetAtlas.Add(finalAssets);

            EditorUtility.SetDirty(targetAtlas); // 아틀라스 저장 표시
            AssetDatabase.SaveAssets();         // 변경사항 저장
            AssetDatabase.Refresh();           // 프로젝트 갱신
        }
        else
        {
            Debug.Log("No new sprites to add to the Sprite Atlas.");
        }
    }
}
