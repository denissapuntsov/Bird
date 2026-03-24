using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NAudio.Wave;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

public class AnalyserMenu : EditorWindow
{
    public AudioClip audioClip;
    private string _outputFile, _inputFile;
    public List<Cue> cues = new List<Cue>();
    public List<Cue> cuesToAdd;
    private int toolbarInt;
    private string[] toolbarStrings = new string[] { "Update/Delete", "Add From List" };
    private Texture2D _waveform;

    [MenuItem("Window/.wav File Analyser")]
    public static void ShowWindow()
    {
        GetWindow<AnalyserMenu>("Analyser");
    }

    private void CreateGUI()
    {
        cues = new List<Cue>();
        var list = new ListView();
        rootVisualElement.Add(list);
        list.makeItem = () => new Label();
        list.bindItem = (item, index) => { (item as Label).text = cues[index].label; };
    }

    private void OnGUI()
    {   
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        
        EditorGUI.BeginChangeCheck();
        audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true) as AudioClip;
        if (EditorGUI.EndChangeCheck())
        {
            // check if scriptable object for file exists, and if so, show the values as list; 
            // otherwise clear list
            
            RhythmData rhythmDataAsset = null;
            if (audioClip != null)
            {
                Debug.Log("new non-empty audio clip");
                rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
            }

            if (!rhythmDataAsset)
            {
                cues = null;
            }
            else
            {
                cues = rhythmDataAsset.cuePoints;
            }

            _waveform = DrawWaveformTexture(audioClip, (int)EditorGUIUtility.currentViewWidth - 10, 40);
        }

        
        EditorGUILayout.EndHorizontal();
        
        if (audioClip)
        {
            GUILayout.Space(10);
            float imageWidth = EditorGUIUtility.currentViewWidth - 10;
            float imageHeight = 40;
            Rect rect = GUILayoutUtility.GetRect(imageHeight, imageHeight);
            GUI.DrawTexture(rect, _waveform, ScaleMode.ScaleToFit, alphaBlend:true);
        }
        
        GUILayout.Space(10);
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(20));
        GUILayout.Space(10);
        DrawHorizontalGUILine();
        GUILayout.Space(10);
        switch (toolbarInt)
        {
            case 0:
                DrawUpdateGUI();
                break;
            case 1:
                DrawAddGUI();
                break;
        }
        GUILayout.Space(10);
    }

    private void DrawUpdateGUI()
    {
        AddProperty(nameof(cues));

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Update"))
        {
            UpdateCues();
        }

        if (GUILayout.Button("Clear"))
        {
            RemoveAllCues();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    private void DrawAddGUI()
    {
        AddProperty(nameof(cuesToAdd));
        
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add From List"))
        {
            AddCuesFromList();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddProperty(string propertyToFind)
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty serializedProperty = so.FindProperty(propertyToFind);
        
        EditorGUILayout.PropertyField(serializedProperty, true);
        so.ApplyModifiedProperties();
    }

    private void UpdateCues() => UpdateCues(false);

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

        AssetDatabase.CreateAsset(newAudioData, $"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
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
    
    private static void DrawHorizontalGUILine(int height = 1) {
        GUILayout.Space(4);

        Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
        rect.height = height;
        rect.xMin = 0;
        rect.xMax = EditorGUIUtility.currentViewWidth;

        Color lineColor = new Color(0.10196f, 0.10196f, 0.10196f, 1);
        EditorGUI.DrawRect(rect, lineColor);
        GUILayout.Space(4);
    }

    private Texture2D DrawWaveformTexture(AudioClip audioClip, int width, int height)
    {
        if (!audioClip) return null;
        
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audioClip.samples];
        float[] waveform = new float[width];
        audioClip.GetData(samples, 0);
        int packSize = (audioClip.samples / width) + 1;
        int s = 0;
        for (int i = 0; i < samples.Length; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newTexture.SetPixel(x, y, EditorGUIUtility.isProSkin
                    ? (Color) new Color32 (56, 56, 56, 255)
                    : (Color) new Color32 (194, 194, 194, 255));
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                newTexture.SetPixel(x, (height / 2) + y, new Color(1.0f, 0.64f, 0.0f, 1f));
                newTexture.SetPixel(x, (height / 2) - y, new Color(1.0f, 0.64f, 0.0f, 1f));
            }
        }

        newTexture.Apply();

        return newTexture;
    }
}
