using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class TileContainer : MonoBehaviour
{
    public Tile tile;
    public GameObject tileChild;
    public TileType tileType;
    public UnityEvent onTileFree;

    public MovingCharacter Owner
    {
        get => _owner;
        set
        {
            _owner = value;
            if (!value)
            {
                onTileFree?.Invoke();
            }
        }
    }
    private MovingCharacter _owner;

    public Vector3 WorldPosition => transform.position;
    public Vector3 GridPosition =>
        new (
            transform.localPosition.x / 2.5f,
            transform.localPosition.y / 2.5f,
            transform.localPosition.z / 2.5f
            );
    public Dictionary<Vector3, TileContainer> Neighbors { get; set; }

    private void OnDisable()
    {
        onTileFree.RemoveAllListeners();
    }
}

public enum TileType
{
    None,
    Ground,
    Water,
    Grass
}
