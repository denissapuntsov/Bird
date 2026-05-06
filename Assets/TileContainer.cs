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
}

public enum TileType
{
    None,
    Ground,
    Water,
    Grass
}
