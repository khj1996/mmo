using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        Managers.Instance.StartCoroutine(AddCoroutine(info, myPlayer));
    }

    IEnumerator AddCoroutine(ObjectInfo info, bool myPlayer = false)
    {
        yield return new WaitUntil(() => !Managers.Scene.isLoading && Managers.UI.CurrentSceneUI.isInitComplete && Managers.UI.CurrentSceneUI.SceneName == "UI_GameScene");

        if (MyPlayer && MyPlayer.Id == info.ObjectId)
            yield break;
        if (_objects.ContainsKey(info.ObjectId))
            yield break;


        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if (objectType == GameObjectType.Player)
        {
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer.prefab");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = new PositionInfo()
                {
                    State = info.PosInfo.State,
                    Pos = info.PosInfo.Pos,
                    Move = new Vec2(),
                    LookDir = new Vec2(),
                };
                MyPlayer.Stat.MergeFrom(info.StatInfo);
                MyPlayer.UpdateExpBar();
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player.prefab");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.Stat.MergeFrom(info.StatInfo);
            }
        }
        else if (objectType == GameObjectType.Monster)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Monster.prefab");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PosInfo;
            mc.Stat = info.StatInfo;
        }
        else if (objectType == GameObjectType.Projectile)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Arrow.prefab");
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.PosInfo = info.PosInfo;
            ac.Stat = info.StatInfo;
            float rotZ = Mathf.Atan2(info.PosInfo.LookDir.Y, info.PosInfo.LookDir.X) * Mathf.Rad2Deg;
            ac.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);
        }
    }

    public void Remove(int id)
    {
        if (MyPlayer != null && MyPlayer.Id == id)
            return;
        if (_objects.ContainsKey(id) == false)
            return;

        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
        MyPlayer = null;
    }
}