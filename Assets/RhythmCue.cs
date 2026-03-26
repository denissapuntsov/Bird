using System;
using UnityEngine;

[Serializable]
public class Cue
{
    public CueKey key;
    public int position;
    public CueState state = CueState.New;
    
    [HideInInspector] public bool isMatched;
    [HideInInspector] public bool isOverlapping;

    public Cue(CueKey key, int position, int sampleRate)
    {
        this.key = key;
        this.position = position;
    }

    public Cue(Cue other)
    {
        key = other.key;
        position = other.position;
        state = other.state;
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (other is not Cue)
        {
            return ReferenceEquals(this, other);
        }
        return Equals((Cue)other);
    }

    public bool Equals(Cue other)
    {
        if (other == null) return false;
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

public enum CueState
{
    New,
    Saved,
    Modified
}