using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO SceneDataSO;

    private bool isLoading = false; //씬 로드 여부 확인

    private AssetReference sceneToLoad;
    private AssetReference currentlyLoadedScene;

    AsyncOperationHandle<SceneInstance> SceneLoadOperationHandle;

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        currentlyLoadedScene = sceneToLoad;
        isLoading = false;

        Scene s = obj.Result.Scene;
        SceneManager.SetActiveScene(s);
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = SceneDataSO.GetScene(sceneName);

        if (sceneToLoad != null)
        {
            if (isLoading)
                return;

            isLoading = true;


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

    public void TransitScene()
    {
    }
}