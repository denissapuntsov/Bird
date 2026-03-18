using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NAudio.Wave;
using UnityEditor;

public class AudioFileAnalyser : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    private string _outputFile, _inputFile;

    [SerializeField] private List<Cue> cues;
    [SerializeField] private List<Cue> cuesToAdd;

    public void UpdateCues() => UpdateCues(false);
    
    private void UpdateCues(bool addFromList)
    {
        _inputFile = AssetDatabase.GetAssetPath(audioClip);
        _outputFile = "Assets/temp.wav";

        using (CueWaveFileReader reader = new CueWaveFileReader(_inputFile))
        {
            using (CueWaveFileWriter writer = new CueWaveFileWriter(_outputFile, reader.WaveFormat))
            {
                byte[] buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }

                if (reader.Cues != null)
                {
                    for (int i = 0; i < reader.Cues.Count; i++)
                    {
                        writer.AddCue(reader.Cues[i].Position, reader.Cues[i].Label);
                    }
                }

                writer.Flush();
                writer.Close();
            }
        }

        using (CueWaveFileReader reader = new CueWaveFileReader(_outputFile))
        {
            using (CueWaveFileWriter writer = new CueWaveFileWriter(_inputFile, reader.WaveFormat))
            {
                byte[] buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }

                if (reader.Cues != null)
                {
                    for (int i = 0; i < reader.Cues.Count; i++)
                    {
                        writer.AddCue(reader.Cues[i].Position, reader.Cues[i].Label);
                    }
                }

                if (addFromList && cuesToAdd.Count > 0)
                {
                    foreach (var cue in cuesToAdd)
                    {
                        if ((float)cue.positionInSamples / reader.WaveFormat.SampleRate > audioClip.length)
                        {
                            cue.positionInSamples = (int)(reader.WaveFormat.SampleRate * audioClip.length);
                        }
                        writer.AddCue(cue.positionInSamples, cue.label);
                    }

                    cuesToAdd.Clear();
                }
                
                writer.Flush();
                writer.Close();
            }
        }
        
        File.Delete(_outputFile);
        
        AssetDatabase.Refresh();
        
        UpdateCueList();
    }

    public void AddCuesFromList() => UpdateCues(true);

    private void UpdateCueList()
    {
        cues = new List<Cue>();
        using (CueWaveFileReader reader = new CueWaveFileReader(AssetDatabase.GetAssetPath(audioClip)))
        {
            if (reader.Cues != null)
            {
                for (int i = 0; i < reader.Cues.Count; i++)
                {
                    cues.Add(new Cue(reader.Cues[i].Label, reader.Cues[i].Position, reader.WaveFormat.SampleRate));
                }
            }
        }
        
        UpdateScriptableObject();
    }

    private void UpdateScriptableObject()
    {
        var newAudioData = ScriptableObject.CreateInstance<RhythmData>();
        newAudioData.audioClip = audioClip;
        newAudioData.cuePoints = new List<Cue>();
        foreach (var cue in cues)
        {
            newAudioData.cuePoints.Add(cue);
        }
        
        AssetDatabase.CreateAsset(newAudioData, $"Assets/Analysis/CuePoints/{audioClip.name}_CuePoints.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void RemoveAllCues()
    {
        using (CueWaveFileReader reader = new CueWaveFileReader(_inputFile))
        {
            using (CueWaveFileWriter writer = new CueWaveFileWriter(_outputFile, reader.WaveFormat))
            {
                byte[] buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }

                writer.Flush();
                writer.Close();
            }
        }
        
        using (CueWaveFileReader reader = new CueWaveFileReader(_outputFile))
        {
            using (CueWaveFileWriter writer = new CueWaveFileWriter(_inputFile, reader.WaveFormat))
            {
                byte[] buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, read);
                }
                
                writer.Flush();
                writer.Close();
            }
        }
        
        File.Delete(_outputFile);

        UpdateCues();
        
        AssetDatabase.Refresh();
        
        UpdateScriptableObject();
    }
}

/*[Serializable]
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
}*/
