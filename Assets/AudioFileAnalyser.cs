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
        while (Encoding.ASCII.GetString(nextChunk) != "bext")
        {
            nextChunk = reader.ReadBytes(4);
        }
        Debug.Log(Encoding.ASCII.GetString(nextChunk));
        reader.Close();
    }


}
