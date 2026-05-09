using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public float step = 2.5f;

    [SerializeField] private TileContainer tileStart, tileGoal;
    
    // key is GridPosition, value is TileContainer
    private Dictionary<Vector3, TileContainer> _tiles = new Dictionary<Vector3, TileContainer>();
    
    public float GetManhattanDistance(TileContainer current, TileContainer goal)
    {
        return Mathf.Abs(current.GridPosition.x - goal.GridPosition.x) + Mathf.Abs(current.GridPosition.z - goal.GridPosition.z);
    }

    private void Start()
    {
        SetupGrid();

        foreach (var tile in CalculatePath(tileStart, tileGoal))
        {
            Debug.Log(tile.GridPosition);
        }
    }

    private void SetupGrid()
    {
        foreach (TileContainer tile in GetComponentsInChildren<TileContainer>())
        {
            _tiles.Add(tile.GridPosition, tile);
        }

        foreach (var tile in _tiles.Values)
        {
            SetNeighbors(tile);
        }
    }

    private void SetNeighbors(TileContainer tile)
    {
        tile.Neighbors = new Dictionary<Vector3, TileContainer>();
        List<TileContainer> neighbors = new List<TileContainer>();
        List<Vector3> offsets = new List<Vector3>
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 0, 1),
            new(0, 0, -1)
        };
        foreach (Vector3 offset in offsets)
        {
            var keyToTest = new Vector3(tile.GridPosition.x + offset.x, tile.GridPosition.y + offset.y, tile.GridPosition.z + offset.z);
            if (_tiles.TryGetValue(keyToTest, out var nextNeighbor))
            {
                neighbors.Add(nextNeighbor);
            }
        }

        foreach (var neighbor in neighbors)
        {
            tile.Neighbors[neighbor.GridPosition] = neighbor;
        }
    }

    public List<TileContainer> CalculatePath(TileContainer start, TileContainer goal)
    {
        start = _tiles[start.GridPosition];
        goal = _tiles[goal.GridPosition];
        
        var openList = new List<TileContainer> { start };
        var closedList = new List<TileContainer>();

        start.gScore = 0;
        start.hScore = GetManhattanDistance(start, goal);
        
        /*var gScore = new Dictionary<TileContainer, float> { [start] = 0 };
        var hScore = new Dictionary<TileContainer, float> { [start] = GetManhattanDistance(start, goal) };*/
        
        var parentMap = new Dictionary<TileContainer, TileContainer>();

        while (openList.Count > 0)
        { 
            var current = openList[0];
            foreach (var tile in openList)
            {
                if (tile.FScore < start.FScore)
                {
                    current = tile;
                }
            }

            if (current == goal)
            {
                return ReconstructMap(parentMap, current);
            }
            
            openList.Remove(current);
            closedList.Add(current);
            
            foreach (var neighbor in current.Neighbors.Values)
            {
                if (closedList.Contains(neighbor)) continue;
                float tentativeGScore = current.gScore + GetManhattanDistance(current, neighbor);

                if (neighbor.gScore == 0 || tentativeGScore < neighbor.gScore)
                {
                    neighbor.gScore = tentativeGScore;
                    neighbor.hScore = GetManhattanDistance(neighbor, goal);

                    parentMap[neighbor] = current;

                    if (openList.Contains(neighbor)) continue;
                    openList.Add(neighbor);
                }
            }
        }
        Debug.LogWarning("No path found");
        return null;
    }

    private List<TileContainer> ReconstructMap(Dictionary<TileContainer, TileContainer> parentMap, TileContainer current)
    {
        var path = new List<TileContainer> { current };

        while (parentMap.ContainsKey(current))
        {
            current = parentMap[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
