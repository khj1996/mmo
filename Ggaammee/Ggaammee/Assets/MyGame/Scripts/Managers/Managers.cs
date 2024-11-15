using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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


    private ObjectManager _obj;
    private DropManager _drop;
    private PoolManager _pool;

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

        Application.targetFrameRate = 60;
        isInit = true;
    }
}