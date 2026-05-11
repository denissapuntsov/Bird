using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public static AStar instance;
    
    public float step = 2.5f;

    [SerializeField] private TileContainer tileStart, tileGoal;
    
    // key is GridPosition, value is TileContainer
    private Dictionary<Vector3, TileContainer> _tiles = new Dictionary<Vector3, TileContainer>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        Scan();
    }

    private void Scan()
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

    public List<TileContainer> CalculatePath(TileContainer start, TileContainer goal, out TileContainer reachableGoal)
    {
        TileContainer tileClosestToGoal = start;
        start = _tiles[start.GridPosition];
        goal = _tiles[goal.GridPosition];
        
        var openList = new List<TileContainer> { start };
        var closedList = new List<TileContainer>();
        
        Dictionary<TileContainer, float> gScoreMap = new Dictionary<TileContainer, float> { [start] = 0 };
        Dictionary<TileContainer, float> hScoreMap = new Dictionary<TileContainer, float> { [start] = GetManhattanDistance(start, goal) };
        
        /*var gScore = new Dictionary<TileContainer, float> { [start] = 0 };
        var hScore = new Dictionary<TileContainer, float> { [start] = GetManhattanDistance(start, goal) };*/
        
        var parentMap = new Dictionary<TileContainer, TileContainer>();

        var current = openList[0];
        
        while (openList.Count > 0)
        { 
            // find tile with lowest fCost
            current = openList[0];
            foreach (var tile in openList)
            {
                if (!gScoreMap.ContainsKey(tile) || !hScoreMap.ContainsKey(tile)) continue;
                if (hScoreMap[tile] + gScoreMap[tile] < hScoreMap[start] + gScoreMap[start])
                {
                    current = tile;
                }
                if (GetManhattanDistance(tile, goal) <= GetManhattanDistance(tileClosestToGoal, goal))
                {
                    tileClosestToGoal = tile;
                }
            }

            // once the goal is reached, reconstruct the map
            if (current == goal)
            {
                reachableGoal = goal;
                return ReconstructMap(parentMap, current);
            }
            
            openList.Remove(current);
            closedList.Add(current);
            
            foreach (var neighbor in current.Neighbors.Values)
            {
                if (neighbor.isOccupied) continue;
                if (closedList.Contains(neighbor)) continue;
                float tentativeGScore = gScoreMap[current] + GetManhattanDistance(current, neighbor);

                if (!gScoreMap.ContainsKey(neighbor) || tentativeGScore < gScoreMap[neighbor])
                {
                    // cost of already travelled tiles
                    gScoreMap[neighbor] = tentativeGScore;
                    // cost of travelling from the neighbor to the goal
                    hScoreMap[neighbor] = GetManhattanDistance(neighbor, goal);

                    parentMap[neighbor] = current;

                    if (openList.Contains(neighbor)) continue;
                    openList.Add(neighbor);
                }
            }
        }

        if (start == tileClosestToGoal)
        {
            reachableGoal = null;
            return null;
        }
        var bestPath = CalculatePath(start, tileClosestToGoal, out reachableGoal);
        return bestPath;
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
    
    public float GetManhattanDistance(TileContainer current, TileContainer goal)
    {
        return Mathf.Abs(current.GridPosition.x - goal.GridPosition.x) + Mathf.Abs(current.GridPosition.z - goal.GridPosition.z);
    }
}
