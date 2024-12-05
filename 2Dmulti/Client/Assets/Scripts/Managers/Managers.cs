using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Managers : MonoBehaviour
{
    private static Managers _instance = null;

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

    private InventoryManager _inven;
    private MapManager _map;
    private ObjectManager _obj;
    private NetworkManager _network;
    private WebManager _web;

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

    DataManager _data;
    PoolManager _pool;
    ResourceManager _resource;
    SoundManager _sound;
    UIManager _ui;
    [SerializeField] SceneLoader _scene;

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

    public void Init()
    {
        _data = new DataManager();
        _pool = new PoolManager();
        _resource = new ResourceManager();
        _sound = new SoundManager();
        _ui = new UIManager();
        _inven = new InventoryManager();
        _map = new MapManager();
        _obj = new ObjectManager();
        _network = new NetworkManager();
        _web = new WebManager();

        _inven.Init();
        _data.Init();
        _pool.Init();
        _sound.Init();
        Application.targetFrameRate = 60;
    }
}