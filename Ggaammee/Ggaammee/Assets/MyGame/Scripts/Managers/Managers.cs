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

    public static ObjectManager ObjectManager
    {
        get { return Instance._obj; }
    }

    public bool isInit = false;


    void Start()
    {
        Init();
    }

    public void Init()
    {
        _obj = new ObjectManager();

        Application.targetFrameRate = 60;
        isInit = true;
    }
}