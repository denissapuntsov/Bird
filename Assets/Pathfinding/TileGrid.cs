using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class TileGrid : MonoBehaviour
{
    public float tileSize = 2.5f;
    [NonSerialized] List<TileContainer> tiles;
    public Dictionary<Vector3, TileContainer> positions = new Dictionary<Vector3, TileContainer>();

    public TileContainer testA, testB;

    private void Start()
    {
        Scan();
    }

    public void Scan()
    {
        tiles = GetComponentsInChildren<TileContainer>().ToList();
        positions = new Dictionary<Vector3, TileContainer>();
        foreach (var tile in tiles)
        {
            bool canAdd = positions.TryAdd(WorldToGridPosition(tile.transform.localPosition), tile);
            if (canAdd)
            {
                tile.GridPosition = WorldToGridPosition(tile.transform.localPosition);
            }
            else
            {
                Debug.LogWarning("Two tiles cannot exist at the same location!");
            }
        }
    }

    public Vector3 WorldToGridPosition(Vector3 localPosition)
    {
        return new Vector3(
            localPosition.x / (tileSize != 0 ? tileSize : 1), 
            localPosition.y / (tileSize != 0 ? tileSize : 1), 
            localPosition.z / (tileSize != 0 ? tileSize : 1));
    }

    public int GetManhattanDistance(Vector3 a, Vector3 b)
    {
        a = WorldToGridPosition(a);
        b = WorldToGridPosition(b);
        return (int)Mathf.Abs(a.x - b.x) + (int)Mathf.Abs(a.z - b.z);
    }
}
