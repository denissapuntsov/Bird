using System;

[Serializable]
public class Cue
{
    public string label;
    public int positionInSamples;
    public float positionInSeconds;

    public Cue(string label, int positionInSamples, int sampleRate)
    { 
        this.label = label; 
        this.positionInSamples = positionInSamples;
        positionInSeconds = (float)positionInSamples / sampleRate;
    }
}