using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OldMovingCharacter : MonoBehaviour
{
    /*public List<CharacterPath> paths;
    private AIPath _aiPath;

    private CharacterPath _currentPath;
    private int _targetIndex = 0;
    private bool _isReversed = false;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _aiPath.enableRotation = true;
        StartPath(0);
    }

    private void Update()
    {
        Move();
    }

    private void StartPath(int index)
    {
        _aiPath.enabled = true;
        _currentPath = paths[index];
        _aiPath.destination = _currentPath[0].destination;
    }

    private void StopPath()
    {
        _aiPath.enabled = false;
    }

    private void Move()
    {
        if (!_aiPath.reachedDestination || _currentPath == null) return;
        _aiPath.destination = GetNextTarget(_currentPath.movementType);
    }

    private Vector3 GetNextTarget(MovementType movementType)
    {
        switch (movementType)
        {
            case MovementType.Simple:
                if (_targetIndex < _currentPath.targets.Count - 1)
                {
                    _targetIndex++;
                }
                else
                {
                    StopPath();
                    return transform.position;
                }
                return _currentPath[_targetIndex].destination;
            case MovementType.Patrol:
                if (_targetIndex == _currentPath.targets.Count - 1)
                {
                    _isReversed = true;
                }
                else if (_targetIndex == 0 && _isReversed)
                {
                    _isReversed = false;
                }
                _targetIndex = _isReversed ? _targetIndex - 1 : _targetIndex + 1;
                Debug.Log(_targetIndex);
                return _currentPath[_targetIndex].destination;
            case MovementType.Wander:
                return _currentPath[Random.Range(0, _currentPath.targets.Count)].destination;
        }
        return transform.position;
    }*/
}

/*
[Serializable]
/*public class CharacterPath
{
    public MovementType movementType;
    public List<CharacterTarget> targets;

    public CharacterTarget this[int index] => this.targets[index];
}#1#

[Serializable]
public class CharacterTarget
{
    public Vector3 destination = new(5, 0, 0);
}
*/
