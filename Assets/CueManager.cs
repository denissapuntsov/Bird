using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NAudio.Wave;
using UnityEditor;

public class CueManager : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    private CueList _cueList;
    public List<Cue> cues;
    private string _filePath;
    private int _sampleRate;

    public void GetCues()
    {
        _filePath = AssetDatabase.GetAssetPath(audioClip);
        var fs = File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite);

        using (WaveFileReader waveFileReader = new WaveFileReader(fs))
        {
            _sampleRate = waveFileReader.WaveFormat.SampleRate;
        }
        fs.Position = 0;
        fs.Close();
        
        fs = File.Open(_filePath, FileMode.Open, FileAccess.Read);

        CueWaveFileReader reader = new CueWaveFileReader(fs);
        foreach (var chunk in reader.ExtraChunks)
        {
            Debug.Log(chunk.IdentifierAsString);
            Debug.Log(chunk.StreamPosition);
        }

        
        
        cues = new List<Cue>();
        if (reader.Cues == null)
        {
            Debug.Log("No cues found.");
            reader.Close();
            fs.Close();
            return;
        }
        
        for (int i = 0; i < reader.Cues.Count; i++)
        {
            var nextCue = reader.Cues[i];
            cues.Add(new Cue(nextCue.Label, nextCue.Position, _sampleRate));
        }

        reader.Close();
        fs.Close();

    }
}

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
        positionInSeconds = positionInSamples / (float)sampleRate;
    }
}
