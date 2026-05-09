using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class TileContainer : MonoBehaviour
{
    public Tile tile;
    public GameObject tileChild;
    public TileType tileType;
    public bool isOccupied = false;

    public Vector3 WorldPosition => transform.position;
    public Vector3 GridPosition =>
        new (
            transform.localPosition.x / 2.5f,
            transform.localPosition.y / 2.5f,
            transform.localPosition.z / 2.5f
            );
    public Dictionary<Vector3, TileContainer> Neighbors { get; set; }
}

public enum TileType
{
    None,
    Ground,
    Water,
    Grass
}
