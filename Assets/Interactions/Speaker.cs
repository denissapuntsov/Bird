using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private bool isSpeaking = false;
    [SerializeField] private AK.Wwise.Event sound;

    public void TakeSound()
    {
        PlayerInventory.instance.currentVocalization = sound;
        Debug.Log("Took sound:" + sound.Name);
    }
}
