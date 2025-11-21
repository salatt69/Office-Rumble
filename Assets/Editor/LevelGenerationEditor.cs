using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGeneration))]
public class LevelGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw all serialized fields

        LevelGeneration generator = (LevelGeneration)target;
        if (GUILayout.Button("Generate New Level"))
        {
            ClearConsole();
            generator.GenerateFromMenu();
        }

        if (GUILayout.Button("Clear Previous Level"))
        {
            ClearConsole();
            generator.ClearPreviousLevel();
        }
    }

    // Editor-only method
    private void ClearConsole()
    {
#if UNITY_EDITOR
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
#endif
    }
}