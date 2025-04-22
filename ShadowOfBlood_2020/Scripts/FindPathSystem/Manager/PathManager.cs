using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private static PathManager instance;
    public static PathManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PathManager>();
                Debug.Log(instance==null);
            }
            return instance;
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
    private void Start()
    {
        /* map = Map.Instance;*/

        map.ToInit(mapWidth, mapHeight);
        for (int i = 0; i < obstaclePoints.Count; i++)
        {
            map.SetObstaclePoint(obstaclePoints[i]);
        }
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

        map.ToInit(mapWidth, mapHeight);

    }
}
