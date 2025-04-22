using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct Map
{
    //     private static Map instance=new Map();
    //     public static Map Instance
    //     {
    //         get
    //         {
    //             return instance;
    //         }

    public girdPoint[] mapPoint;
/*    private int mapWidth;*/
    private int2 mapSize;
    [SerializeField]
    public struct girdPoint
    {
        public bool isObstacle;
    }

    public void ToInit(int x, int y)
    {
        mapSize = new int2(x, y);
        mapPoint = new girdPoint[x * y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                mapPoint[posIndexToArray(i, j)] = new girdPoint { isObstacle = false };
            }
        }
        //         Debug.Log("y");
        //         Debug.Log(mapPoint.Length);
    }
    public bool GetISObstacleInPoint(int index)
    {
        return mapPoint[index].isObstacle;
    }
    public bool GetISObstacleInPoint(int2 point)
    {
        return mapPoint[posIndexToArray(point)].isObstacle;
    }
    private  int CalculatePosToAarryIndex(int posx, int posY)
    {
        return (posx + mapSize.x / 2) + (posY + mapSize.y / 2) * mapSize.x;
    }
    public void SetObstaclePoint(int2 point)
    {
        if (PathManager.Instance.IsPositionInsideGrid(point))
        {
            int index = CalculatePosToAarryIndex(point.x, point.y);
            girdPoint gird = mapPoint[index];
            gird.isObstacle = !gird.isObstacle;
            gird.isObstacle = true;
            mapPoint[index] = gird;
        }
    }

    private int posIndexToArray(int x, int y)
    {
        return x + y * mapSize.x;
    }
    private int posIndexToArray(int2 point)
    {
        return posIndexToArray(point.x, point.y);
    }
}
