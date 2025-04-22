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
    public int2 arraySize;
    public int girdSize;
    
    [SerializeField]
    public struct girdPoint
    {
        public bool isObstacle;
    }

    public void ToInit(int x, int y,int2 arraySize,int girdSize)
    {
        mapSize = new int2(x, y);
        this.arraySize = arraySize;
        this.girdSize = girdSize;
        mapPoint = new girdPoint[arraySize.x * arraySize.y];
        for (int i = 0; i < arraySize.x; i++)
        {
            for (int j = 0; j < arraySize.y; j++)
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
    public bool GetISObstacleInPoint(int2 point,bool isFromArrayConvert=true)
    {
        if (isFromArrayConvert)
        {
            return mapPoint[posIndexToArray(point)].isObstacle;
        }
       return mapPoint[CalculatePosToAarryIndex(point.x,point.y)].isObstacle;
    }
    private  int CalculatePosToAarryIndex(int posx, int posY)
    {
        return ((posx + mapSize.x / 2) / girdSize) + ((posY + mapSize.y / 2) / girdSize) * arraySize.x;
    }
    public void SetObstaclePoint(int2 point)
    {
        Debug.Log(arraySize);
        if (PathManager.Instance2.IsPositionInsideGrid(point))
        {
            int index = CalculatePosToAarryIndex(point.x, point.y);
            girdPoint gird = mapPoint[index];
            /*gird.isObstacle = !gird.isObstacle;*/
            gird.isObstacle = true;
            mapPoint[index] = gird;
        }
    }

    private int posIndexToArray(int x, int y)
    {
       
        return x + y * arraySize.x;
    }
    private int posIndexToArray(int2 point)
    {
        return posIndexToArray(point.x, point.y);
    }
}
