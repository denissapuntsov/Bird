using UnityEngine;

public class CueIndicator : MonoBehaviour
{
    private bool _isMatched;
    [SerializeField] GameObject matched, unmatched;

    public bool IsMatched
    {
        get { return _isMatched; }
        set
        {
            _isMatched = value;
            matched.SetActive(value);
            unmatched.SetActive(!value);
        }
    }
}
