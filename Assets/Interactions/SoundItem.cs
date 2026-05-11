using UnityEngine;

[CreateAssetMenu(fileName = "SoundItem", menuName = "Scriptable Objects/SoundItem")]
public class SoundItem : ScriptableObject
{
    public int range = 1;
    public AudioClip clip;
}
