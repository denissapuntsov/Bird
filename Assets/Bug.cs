using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(AIDestinationSetter))]

public class Bug : MonoBehaviour
{
    [SerializeField] private Transform targetIdle, targetEnd;

    private bool _isSpooked = false;

    private AIDestinationSetter _aiDestinationSetter;
    private AIPath _aiPath;
    
    private void Start()
    {
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiPath = GetComponent<AIPath>();
    }

    public void ToggleSpook()
    {
        if (_isSpooked) return;
       _aiDestinationSetter.target = targetEnd;

       StartCoroutine(WaitForDestinationReached());
        
        _isSpooked = true;
    }

    private IEnumerator WaitForDestinationReached()
    {
        while (!_aiPath.reachedDestination) yield return null;
        _aiDestinationSetter.target = null;
        _aiPath.enabled = false;
    }
}
