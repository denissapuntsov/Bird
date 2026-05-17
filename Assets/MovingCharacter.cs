using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MovingCharacter : MonoBehaviour
{
    public UnityEvent<TileContainer> onTileReached;
    [SerializeField] private float speed = 5.0f;
    private Tween _move;
    
    private TileContainer _occupiedTile;
    public TileContainer OccupiedTile
    {
        get => _occupiedTile;
        private set
        {
            if (value == _occupiedTile) return;
            if (_occupiedTile)
            {
                TileManager.instance.Remove(_occupiedTile);
            }
            _occupiedTile = value;
            TileManager.instance.Add(this);
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
    
    public void Move(TileContainer goal) => Move(goal, null);
    
    public void Move(TileContainer goal, Action onCompletePath)
    {
        if (goal == OccupiedTile) return;
        var path = AStar.instance.CalculatePath(this, goal, out var reachableGoal);
        StartPath(path, goal, onCompletePath);
    }
    
    public void StartPath(List<TileContainer> path, TileContainer goal) => StartPath(path, goal, goal);
    public void StartPath(List<TileContainer> path, TileContainer goal, Action onCompletePath) => StartPath(path, goal, goal);
    private void StartPath(List<TileContainer> path, TileContainer goal, TileContainer reachableGoal)
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
                onTileReached?.Invoke(OccupiedTile);
                if (OccupiedTile == goal || OccupiedTile == reachableGoal)
                {
                    return;
                }
                var nextPath = AStar.instance.CalculatePath(this, goal, out reachableGoal);
                StartPath(nextPath, goal, reachableGoal);
            });
    }
}
