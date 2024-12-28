using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        Util.BindEvent(startButton.gameObject, _ => { SceneLoader.Instance.LoadScene("GameScene"); });
        Util.BindEvent(exitButton.gameObject, _ => { Application.Quit(); });
    }
}