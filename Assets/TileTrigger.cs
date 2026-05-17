using System;
using UnityEngine;
using UnityEngine.Events;

public class TileTrigger : MonoBehaviour
{
    public UnityEvent<MovingCharacter> onCharacterEnter, onCharacterExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || !other.GetComponent<MovingCharacter>()) return;
        onCharacterEnter.Invoke(other.GetComponent<MovingCharacter>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || !other.GetComponent<MovingCharacter>()) return;
        onCharacterExit.Invoke(other.GetComponent<MovingCharacter>());
    }
}
