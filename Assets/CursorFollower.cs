using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    private const string TILE_TAG = "Tile";
    private AIPath _aiPath;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (!Physics.Raycast(rayOrigin, out hitInfo)) return;
        
        if (!hitInfo.collider.transform.parent.GetComponent<TileContainer>()) return;
        _aiPath.destination = hitInfo.point;
    }
}
