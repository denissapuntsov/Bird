using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RhythmData", menuName = "Scriptable Objects/RhythmData")]
public class RhythmData : ScriptableObject
{
    public AudioClip audioClip;
    public List<CuePoint> cuePoints;
}
