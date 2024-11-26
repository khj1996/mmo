using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[Serializable]
public struct StartItem
{
    public ItemData data;
    public int count;
}

public class Managers : MonoBehaviour
{
    #region 싱글톤

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

    #endregion


    [SerializeField] public List<StartItem> StartItemData;

    private ObjectManager _obj;
    private DropManager _drop;
    private PoolManager _pool;
    private InventoryManager _inventory;
    private DataManager _data;
    private ShopManager _shop;
    private GameStateManager _gameState;

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


    public bool isInit = false;


    void Awake()
    {
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

        Application.targetFrameRate = 60;
        isInit = true;
    }
}