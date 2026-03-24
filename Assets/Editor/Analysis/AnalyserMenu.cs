using System.Collections.Generic;
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
    private string[] toolbarStrings = new string[] { "Update/Delete"/*, "Add From List" */};
    private Texture2D _waveform;

    [MenuItem("Window/.wav File Analyser")]
    public static void ShowWindow()
    {
        GetWindow<AnalyserMenu>("Analyser");
    }

    private void OnGUI()
    {   
        GUILayout.Space(10);
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
            _waveform = DrawWaveformTexture(audioClip, (int)EditorGUIUtility.currentViewWidth - 10, 40);
            GUILayout.Space(10);
            float imageWidth = EditorGUIUtility.currentViewWidth - 10;
            float imageHeight = 40;
            
            DrawHorizontalGUILine();
            GUILayout.Space(10);
            Rect rect = GUILayoutUtility.GetRect(imageHeight, imageHeight);
            GUILayout.Space(10);
            DrawHorizontalGUILine();
            if (_waveform)
            {
                GUI.DrawTexture(rect, _waveform, ScaleMode.ScaleToFit, alphaBlend:true);
            }
            
            var cueImage = DrawCueMarks(cues, audioClip, (int)EditorGUIUtility.currentViewWidth - 10, 40);
            if (cueImage)
            {
                GUI.DrawTexture(rect, cueImage, ScaleMode.ScaleToFit, alphaBlend:true);
            }
        }
        
        /*GUILayout.Space(10);
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(20));
        GUILayout.Space(10);
        DrawHorizontalGUILine();*/
        GUILayout.Space(10);

        DrawUpdateGUI();
        /*switch (toolbarInt)
        {
            case 0:
                DrawUpdateGUI();
                break;
            case 1:
                DrawAddGUI();
                break;
        }
        GUILayout.Space(10);*/
    }

    private void GetCuesFromScriptableObject()
    {
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
                cues.Add(cue);
            }
        }
    }

    private void UpdateScriptableObject()
    {
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
            rhythmDataAsset.cuePoints.Add(cue);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawUpdateGUI()
    {
        AddProperty(nameof(cues));

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Apply"))
        {
            UpdateScriptableObject();
        }

        if (GUILayout.Button("Revert"))
        {
            GetCuesFromScriptableObject();
        }

        if (GUILayout.Button("Clear"))
        {
            cues.Clear();
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
            //AddCuesFromList();
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
            for (int y = 0; y <= waveform[x] * ((float)height * 0.75f); y++)
            {
                newTexture.SetPixel(x, (height / 2) + y, new Color(1.0f, 0.64f, 0.0f, 1f));
                newTexture.SetPixel(x, (height / 2) - y, new Color(1.0f, 0.64f, 0.0f, 1f));
            }
        }

        newTexture.Apply();

        return newTexture;
    }

    private Texture2D DrawCueMarks(List<Cue> cues, AudioClip clip, int width, int height)
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
            var cuePositionScaled = cue.position / (clip.samples / width);

            for (int y = 0; y <= height; y++)
            {
                newTexture.SetPixel(cuePositionScaled, (height / 2) + y, Color.cyan);
                newTexture.SetPixel(cuePositionScaled, (height / 2) - y, Color.cyan);
            }
        }

        newTexture.Apply();
        
        return newTexture;
    }
}
