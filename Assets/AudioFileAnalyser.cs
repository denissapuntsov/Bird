using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioFileAnalyser : MonoBehaviour
{
    public AudioClip audioClip;
    [SerializeField] private List<CuePoint> cuePoints;
    
    public void Analyse()
    {
        cuePoints = new List<CuePoint>();
        string filePath = AssetDatabase.GetAssetPath(audioClip);
        var audioFile = File.Open(filePath, FileMode.Open);
        BinaryReader reader = new BinaryReader(audioFile);

        reader.ReadBytes(24);
        float sampleRate = reader.ReadUInt16();
        
        byte[] nextChunk = reader.ReadBytes(2);
        while (Encoding.ASCII.GetString(nextChunk) != "cue ")
        {
            nextChunk = reader.ReadBytes(4);
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                Debug.Log("Nothing found.");
                reader.Close();
                return;
            }
        }

        long chunkDataSize = reader.ReadUInt32();
        long dwCuePoints = reader.ReadUInt32();

        for (int i = 0; i < dwCuePoints; i++)
        {
            var name = reader.ReadUInt32();
            var position = reader.ReadUInt32();
            var positionInSeconds = position / sampleRate;
            reader.ReadBytes(16); // skip chunk id, chunk start, block start and sample offset, each 4 bytes long
            CuePoint newCuePoint = new CuePoint(name, position, positionInSeconds);
            cuePoints.Add(newCuePoint);
            Debug.Log("Marker " + name + " at " + positionInSeconds + " s");
        }
        reader.Close(); 
    }
}

[System.Serializable]
public class CuePoint
{
    public string cueMarkerName;
    public int positionInSamples;
    public float positionInSeconds;

    public CuePoint(uint name, uint position, float positionInSeconds)
    {
        this.cueMarkerName = name.ToString();
        this.positionInSamples = (int)position;
        this.positionInSeconds = positionInSeconds;
    }
}