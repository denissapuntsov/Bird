using Unity.VisualScripting;
using UnityEditor;

[CustomEditor(typeof(MovingCharacter))]
public class MovingCharacterEditor : Editor
{
    public void OnSceneGUI()
    {
        var character = target as MovingCharacter;
        if (!character) return;

        if (character.paths.Count == 0) return;

        foreach (var path in character.paths)
        {
            for (int i = 0; i < path.targets.Count; i++)
            {
                var newTarget = path[i];
                Handles.Label(newTarget.destination, "Path " + character.paths.IndexOf(path) + " / Target " + path.targets.IndexOf(newTarget));

                EditorGUI.BeginChangeCheck();
                var newPosition = Handles.PositionHandle(newTarget.destination, character.transform.rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    newTarget.destination = newPosition;
                    Undo.RecordObject(character, "Move Target " + i);
                }
            }
        }
    }
}
