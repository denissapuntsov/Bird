using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public SoundItem currentSoundItem;
    public int Range => currentSoundItem.range;
}
