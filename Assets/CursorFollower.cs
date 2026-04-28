using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
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

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            _aiPath.destination = hitInfo.point;
        }
    }
}
