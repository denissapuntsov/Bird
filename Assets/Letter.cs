using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    [SerializeField] private GameObject matched, unmatched;

    public string key;
    
    private bool _isMatched;
}
