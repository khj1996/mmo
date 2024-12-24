using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private bool IsLoading { get; set; } = false;

    private SceneInstance currentlyLoadedScene;

    public string CurrentSceneName { get; private set; }

    public void LoadScene(string sceneName)
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;

        StartCoroutine(SwitchScene(sceneName));
    }

    private IEnumerator SwitchScene(string newSceneName)
    {
        if (currentlyLoadedScene.Scene.name != null)
        {
            Addressables.UnloadSceneAsync(currentlyLoadedScene, true).Completed += Addressables.Release;


            yield return new WaitUntil(() => !currentlyLoadedScene.Scene.isLoaded);
        }

        Addressables.LoadSceneAsync(newSceneName, LoadSceneMode.Additive).Completed += (result) =>
        {
            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                currentlyLoadedScene = result.Result;
                CurrentSceneName = newSceneName;

                SceneManager.SetActiveScene(result.Result.Scene);
            }
        };

        IsLoading = false;
    }
}