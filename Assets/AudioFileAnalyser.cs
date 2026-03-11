using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioFileAnalyser : MonoBehaviour
{
    public AudioClip audioClip;

    public void Analyse()
    {
        string filePath = AssetDatabase.GetAssetPath(audioClip);
        var audioFile = File.Open(filePath, FileMode.Open);
        BinaryReader reader = new BinaryReader(audioFile);

        byte[] nextChunk = reader.ReadBytes(4);
        while (Encoding.ASCII.GetString(nextChunk) != "cue")
        {
            nextChunk = reader.ReadBytes(4);
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                Debug.Log("Nothing found.");
                return;
            }
        }

        long chunkDataSize = reader.ReadInt32();
        long dwCuePoints = reader.ReadInt32();
        
        Debug.Log(Encoding.ASCII.GetString(nextChunk) + " " + dwCuePoints);
        reader.Close();
    }


}
