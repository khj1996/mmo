using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Events/SceneList")]
public class SceneDataSO : ScriptableObject
{
    public SceneData[] data;

    public AssetReference GetScene(string sceneName)
    {
        foreach(var asset in data)
        {
            if(asset.SceneName == sceneName)
            {
                return asset.Scene;
            }
        }
        return null;
    }
}

[System.Serializable]
public class SceneData
{
    public string SceneName;
    public AssetReference Scene;

}