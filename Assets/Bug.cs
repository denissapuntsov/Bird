using Pathfinding;
using UnityEngine;

public class Bug : MonoBehaviour
{
    [SerializeField] private Transform target;
    private IAstarAI _ai;

    private void OnEnable()
    {
        _ai = GetComponent<IAstarAI>();
    }
    
    public void RunAway()
    {
        if (_ai == null) return;
        _ai.destination = target.position;
    }
}
