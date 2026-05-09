using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    private const string TILE_TAG = "Tile";
    private Character _character;

    private void Start()
    {
        _character = GetComponent<Character>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (!Physics.Raycast(rayOrigin, out hitInfo)) return;
        
        if (!hitInfo.collider.transform.parent.GetComponent<TileContainer>()) return;
        var hitTile = hitInfo.collider.transform.parent.GetComponent<TileContainer>();
        
        foreach (var tile in AStar.instance.CalculatePath(_character.OccupiedTile, hitTile))
        {
            Debug.Log(tile.GridPosition);
        }
        
    }
}
