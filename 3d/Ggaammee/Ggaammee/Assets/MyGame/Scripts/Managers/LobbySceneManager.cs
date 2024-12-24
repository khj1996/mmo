using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        startButton.gameObject.BindEvent(_ => { SceneLoader.Instance.LoadScene("GameScene"); });
        exitButton.gameObject.BindEvent(_ => { Application.Quit(); });
    }
}