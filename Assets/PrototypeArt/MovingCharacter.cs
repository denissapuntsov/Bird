using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class MovingCharacter : MonoBehaviour
{
    public List<CharacterPath> paths;
    private AIPath _aiPath;

    private CharacterPath _currentPath;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _aiPath.enableRotation = true;
        TraversePath(0);
    }

    private void Update()
    {
        if (_currentPath == null) return;
        if (_aiPath.reachedDestination)
        {
            _currentPath = null;
        }
    }

    public void OnAcceptSound()
    {
        
    }

    public void OnRejectSound()
    {
        
    }

    public void TraversePath(int index)
    {
        CharacterPath path = new CharacterPath();
        if (paths.Count <= 0)
        {
            Debug.LogWarning($"No paths are set for {gameObject.name}");
            return;
        }
        if (paths.Count < index)
        {
            path = paths[^1];
            Debug.LogWarning($"No path at index {index} for {gameObject.name}. Using Path {paths.Count - 1}");
        }
        if (index < 0)
        {
            path = paths[0];
            Debug.LogWarning($"Requested path index is a negative integer. Using Path 0");
        }
        else
        {
            path = paths[index];
        }
    }

    private IEnumerator TraversePath(CharacterPath path)
    {
        while (path.pathType == PathType.Simple)
        {
            foreach (var target in path.targets)
            {
                _aiPath.destination = target.destination;
                while (!_aiPath.reachedDestination) yield return null;
            }
            // stop the coroutine
        }
        while (path.pathType == PathType.Wander)
        {
            _aiPath.destination = path.targets[Random.Range(0, path.targets.Count)].destination;
            while (!_aiPath.reachedDestination) yield return null;
        }
    }
    
}

[Serializable]
public class CharacterPath
{
    public PathType pathType;
    public List<CharacterTarget> targets;
}

[Serializable]
public class CharacterTarget
{
    public Vector3 destination = new(5, 0, 0);
}

public enum PathType
{
    Simple,
    Patrol,
    Wander
}
