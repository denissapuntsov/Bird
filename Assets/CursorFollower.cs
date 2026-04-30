using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    private const string TILE_TAG = "Tile";
    private AIPath _aiPath;
    private RaycastHit _hitInfo;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (!_hitInfo.collider.transform.parent.CompareTag(TILE_TAG)) return;
        _aiPath.destination = _hitInfo.point;
    }
}
