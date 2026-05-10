using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFollower : MonoBehaviour
{
    private MovingCharacter _movingCharacter;
    public CharacterPath targets;
    private Action _onReachTarget;
    private int _currentTargetIndex = 0;
    private bool _isReversed = false;

    private void Awake()
    {
        _movingCharacter = GetComponent<MovingCharacter>();
        _onReachTarget += GoToNextTarget;
    }

    private void Start()
    {
        MoveAlongPath(targets, MovementType.Simple);
    }

    private void GoToNextTarget()
    {
        switch (targets.movementType)
        {
            case MovementType.Simple:
                if (_currentTargetIndex == targets.Length - 1)
                {
                    Debug.Log("Reached end of path");
                    return;
                }
                _currentTargetIndex++;
                break;
            case MovementType.Wander:
                var newIndex = _currentTargetIndex;
                while (newIndex == _currentTargetIndex)
                {
                    newIndex = Random.Range(0, targets.Length);
                }
                _currentTargetIndex = newIndex;
                break; 
            case MovementType.Patrol:
                if (_currentTargetIndex == targets.Length - 1)
                {
                    _isReversed = true;
                }
                else if (_currentTargetIndex == 0 && _isReversed)
                {
                    _isReversed = false;
                }
                _currentTargetIndex = _isReversed ? _currentTargetIndex - 1 : _currentTargetIndex + 1;
                break;
        }
        Debug.Log(_currentTargetIndex);
        MoveToTargetOnPath(targets, _currentTargetIndex);
    }
    
    public void MoveAlongPath(CharacterPath newTargets, MovementType movementType)
    {
        _isReversed = false;
        MoveToTargetOnPath(newTargets, _currentTargetIndex);
    }

    private void MoveToTargetOnPath(CharacterPath characterPath, int targetIndex)
    {
        var path = AStar.instance.CalculatePath(_movingCharacter.OccupiedTile, characterPath[targetIndex], out var goal);
        _movingCharacter.StartPath(path, goal, _onReachTarget);
    }
}

[Serializable]
public class CharacterPath
{
    public MovementType movementType;
    public List<TileContainer> targets;
    public TileContainer this[int index] => targets[index];
    public int Length => targets.Count;
}

public enum MovementType
{
    Simple,
    Patrol,
    Wander
}