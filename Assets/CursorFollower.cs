using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Character _character;
    private Tween _move;

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
        if (hitTile == _character.OccupiedTile) return;

        /*if (AStar.instance.GetManhattanDistance(_character.OccupiedTile, hitTile) > 4)
        {
            Debug.Log("Tile too far away!");
            return;
        }*/
        var path = AStar.instance.CalculatePath(_character.OccupiedTile, hitTile, out TileContainer reachableGoal);
        StartPath(path, reachableGoal);
    }

    private void StartPath(List<TileContainer> path, TileContainer goal)
    {
        if (path == null) return;
        _move?.Kill();
        _move = transform
            .DOMove(path[1].WorldPosition, speed)
            .SetSpeedBased(true)
            .SetAutoKill()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (_character.OccupiedTile == goal)
                {
                    Debug.Log("Destination reached");
                    return;
                }
                var nextPath = AStar.instance.CalculatePath(_character.OccupiedTile, goal, out TileContainer reachableGoal);
                StartPath(nextPath, reachableGoal);
            });
    }
}
