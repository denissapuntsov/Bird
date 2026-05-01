using System;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
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
