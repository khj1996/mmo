using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorGameSceneLoader : Editor
{
    private static string sceneFolderLocation = "Assets/MyGame/Scenes/";

    private static string startScene = "StartScene.unity";
    private static string lobbyScene = "LobbyScene.unity";
    private static string gameScene = "GameScene.unity";

    private static string persistentScene = "Manager/PersistentScene.unity";

    [MenuItem("Tools/Scenes/Load StartScene")]
    public static void LoadstartScene()
    {
        LoadScene(startScene);
    }

    [MenuItem("Tools/Scenes/Load LobbyScene")]
    public static void LoadLobbyScene()
    {
        LoadScene(lobbyScene);
    }

    [MenuItem("Tools/Scenes/Load GameScene")]
    public static void LoadGameScene()
    {
        LoadScene(gameScene);
    }

    [MenuItem("Tools/Scenes/Load PersistentScene")]
    public static void LoadPersistentScene()
    {
        LoadScene(persistentScene);
    }

    private static void LoadScene(string selectedScenePath, OpenSceneMode openSceneMode = OpenSceneMode.Single)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(sceneFolderLocation + selectedScenePath, openSceneMode);
        }
    }
}