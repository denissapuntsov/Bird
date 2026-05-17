using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public static AStar instance;
    
    public float step = 2.5f;

    private TileContainer _tileStart, _tileGoal;
    
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

    public List<TileContainer> GetNeighborsInRange(TileContainer origin, int range)
    {
        List<TileContainer> neighbors = new List<TileContainer>();
            
        foreach (var tileKvp in _tiles)
        {
            if (tileKvp.Value == origin) continue;
            if (GetManhattanDistance(tileKvp.Value, origin) <= range)
            {
                neighbors.Add(tileKvp.Value);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Returns list of TileContainers that form a path from a MovingCharacter's current Occupied Tile to the goal Tile (or its "closest" [see reachableGoal] alternative).
    /// </summary>
    /// <param name="character"> MovingCharacter to move along path. Start Tile is set to the character's OccupiedTile. </param>
    /// <param name="goal"> Desired end Tile for the path. </param>
    /// <param name="reachableGoal"> Desired goal if the path is unobstructed; otherwise returns the path to the Tile with the lowest Manhattan distance to the desired goal. </param>
    /// <returns></returns>
    public List<TileContainer> CalculatePath(MovingCharacter character, TileContainer goal, out TileContainer reachableGoal)
    {
        var start = character.OccupiedTile;
        TileContainer tileClosestToGoal = start;
        start = _tiles[start.GridPosition];
        goal = _tiles[goal.GridPosition];
        
        var openList = new List<TileContainer> { start };
        var closedList = new List<TileContainer>();
        
        Dictionary<TileContainer, float> gScoreMap = new Dictionary<TileContainer, float> { [start] = 0 };
        Dictionary<TileContainer, float> hScoreMap = new Dictionary<TileContainer, float> { [start] = GetManhattanDistance(start, goal) };
        
        var parentMap = new Dictionary<TileContainer, TileContainer>();
        
        while (openList.Count > 0)
        { 
            // find tile with lowest fCost
            var current = openList[0];
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
                // same as isOccupied, WIP 
                if (TileManager.instance.GetTileOwner(neighbor) != character && TileManager.instance.GetTileOwner(neighbor) != null) continue;
                
                if (closedList.Contains(neighbor)) continue;
                float tentativeGScore = gScoreMap[current] + GetManhattanDistance(current, neighbor);

                if (!gScoreMap.ContainsKey(neighbor) || tentativeGScore < gScoreMap[neighbor])
                {
                    // cost of already traveled tiles
                    gScoreMap[neighbor] = tentativeGScore;
                    // cost of traveling from the neighbor to the goal
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
        var bestPath = CalculatePath(character, tileClosestToGoal, out reachableGoal);
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
