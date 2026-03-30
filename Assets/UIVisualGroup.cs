using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class UIVisualGroup : MonoBehaviour
{
    [SerializeField] private List<MovingUIElement> objects = new List<MovingUIElement>();
    //[SerializeField] private List<GameObject> objects;
    //[SerializeField] private List<Vector3> offsets;
    //[SerializeField] private List<float> delays;
    
    private List<Transform> _startTransforms = new List<Transform>();
    private List<Tweener> _tweeners;
    private bool[] _areTweenersDone;
    private Action _onComplete;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _tweeners = new List<Tweener>();
        
        foreach (MovingUIElement element in objects)
        {
            _startTransforms.Add(element.gameObject.transform);
            var totalOffset = element.offset * (1 + element.delay);
            element.gameObject.transform.position -= totalOffset;
            CreateSequence(element, totalOffset);
        }
    }

    public void Open()
    {
        foreach (Tweener tweener in _tweeners)
        {
            tweener.PlayForward();
        }
    }

    private void CreateSequence(MovingUIElement element, Vector3 totalOffset)
    {
        Tweener newTweener;
        newTweener =
            element.gameObject.transform
                .DOBlendableMoveBy(totalOffset, element.speed)
                .SetSpeedBased(true)
                .SetUpdate(true).SetAutoKill(false);
        _tweeners.Add(newTweener);
    }

    public void Close(Action onComplete)
    {
        _onComplete = onComplete;
        _areTweenersDone = new bool[objects.Count];

        foreach (Tweener tweener in _tweeners)
        {
            tweener.PlayBackwards();
            tweener.OnRewind(() => { OnTweenerDone(_tweeners.IndexOf(tweener)); });
        }
    }

    private void OnTweenerDone(int index)
    {
        _areTweenersDone[index] = true;
        if (!_areTweenersDone.Contains(false))
        {
            _onComplete.Invoke();
        }
    }
}

[System.Serializable]
public struct MovingUIElement
{
    public GameObject gameObject;
    public Vector3 offset;
    public float speed;
    public float delay;
}
