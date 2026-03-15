using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AudioFileAnalyser : MonoBehaviour
{
    private const int CUE_SIZE = 24;
    private const int CUE_CHUNK_SIZE = 12;
    
    public AudioClip audioClip;
    [SerializeField] private List<CuePoint> cuePoints;
    private List<CuePoint> _tempPoints;
    private string _filePath;
    private long _chunkDataSize, _dwCuePoints;
    private float _sampleRate;

    public void Analyse()
    {
        cuePoints = new List<CuePoint>();
        
        _filePath = AssetDatabase.GetAssetPath(audioClip);
        _chunkDataSize = CUE_CHUNK_SIZE;
        
        var audioFile = File.Open(_filePath, FileMode.Open);
        BinaryReader reader = new BinaryReader(audioFile);

        reader.ReadBytes(24);
        _sampleRate = reader.ReadUInt16();
        
        byte[] nextChunk = reader.ReadBytes(2);
        while (Encoding.ASCII.GetString(nextChunk) != "cue ")
        {
            nextChunk = reader.ReadBytes(4);
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                Debug.Log("No cue chunk found. Adding arbitrary cue!");
                audioFile.Close();
                reader.Close();
                AddCueChunk();
                return;
            }
        }

        _chunkDataSize = reader.ReadUInt32();
        _dwCuePoints = reader.ReadUInt32();

        Debug.Log(_sampleRate);
        for (int i = 0; i < _dwCuePoints; i++)
        {
            var name = reader.ReadUInt32();
            var position = reader.ReadUInt32();
            var positionInSeconds = position / _sampleRate;
            var dataChunkId = reader.ReadUInt32();
            var chunkStart = reader.ReadUInt32();
            var blockStart = reader.ReadUInt32();
            var sampleOffset = reader.ReadUInt32();
            
            CuePoint newCuePoint = new CuePoint(name, position, positionInSeconds, dataChunkId, chunkStart, blockStart, position);
            cuePoints.Add(newCuePoint);
            Debug.Log("Marker " + name + " at " + positionInSeconds + " s");
        }
        reader.Close();
        audioFile.Close();

        UpdateScriptableObject();
    }

    /*public void ClearMarkers()
    {
        // create temp copy
        var temp = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/temp.wav", FileMode.OpenOrCreate);

        // write everything but the cue chunk inside
        var audioFile = File.Open(_filePath, FileMode.OpenOrCreate);
        
        BinaryReader reader = new BinaryReader(audioFile);
        BinaryWriter writer = new BinaryWriter(temp);
        
        byte[] nextChunk = reader.ReadBytes(4);
        
        for (int i = 0; i < reader.BaseStream.Length / 4; i++)
        {
            {
                writer.Write(nextChunk);
                nextChunk = reader.ReadBytes(nextChunk.Length);
                if (Encoding.ASCII.GetString(nextChunk) == "cue ")
                {
                    break;
                }
            }
        }

        if (reader.BaseStream.Position >= reader.BaseStream.Length)
        {
            audioFile.Close();
            temp.Close();
            reader.Close();
            writer.Close();
            return;
        }
        
        reader.ReadUInt32(); // skip chunk data size
        int tempDwCuePoints = (int)reader.ReadUInt32();
        reader.ReadBytes(CUE_SIZE * tempDwCuePoints);
        
        nextChunk = reader.ReadBytes(4);
        for (int i = 0; i < reader.BaseStream.Length; i++)
        {
            {
                nextChunk = reader.ReadBytes(nextChunk.Length);
                writer.Write(nextChunk);
            }
        }

        temp.Close();
        audioFile.Close();
        reader.Close();
        writer.Close();
        
        temp = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/temp.wav", FileMode.OpenOrCreate);
        audioFile = File.Open(_filePath, FileMode.OpenOrCreate);
        temp.Position = 0;
        audioFile.Position = 0;
        // copy the temp copy into the original file
        temp.CopyTo(audioFile);
        
        temp.Close();
        audioFile.Close();
        
        // What they're doing is very smart but also very dangerous
        File.Delete(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/temp.wav");
    }*/
    
    private void AddCueChunk()
    {
        var audioFile = File.Open(_filePath, FileMode.Append);
        BinaryWriter writer = new BinaryWriter(audioFile);
        writer.Write("cue ".ToCharArray()); // chunk id
        writer.Write(CUE_CHUNK_SIZE + CUE_SIZE); // default size for one cue
        writer.Write(1); // _dwCuePoints
        
        AddCuePoint(writer);
        writer.Close();
        audioFile.Close();
        
        Analyse();
    }

    private void AddCuePoint(BinaryWriter writer)
    {
        writer.Write(0); // id
        writer.Write(122); // play order position
        writer.Write("data ".ToCharArray());
        writer.Write(0);
        writer.Write(0);
        writer.Write(122);
    }

    private void UpdateScriptableObject()
    {
        var newScriptableObject = ScriptableObject.CreateInstance<RhythmData>();
        newScriptableObject.audioClip = audioClip;
        newScriptableObject.cuePoints = cuePoints;

        AssetDatabase.CreateAsset(newScriptableObject, $"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

[System.Serializable]
public class CuePoint
{
    public string cueMarkerName;
    public int positionInSamples;
    public float positionInSeconds;
    public int dataChunkID;
    public int chunkStart;
    public int blockStart;
    public int sampleOffset;

    public CuePoint(uint name, uint position, float positionInSeconds, uint dataChunkID, uint chunkStart, uint blockStart, uint sampleOffset)
    {
        this.cueMarkerName = name.ToString();
        this.positionInSamples = (int)position;
        this.positionInSeconds = positionInSeconds;
        this.dataChunkID = (int)dataChunkID;
        this.chunkStart = (int)chunkStart;
        this.blockStart = (int)blockStart;
        this.sampleOffset = (int)sampleOffset;
    }
}