using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CursorFollower : MonoBehaviour
{
    private const string TILE_TAG = "Tile";
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
        
        var path = AStar.instance.CalculatePath(_character.OccupiedTile, hitTile);
        StartPath(path, hitTile);
    }

    private void StartPath(List<TileContainer> path, TileContainer goal)
    {
        _move?.Kill();
        _move = transform
            .DOMove(path[1].WorldPosition, 3f)
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
                var nextPath = AStar.instance.CalculatePath(_character.OccupiedTile, goal);
                StartPath(nextPath, goal);
            });
    }
}
