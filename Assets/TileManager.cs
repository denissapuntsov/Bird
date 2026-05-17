using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public UnityEvent<TileContainer> onTileFree;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<TileContainer, MovingCharacter> owners = new Dictionary<TileContainer, MovingCharacter>();

    public void Add(MovingCharacter character)
    {
        owners.Add(character.OccupiedTile, character);
    }

    public void Remove(TileContainer tile)
    {
        if (owners.Remove(tile))
        {
            owners.Remove(tile);
            onTileFree.Invoke(tile);
        }
    }

    public bool IsTileFree(TileContainer tile)
    {
        return !owners.ContainsKey(tile);
    }

    public MovingCharacter GetTileOwner(TileContainer tile)
    {
        if (!owners.ContainsKey(tile)) return null;
        return owners[tile];
    }
}
