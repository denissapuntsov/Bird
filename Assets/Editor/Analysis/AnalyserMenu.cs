using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;

public class AnalyserMenu : EditorWindow
{
    public AudioClip audioClip;
    private List<Cue> _cues = new List<Cue>();

    private Dictionary<int, Cue> _positions = new Dictionary<int, Cue>();
    private bool _waveformFoldout = true;
    private bool _cuePointHeader = true;
    private Vector2 _scrollPosition;

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
            _cues = new List<Cue>();
            GetCuesFromScriptableObject();
        }
        
        EditorGUILayout.EndHorizontal();
        
        if (audioClip)
        {
            ShowHeaders();
        }
        
        if (_cuePointHeader && _cues != null && _cues.Count > 0)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition,GUILayout.Width(EditorGUIUtility.currentViewWidth - 7.5f));
            ShowCues();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        DrawButtons();
    }
    private void DrawButtons()
    {
        DrawHorizontalGUILine();
        GUILayout.Space(10);
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = AreChangesPresent() && !AreCuesOverlapping();
        if (GUILayout.Button("Save"))
        {
            Save();
        }

        GUI.enabled = AreChangesPresent();
        if (GUILayout.Button("Revert"))
        {
            GetCuesFromScriptableObject();
        }
        GUI.enabled = true;

        GUI.enabled = _cues is { Count: > 0 };
        if (GUILayout.Button("Clear"))
        {
            _cues?.Clear();
        }
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    #region Blocks

    private void ShowCues()
    {
        foreach (Cue cue in _cues)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Cue {_cues.IndexOf(cue)}", EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
            {
                _cues.Remove(cue);
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
        DrawHorizontalGUILine();
        
        float imageWidth = EditorGUIUtility.currentViewWidth - 10;
        float imageHeight = (EditorGUIUtility.currentViewWidth - 10) / 8 < 100 ? (EditorGUIUtility.currentViewWidth - 10) / 8 : 100;
            
        GUILayout.Space(5);
        ShowWaveformBlock(imageWidth, imageHeight);

        DrawHorizontalGUILine();
        GUILayout.Space(10);
            
        ShowCueBlock();
        GUILayout.Space(10);
    }

    private void ShowWaveformBlock(float imageWidth, float imageHeight)
    {
        Rect headerRect = GUILayoutUtility.GetRect(10, 15);
        _waveformFoldout = EditorGUI.BeginFoldoutHeaderGroup(headerRect, _waveformFoldout, "Waveform and Cue Markers");

        if (_waveformFoldout)
        {
            DrawWaveformAndCues(imageWidth, imageHeight);
        }
            
        GUILayout.Space(10);
        EditorGUI.EndFoldoutHeaderGroup();
    }

    private void ShowCueBlock()
    {
        EditorGUILayout.BeginHorizontal();
        Rect cueHeaderRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 100, 20);
        _cuePointHeader = EditorGUI.BeginFoldoutHeaderGroup(cueHeaderRect, _cuePointHeader, "Cue Point List");
        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(15)))
        {
            AddCue();
            _cuePointHeader = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Sync

    private void GetCuesFromScriptableObject()
    {
        RhythmData rhythmDataAsset = null;
        if (audioClip != null)
        {
            rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        }

        if (!rhythmDataAsset)
        {
            _cues = null;
        }
        else
        {
            _cues.Clear();
            _positions.Clear();
            foreach (Cue cue in rhythmDataAsset.cuePoints)
            {
                cue.state = CueState.Saved;
                var newCue = new Cue(cue);
                _cues.Add(newCue);
                _positions.Add(newCue.position, newCue);
            }
        }
    }
    private void Save()
    {
        if (!audioClip) return;
        
        RhythmData rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");

        if (!rhythmDataAsset)
        {
            AssetDatabase.CreateAsset(rhythmDataAsset, $"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        }
        
        rhythmDataAsset.audioClip = audioClip;
        rhythmDataAsset.cuePoints = new List<Cue>();
        
        SortCues();

        _positions.Clear();
        foreach (var cue in _cues)
        {
            cue.state = CueState.Saved;
            Cue newCue = new Cue(cue);
            _positions.Add(newCue.position, newCue);
            rhythmDataAsset.cuePoints.Add(newCue);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private bool AreChangesPresent()
    {
        if (!audioClip) return false;
        if (!AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset")) return false;
        
        RhythmData rhythmDataAsset = AssetDatabase.LoadAssetAtPath<RhythmData>($"Assets/CuePoints/{audioClip.name}_CuePoints.asset");
        int changedCueCount = 0;

        if (rhythmDataAsset.cuePoints.Count != _cues.Count)
        {
            changedCueCount++;
        }

        bool areNewCuesPresent = false;
        
        foreach (Cue cue in _cues)
        {
            if (cue.state == CueState.New)
            {
                areNewCuesPresent = true;
                continue;
            }
            if (_cues.IndexOf(cue) > rhythmDataAsset.cuePoints.Count - 1 ||
                !cue.Equals(rhythmDataAsset.cuePoints[_cues.IndexOf(cue)]))
            {
                changedCueCount++;
                cue.state = CueState.Modified;
            }
            else cue.state = CueState.Saved;
        }

        return changedCueCount > 0 || areNewCuesPresent;
    }
    
    private bool AreCuesOverlapping()
    {
        if (!audioClip || _cues.Count == 0)
        {
            _positions.Clear();
            return false;
        }
        
        int overlappingCuesCount = 0;
        _positions.Clear();
        foreach (Cue cue in _cues)
        {
            if (!_positions.TryGetValue(cue.position, out Cue duplicate))
            {
                _positions.Add(cue.position, cue);
                cue.isOverlapping = false;
            }
            else
            {
                overlappingCuesCount++;
                duplicate.isOverlapping = true;
                cue.isOverlapping = true;
            }
        }
        return overlappingCuesCount > 0;
    }
    private void AddCue()
    {
        Cue newCue = new Cue(CueKey.W, 0, audioClip.frequency);
        _cues.Add(newCue);
    }

    #endregion

    #region Visuals

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
        if (!audioClip || _cues == null || _cues.Count == 0) return null;
        
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newTexture.SetPixel(x, y, Color.clear);
            }
        }
        
        foreach (Cue cue in _cues)
        {
            var cuePositionScaled = cue.position / (audioClip.samples / width);

            for (int y = 0; y <= height; y++)
            {
                Color newColor;

                if (cue.isOverlapping)
                {
                    newColor = Color.red;
                }
                else newColor = cue.state switch
                {
                    CueState.New => Color.yellow,
                    CueState.Saved => Color.white,
                    CueState.Modified => Color.cyan,
                    _ => Color.magenta
                };
                
                newTexture.SetPixel(cuePositionScaled, (height / 2) + y, newColor);
                newTexture.SetPixel(cuePositionScaled, (height / 2) - y, newColor);
            }
        }

        newTexture.Apply();
        return newTexture;
    }

    #endregion
    
    #region Sorting
    
    private void SortCues()
    {
        if (!audioClip) return;
        
        List<Cue> sortedList = new List<Cue>();
        
        Tuple<int, int>[] array = new Tuple<int, int>[_cues.Count]; // Item1 is position in samples, Item2 is index in original list

        for (int i = 0; i < _cues.Count; i++)
        {
            array[i] = new Tuple<int, int>(_cues[i].position, i);
        }
        
        QuickSort(array, 0, array.Length - 1);

        foreach (Tuple<int, int> pair in array)
        {
            sortedList.Add(_cues[pair.Item2]);
        }
        
        _cues = new List<Cue>(sortedList);
    }
    private void QuickSort(Tuple<int, int>[] array, int start, int end)
    {
        if (start < end)
        {
            int pivot = Partition(array, start, end);
            QuickSort(array, start, pivot - 1);
            QuickSort(array, pivot + 1, end);
        }
    }
    private int Partition(Tuple<int, int>[] array, int start, int end)
    {
        Tuple<int, int> pivot = array[end];
        int i = start - 1;

        for (int j = start; j < end; j++)
        {
            if (array[j].Item1 <= pivot.Item1)
            {
                i++;
                
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        (array[i + 1], array[end]) = (array[end], array[i + 1]);

        return i + 1;
    }
    
    #endregion
}
