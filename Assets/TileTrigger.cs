using UnityEngine;
using UnityEngine.Events;

public class TileTrigger : MonoBehaviour
{
    public UnityEvent<MovingCharacter> onMovingCharacterEnter;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || !other.GetComponent<MovingCharacter>()) return;
        var character = other.gameObject.GetComponent<MovingCharacter>();
        Debug.Log("Invoking");
        onMovingCharacterEnter?.Invoke(character);
    }
}
