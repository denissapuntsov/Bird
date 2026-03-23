using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using Unity.VisualScripting;
using UnityEditor;
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
        audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true) as AudioClip;
        EditorGUILayout.EndHorizontal();
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
}
