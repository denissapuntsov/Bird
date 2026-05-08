using System;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class TileContainer : MonoBehaviour
{
    public Tile tile;
    public GameObject tileChild;
    public TileType tileType;

    [SerializeField] private Vector3 _gridPosition;
    public Vector3 GridPosition
    {
        get =>  _gridPosition;
        set
        {
            _gridPosition = value;
            name = $"[{GridPosition.x}, {GridPosition.z}]";
        }
    }
    public float GCost = float.MaxValue;
    public float HCost;
    public float FCost => GCost + HCost;
    public TileContainer parent;
}

public enum TileType
{
    None,
    Ground,
    Water,
    Grass
}
