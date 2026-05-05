using UnityEditor;

[CustomEditor(typeof(MovingCharacter))]
public class MovingCharacterEditor : Editor
{
    public void OnSceneGUI()
    {
        var character = target as MovingCharacter;
        if (!character) return;

        if (character.targets.Count == 0) return;
        for (int i = 0; i < character.targets.Count; i++)
        {
            var newTarget = character.targets[i];
            Handles.Label(newTarget.destination, newTarget.label);

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
