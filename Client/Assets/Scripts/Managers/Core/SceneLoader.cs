using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO SceneDataSO;

    public bool isLoading { get; private set; } = false;

    private AssetReference sceneToLoad;
    private AssetReference currentlyLoadedScene;

    AsyncOperationHandle<SceneInstance> SceneLoadOperationHandle;

    public string currentSceneName;

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        currentlyLoadedScene = sceneToLoad;

        Scene s = obj.Result.Scene;
        SceneManager.SetActiveScene(s);
        isLoading = false;
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = SceneDataSO.GetScene(sceneName);

        if (sceneToLoad != null)
        {
            if (isLoading)
                return;

            isLoading = true;

            currentSceneName = sceneName;

            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            Debug.LogError("씬 찾기 실패");
        }
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (currentlyLoadedScene != null)
        {
            if (currentlyLoadedScene == sceneToLoad)
            {
                yield return currentlyLoadedScene.UnLoadScene().Task.IsCompleted;
            }
            else
            {
                if (currentlyLoadedScene.OperationHandle.IsValid())
                {
                    currentlyLoadedScene.UnLoadScene();
                }
            }
        }

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        SceneLoadOperationHandle = sceneToLoad.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
        SceneLoadOperationHandle.Completed += OnNewSceneLoaded;
    }
}