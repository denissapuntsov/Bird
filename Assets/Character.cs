using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    private TileContainer _occupiedTile;
    public TileContainer OccupiedTile
    {
        get => _occupiedTile;
        private set
        {
            _occupiedTile.isOccupied = false;
            _occupiedTile = value;
            _occupiedTile.isOccupied = true;
        }
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down);
        RaycastHit hitInfo;
        
        if (!Physics.Raycast(ray, out hitInfo)) return;
        if (!hitInfo.collider.transform.parent.GetComponent<TileContainer>()) return;
        var hitTile = hitInfo.collider.transform.parent.GetComponent<TileContainer>();
        if (hitTile == OccupiedTile) return;
        OccupiedTile = hitTile;
    }
}
