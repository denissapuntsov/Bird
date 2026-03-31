using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/Audio Data")]
public class AudioData : ScriptableObject
{
    public AudioClip audioClip;
    public Sprite sprite;
}
