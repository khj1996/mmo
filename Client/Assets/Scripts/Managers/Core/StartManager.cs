using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] AssetReference persistentScene = default;

    Scene currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        LoadPersistent();
    }

    private void LoadPersistent()
    {
        persistentScene.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadCompleted;
    }

    private void LoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        Managers.Scene.LoadScene("Login");
        SceneManager.UnloadSceneAsync(currentScene);
    }
}