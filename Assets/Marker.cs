using System;
using UnityEngine;

[System.Serializable]
public class Marker : IEquatable<Marker>
{
    public int time;
    public string key;
    public bool isMatched;

    public Marker(long newTime, string newKey)
    {
        time = (int)newTime;
        key = newKey;
        isMatched = false;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Tuple<int, string> objAsTuple = obj as Tuple<int, string>;
        if (objAsTuple == null) return false;
        else return Equals(objAsTuple);
    }

    public bool Equals(Marker other)
    {
        if (other == null) return false;
        return this.time == other.time && this.key == other.key;
    }
}
