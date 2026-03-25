using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnalyserMenu : EditorWindow
{
    public AudioClip audioClip;
    public List<Cue> cues = new List<Cue>();
    
    private List<Cue> _changedCues = new List<Cue>();
    private List<Cue> _unsavedCues = new List<Cue>();
    private bool _waveformFoldout = true;
    private bool _cuePointHeader = true;

    [MenuItem("Window/.wav File Analyser")]
    public static void ShowWindow()
    {
        GetWindow<AnalyserMenu>("Analyser");
    }

    private void OnGUI()
    {
        GUILayout.Space(20);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true) as AudioClip;
        if (EditorGUI.EndChangeCheck())
        {
            cues = new List<Cue>();
            GetCuesFromScriptableObject();
        }
        
        EditorGUILayout.EndHorizontal();
        
        if (audioClip)
        {
            ShowHeaders();
        }
        
        if (_cuePointHeader && cues != null && cues.Count > 0)
        {
            ShowCues();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        DrawUpdateGUI();
    }

    private void ShowCues()
    {
        foreach (Cue cue in cues)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Cue {cues.IndexOf(cue)}", EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
            {
                cues.Remove(cue);
                _changedCues.Remove(cue);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();
            cue.key = (CueKey)EditorGUILayout.EnumPopup("Key", cue.key);
            GUILayout.Space(5);
                
            cue.position = EditorGUILayout.IntSlider("Position in Samples",  cue.position, 0, audioClip.samples - 1);
                
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }
    }

    private void ShowHeaders()
    {
        GUILayout.Space(5);
        float imageWidth = EditorGUIUtility.currentViewWidth - 10;
        float imageHeight = (EditorGUIUtility.currentViewWidth - 10) / 8 < 100 ? (EditorGUIUtility.currentViewWidth - 10) / 8 : 100;
            
        DrawHorizontalGUILine();
        GUILayout.Space(5);
        Rect headerRect = GUILayoutUtility.GetRect(10, 15);
        _waveformFoldout = EditorGUI.BeginFoldoutHeaderGroup(headerRect, _waveformFoldout, "Waveform and Cue Markers");

        if (_waveformFoldout)
        {
            DrawWaveformAndCues(imageWidth, imageHeight);
        }
            
        GUILayout.Space(10);
        EditorGUI.EndFoldoutHeaderGroup();
            
        DrawHorizontalGUILine();
        GUILayout.Space(10);
            
        EditorGUILayout.BeginHorizontal();
        Rect cueHeaderRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 100, 20);
        _cuePointHeader = EditorGUI.BeginFoldoutHeaderGroup(cueHeaderRect, _cuePointHeader, "Cue Point List");
        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(15)))
        {
            AddCue();
            _cuePointHeader = true;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void DrawWaveformAndCues(float imageWidth, float imageHeight)
    {
        GUILayout.Space(10);
        Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
            
        Texture2D waveform = DrawWaveformTexture((int)imageWidth - 10, (int)imageHeight);
        if (waveform)
        {
            GUI.DrawTexture(rect, waveform, ScaleMode.ScaleToFit, alphaBlend:true);
        }
            
        Texture2D cueImage = DrawCueMarks((int)imageWidth - 10, (int)imageHeight);
        if (cueImage)
        {
            GUI.DrawTexture(rect, cueImage, ScaleMode.ScaleToFit, alphaBlend:true);
        }
    }

    private void GetCuesFromScriptableObject()
    {
        _changedCues = new List<Cue>();
        _unsavedCues = new List<Cue>();
        
        RhythmData rhythmDataAsset = null;
        if (audioClip != null)
        {
            rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        }

        if (!rhythmDataAsset)
        {
            cues = null;
        }
        else
        {
            cues.Clear();
            foreach (Cue cue in rhythmDataAsset.cuePoints)
            {
                cues.Add(new Cue(cue));
            }
        }
    }

    private void UpdateScriptableObject()
    {
        _changedCues = new List<Cue>();
        _unsavedCues = new List<Cue>();
        
        if (!audioClip) return;
        
        RhythmData rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");

        if (!rhythmDataAsset)
        {
            AssetDatabase.CreateAsset(rhythmDataAsset, $"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        }
        
        rhythmDataAsset.audioClip = audioClip;
        rhythmDataAsset.cuePoints = new List<Cue>();

        foreach (var cue in cues)
        {
            rhythmDataAsset.cuePoints.Add(new Cue(cue));
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawUpdateGUI()
    {
        GUILayout.Space(10);
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = AreChangesPresent();
        if (GUILayout.Button("Apply"))
        {
            UpdateScriptableObject();
        }

        if (GUILayout.Button("Revert"))
        {
            GetCuesFromScriptableObject();
        }
        GUI.enabled = true;

        GUI.enabled = cues is { Count: > 0 };
        if (GUILayout.Button("Clear"))
        {
            cues?.Clear();
        }
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    private bool AreChangesPresent()
    {
        if (!audioClip) return false;
        if (!AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset")) return false;
        
        RhythmData rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        
        foreach (Cue cue in cues)
        {
            if (_unsavedCues.Contains(cue)) continue;
            if (cues.IndexOf(cue) > rhythmDataAsset.cuePoints.Count - 1 || !cue.Equals(rhythmDataAsset.cuePoints[cues.IndexOf(cue)]))
            {
                _changedCues.Add(cue);
            }
        }

        return _changedCues.Count > 0 || _unsavedCues.Count > 0;
    }

    private void AddCue()
    {
        Cue newCue = new Cue(CueKey.W, 0, audioClip.frequency);
        cues.Add(newCue);
        _unsavedCues.Add(newCue);
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

    private Texture2D DrawWaveformTexture(int width, int height)
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
                    ? new Color32 (56, 56, 56, 255)
                    : new Color32 (194, 194, 194, 255));
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * 0.75f); y++)
            {
                newTexture.SetPixel(x, (height / 2) + y, new Color(1.0f, 0.64f, 0.0f, 1f));
                newTexture.SetPixel(x, (height / 2) - y, new Color(1.0f, 0.64f, 0.0f, 1f));
            }
        }

        newTexture.Apply();

        return newTexture;
    }

    private Texture2D DrawCueMarks(int width, int height)
    {
        if (!audioClip || cues == null || cues.Count == 0) return null;
        
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newTexture.SetPixel(x, y, Color.clear);
            }
        }
        
        foreach (Cue cue in cues)
        {
            var cuePositionScaled = cue.position / (audioClip.samples / width);

            for (int y = 0; y <= height; y++)
            {
                Color newColor;

                if (_changedCues.Contains(cue))
                {
                    newColor = Color.cyan;
                }
                else if (_unsavedCues.Contains(cue))
                {
                    newColor = Color.yellow;
                }
                else
                {
                    newColor = Color.white;
                }
                
                newTexture.SetPixel(cuePositionScaled, (height / 2) + y, newColor);
                newTexture.SetPixel(cuePositionScaled, (height / 2) - y, newColor);
            }
        }

        newTexture.Apply();
        return newTexture;
    }
}
