using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovingCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Tween _move;
    
    private TileContainer _occupiedTile;
    public TileContainer OccupiedTile
    {
        get => _occupiedTile;
        private set
        {
            if (_occupiedTile != null)
            {
                _occupiedTile.isOccupied = false;
            }
            _occupiedTile = value;
            _occupiedTile.isOccupied = true;
        }
    }

    private void Awake()
    {
        GetOccupiedTile();
    }

    private void Update()
    {
        GetOccupiedTile();
    }

    private void GetOccupiedTile()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down);
        RaycastHit hitInfo;
        
        if (!Physics.Raycast(ray, out hitInfo)) return;
        if (!hitInfo.collider.transform.parent.GetComponent<TileContainer>()) return;
        var hitTile = hitInfo.collider.transform.parent.GetComponent<TileContainer>();
        if (hitTile == OccupiedTile) return;
        OccupiedTile = hitTile;
    }

    public void StartPath(List<TileContainer> path, TileContainer goal) => StartPath(path, goal, null);
    public void StartPath(List<TileContainer> path, TileContainer goal, Action onCompletePath)
    {
        if (path == null) return;
        _move?.Kill();
        path[1].isOccupied = true;
        _move = transform
            .DOMove(path[1].WorldPosition, speed)
            .SetSpeedBased(true)
            .SetAutoKill()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (OccupiedTile == goal)
                {
                    onCompletePath?.Invoke();
                    return;
                }
                var nextPath = AStar.instance.CalculatePath(OccupiedTile, goal, out TileContainer reachableGoal);
                StartPath(nextPath, reachableGoal, onCompletePath);
            });
    }
}
