using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    [SerializeField] private GameObject matched, unmatched;

    public string key;
    
    private bool _isMatched;

    public bool IsMatched
    {
        get => _isMatched;
        set
        {
            _isMatched = value;
            matched.SetActive(_isMatched);
            unmatched.SetActive(!_isMatched);
            Debug.Log(_isMatched);
        }
    }
}
