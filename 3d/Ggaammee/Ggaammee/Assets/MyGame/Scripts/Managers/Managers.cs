using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StartItem
{
    public ItemData data;
    public int count;
}

public class Managers : Singleton<Managers>
{
    [SerializeField] public List<StartItem> StartItemData;
    [SerializeField] public AudioClip bgmClip;

    private ObjectManager _obj;
    private DropManager _drop;
    private PoolManager _pool;
    private InventoryManager _inventory;
    private DataManager _data;
    private ShopManager _shop;
    private GameStateManager _gameState;
    private QuestManager _quest;
    private SoundManager _sound;

    public static ObjectManager ObjectManager
    {
        get { return Instance._obj; }
    }

    public static DropManager DropManager
    {
        get { return Instance._drop; }
    }

    public static PoolManager PoolManager
    {
        get { return Instance._pool; }
    }

    public static InventoryManager InventoryManager
    {
        get { return Instance._inventory; }
    }

    public static DataManager DataManager
    {
        get { return Instance._data; }
    }

    public static ShopManager ShopManager
    {
        get { return Instance._shop; }
    }

    public static GameStateManager GameStateManager
    {
        get { return Instance._gameState; }
    }


    public static QuestManager QuestManager
    {
        get { return Instance._quest; }
    }

    public static SoundManager SoundManager
    {
        get { return Instance._sound; }
    }


    public bool isInit = false;


    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    public void Init()
    {
        _obj = new ObjectManager();
        _pool = new PoolManager();
        _drop = new DropManager();
        _inventory = new InventoryManager();
        _data = new DataManager();
        _shop = new ShopManager();
        _gameState = new GameStateManager();
        _quest = new QuestManager();
        _sound = new SoundManager();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        isInit = true;
    }

    private void Start()
    {
        StartCoroutine(_quest.SetDefaultQuest());
    }
}