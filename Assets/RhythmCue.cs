using System;
using UnityEngine;

[Serializable]
public class Cue
{
    public CueKey key;
    public string label;
    public int positionInSamples;
    public float positionInSeconds;

    [HideInInspector] public bool isMatched;

    public Cue(string label, int positionInSamples, int sampleRate)
    {

        if (!Enum.IsDefined(typeof(CueKey), label.ToUpper()))
        {
            this.key = CueKey.None;
        }
        else this.key = (CueKey)Enum.Parse(typeof(CueKey), label.ToUpper());
        
        this.label = label;
        this.positionInSamples = positionInSamples;
        positionInSeconds = (float)positionInSamples / sampleRate;
    }

    public override bool Equals(object other)
    {
        if (other is not Cue)
        {
            return ReferenceEquals(this, other);
        }
        return Equals((Cue)other);
    }

    public bool Equals(Cue other)
    {
        return key == other.key && positionInSamples == other.positionInSamples;
    }
}

public enum CueKey
{
    W,
    A,
    S,
    D,
    None
}