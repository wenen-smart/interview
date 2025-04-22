using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private static PathManager instance2;
   /* public  int RoundDis = 1;*/
    public static PathManager Instance2
    {
        get
        {
            if (instance2 == null)
            {
                instance2 = GameObject.FindObjectOfType<PathManager>();
                Debug.Log(instance2==null);
            }
            return instance2;
        }
    }
    public List<int2> obstaclePoints = new List<int2>();
    public int MapWidth { get => mapWidth; set => mapWidth = value; }
    public int MapHeight { get => mapHeight; set => mapHeight = value; }
    public float3 OrginPos { get => orginPos; set => orginPos = value; }
    public Map map;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    public int2 MapSize { get => new int2(mapWidth, mapHeight); }
    [SerializeField] private float3 orginPos;
    [SerializeField] private GameObject target;
    public int girdSize;
    private void Start()
    {
        
        DynamicWriteMap();


    }

    public void setObstacle()
    {
        for (int i = 0; i < obstaclePoints.Count; i++)
        {
            map.SetObstaclePoint(obstaclePoints[i]);
        }
    }

    public void DynamicWriteMap()
    {
        InitMap();
        setObstacle();
    }
    private int posIndexToArray(int x, int y)
    {
        return x + y * mapWidth;
    }
    public bool IsPositionInsideGrid(int2 gridPosition)
    {
        return
           IsPositionInsideGrid(gridPosition, MapSize);
    }
    private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return
            gridPosition.x > -gridSize.x/2 &&
            gridPosition.y > -gridSize.y / 2 &&
            gridPosition.x < gridSize.x/2 &&
            gridPosition.y < gridSize.y/2;
    }

    public void InitMap()
    {
        /* Debug.Log(mapWidth);*/
        int arrayWidth = mapWidth / girdSize;
        int arrayHeight = mapHeight / girdSize;
        int2 arraySize = new int2(arrayWidth, arrayHeight);
        map.arraySize = arraySize;
        map.ToInit(mapWidth, mapHeight,arraySize,girdSize);

    }
}
