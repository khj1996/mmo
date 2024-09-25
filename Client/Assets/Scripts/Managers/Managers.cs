using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance = null; // 유일성이 보장된다

    public static Managers Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<Managers>();

                if (_instance is null) return null;
            }

            return _instance;
        }
    }

    #region Contents

    InventoryManager _inven = new InventoryManager();
    MapManager _map = new MapManager();
    ObjectManager _obj = new ObjectManager();
    NetworkManager _network = new NetworkManager();
    WebManager _web = new WebManager();

    public static InventoryManager Inven
    {
        get { return Instance._inven; }
    }

    public static MapManager Map
    {
        get { return Instance._map; }
    }

    public static ObjectManager Object
    {
        get { return Instance._obj; }
    }

    public static NetworkManager Network
    {
        get { return Instance._network; }
    }

    public static WebManager Web
    {
        get { return Instance._web; }
    }

    #endregion

    #region Core

    [SerializeField] DataManager _data = new DataManager() ;
    [SerializeField] PoolManager _pool = new PoolManager();
    [SerializeField] ResourceManager _resource = new ResourceManager();
    [SerializeField] SceneLoader _scene;
    [SerializeField] SoundManager _sound = new SoundManager();
    [SerializeField] UIManager _ui = new UIManager();

    public static DataManager Data
    {
        get { return Instance._data; }
    }

    public static PoolManager Pool
    {
        get { return Instance._pool; }
    }

    public static ResourceManager Resource
    {
        get { return Instance._resource; }
    }

    public static SceneLoader Scene
    {
        get { return Instance._scene; }
    }

    public static SoundManager Sound
    {
        get { return Instance._sound; }
    }

    public static UIManager UI
    {
        get { return Instance._ui; }
    }

    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        _network.Update();
    }

    void Init()
    {
        _data.Init();
        _pool.Init();
        _sound.Init();
        Application.targetFrameRate = 60;
    }
}