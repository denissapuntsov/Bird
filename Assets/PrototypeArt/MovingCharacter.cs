using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class MovingCharacter : MonoBehaviour
{
    public List<Target> targets;
    private AIPath _aiPath;

    private void Start()
    {
        _aiPath = GetComponent<AIPath>();
        _aiPath.enableRotation = true;
        MoveTo(0);
    }

    public void OnAcceptSound()
    {
        
    }

    public void OnRejectSound()
    {
        
    }
    public void MoveTo(int targetIndex)
    {
        if (targetIndex > targets.Count)
        {
            Debug.LogWarning($"Target index {targetIndex} exceeds the number of targets. Using target {targets.Count - 1} instead");
            targetIndex = targets.Count - 1;
        }
        else if (targetIndex < 0)
        {
            Debug.LogWarning($"Target index {targetIndex} is a negative integer. Using target 0 instead");
        }
        else if (targets.Count == 0)
        {
            Debug.LogWarning($"No targets found for {gameObject.name}");
            return;
        }
        
        _aiPath.destination = targets[targetIndex].destination;
    }

    public void MoveTo(string targetName)
    {
        var target = FindTargetByName(targetName);
        if (target == null)
        {
            Debug.LogWarning($"No target of name {targetName} found for {gameObject.name}");
            return;
        } 
        
        _aiPath.destination = target.destination;
    }

    private Target FindTargetByName(string targetName)
    {
        foreach (Target target in targets)
        {
            if (target.label == targetName)
            {
                return target;
            }
        }
        return null;
    }
}

[Serializable]
public class Target
{
    public string label = "New Target";
    public Vector3 destination = new(5, 0, 0);
    public UnityEvent onTargetReached = new UnityEvent();
}
