using System;
using UnityEngine;

[Serializable]
public class Cue
{
    public CueKey key;
    public int position;

    [HideInInspector] public bool isMatched;

    public Cue(CueKey key, int position, int sampleRate)
    {
        this.key = key;
        this.position = position;
    }

    public Cue(Cue other)
    {
        key = other.key;
        position = other.position;
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
        return key == other.key && position == other.position;
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