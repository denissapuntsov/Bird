using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class MovingCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Tween _move;
    [HideInInspector] public UnityEvent<MovingCharacter, TileContainer> onLocationChangeStart;
    [HideInInspector] public UnityEvent<TileContainer> onLocationChangeEnd;
    
    private TileContainer _occupiedTile;
    public TileContainer OccupiedTile
    {
        get => _occupiedTile;
        private set
        {
            if (_occupiedTile)
            {
                _occupiedTile.Owner = null;
            }

            if (_hasJustStartedMoving)
            {
                onLocationChangeStart?.Invoke(this, _occupiedTile);
                _hasJustStartedMoving = false;
            }
            _occupiedTile = value;
            _occupiedTile.Owner = this;
        }
    }
    
    private bool _hasJustStartedMoving;

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
        var path = AStar.instance.CalculatePath(this, goal, out var reachableGoal);
        if (path != null)
        {
            _hasJustStartedMoving = true;
        }
        StartPath(path, goal, onCompletePath);
    }
    
    public void StartPath(List<TileContainer> path, TileContainer goal) => StartPath(path, goal, null);
    public void StartPath(List<TileContainer> path, TileContainer goal, Action onCompletePath)
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
                onLocationChangeEnd?.Invoke(OccupiedTile);
                if (OccupiedTile == goal)
                {
                    onCompletePath?.Invoke();
                    return;
                }
                var nextPath = AStar.instance.CalculatePath(this, goal, out TileContainer reachableGoal);
                StartPath(nextPath, reachableGoal, onCompletePath);
            });
    }
}
