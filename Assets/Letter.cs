using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{ 
    private bool _isMatched;

    public bool IsMatched
    {
        get => _isMatched;
        set
        {
            _isMatched = value;
            Debug.Log(_isMatched);
        }
    }
}
