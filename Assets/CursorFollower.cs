using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    private MovingCharacter _movingCharacter;

    private void Start()
    {
        _movingCharacter = GetComponent<MovingCharacter>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (!Physics.Raycast(rayOrigin, out hitInfo)) return;
        if (!hitInfo.collider.transform.parent.GetComponent<TileContainer>()) return;
        var hitTile = hitInfo.collider.transform.parent.GetComponent<TileContainer>();
        if (hitTile == _movingCharacter.OccupiedTile) return;
        
        var path = AStar.instance.CalculatePath(_movingCharacter.OccupiedTile, hitTile, out TileContainer reachableGoal);
        _movingCharacter.StartPath(path, reachableGoal);
    }
}
