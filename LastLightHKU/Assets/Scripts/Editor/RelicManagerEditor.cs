using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RelicManager))]
public class RelicManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RelicManager manager = (RelicManager)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Reset All Relics", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Reset All Relics", 
                "Are you sure you want to reset all collected relics?", 
                "Yes", "Cancel"))
            {
                manager.ResetAll();
            }
        }
    }
}
