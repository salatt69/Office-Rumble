using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomModuleDefinition))]
public class RoomModuleDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomModuleDefinition room = (RoomModuleDefinition)target;

        room.roomType = (LevelGeneration.Rooms)
            EditorGUILayout.EnumPopup("Room Type", room.roomType);

        room.doors = (DoorMask)
            EditorGUILayout.EnumFlagsField("Doors", room.doors);

        room.weight = EditorGUILayout.IntField("Weight", room.weight);

        if (GUI.changed)
            EditorUtility.SetDirty(room);
    }
}