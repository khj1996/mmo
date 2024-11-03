using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    public PolygonCollider2D CameraClamp { get; private set; }

    public Action MapChangeAction;

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }


    bool[,] _collision;


    public async void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map_" + mapId.ToString("000");

        GameObject go = await Addressables.InstantiateAsync($"Prefabs/Map/{mapName}.prefab").ToUniTask();
        go.name = "Map";

        CurrentGrid = go.GetComponent<Grid>();
        CameraClamp = go.GetComponent<PolygonCollider2D>();

        TextAsset txt = Util.HandleAndRelease<TextAsset>($"Map/{mapName}.txt");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX;
        int yCount = MaxY - MinY;
        _collision = new bool[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1');
            }
        }

        MapChangeAction?.Invoke();
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}