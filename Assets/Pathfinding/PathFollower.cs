using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFollower : MonoBehaviour
{
    private MovingCharacter _movingCharacter;
    public List<CharacterPath> paths = new List<CharacterPath>();
    private CharacterPath _activePath = new CharacterPath();
    private Action _onReachTarget;
    private int _currentTargetIndex = 0;
    private bool _isReversed = false;
    
    [HideInInspector] public bool moveOnStart = false;
    [HideInInspector] public int startPathIndex = 0;

    private void Awake()
    {
        _movingCharacter = GetComponent<MovingCharacter>();
        _onReachTarget += GoToNextTarget;
    }

    private void Start()
    {
        if (!moveOnStart) return;
        MoveAlongPath(startPathIndex);
    }

    private void GoToNextTarget()
    {
        switch (_activePath.movementType)
        {
            case MovementType.Simple:
                if (_currentTargetIndex == _activePath.Length - 1)
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
                    newIndex = Random.Range(0, _activePath.Length);
                }
                _currentTargetIndex = newIndex;
                break; 
            case MovementType.Patrol:
                if (_currentTargetIndex == _activePath.Length - 1)
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
        MoveToTargetOnPath(_activePath, _currentTargetIndex);
    }
    
    public void MoveAlongPath(int indexOfPath)
    {
        if (indexOfPath < 0 || indexOfPath >= paths.Count) return;
        _activePath = paths[indexOfPath];
        _isReversed = false;
        MoveToTargetOnPath(_activePath, _currentTargetIndex);
    }

    private void MoveToTargetOnPath(CharacterPath characterPath, int targetIndex)
    {
        _movingCharacter.Move(characterPath[targetIndex], _onReachTarget);
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