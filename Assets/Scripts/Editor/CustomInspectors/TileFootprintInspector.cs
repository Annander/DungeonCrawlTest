using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TileFootprint))]
public class TileFootprintInspector : Editor
{
    private SerializedProperty width, height;
    private SerializedProperty footprint;
    private SerializedProperty entrances;

    private void OnEnable()
    {
        width = serializedObject.FindProperty("Width");
        height = serializedObject.FindProperty("Height");
        footprint = serializedObject.FindProperty("Footprint");
        entrances = serializedObject.FindProperty("Entrances");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);

        if (width.intValue == 0 || height.intValue == 0)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        footprint.arraySize = width.intValue * height.intValue;

        EditorGUILayout.Space(20f);

        var newEntrances = new List<int>();

        for (int y = 0; y < height.intValue; y++) 
        {
            EditorGUILayout.BeginHorizontal();

            for(int x = 0; x < width.intValue; x++)
            {
                var index = y * width.intValue + x;
                var element = footprint.GetArrayElementAtIndex(index);

                GUI.color = Color.white;

                if(element.boolValue)
                {
                    if(height.intValue > 1)
                    {
                        // North
                        if (x == width.intValue / 2 && y == height.intValue - 1)
                        {
                            newEntrances.Add(index);
                            GUI.color = Color.green;
                        }

                        // South
                        if (x == width.intValue / 2 && y == 0)
                        {
                            newEntrances.Add(index);
                            GUI.color = Color.green;
                        }
                    }

                    if(width.intValue > 1)
                    {
                        // West
                        if (x == 0 && y == height.intValue / 2)
                        {
                            newEntrances.Add(index);
                            GUI.color = Color.green;
                        }

                        // East
                        if (x == width.intValue - 1 && y == height.intValue / 2)
                        {
                            newEntrances.Add(index);
                            GUI.color = Color.green;
                        }
                    }
                }

                element.boolValue = EditorGUILayout.Toggle(element.boolValue, GUILayout.Width(20f));
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.color = Color.white;

        entrances.ClearArray();
        entrances.arraySize = newEntrances.Count;

        var entranceIndexLabel = "";

        for (int i = 0; i < newEntrances.Count; i++)
        {
            entrances.InsertArrayElementAtIndex(i);
            entrances.GetArrayElementAtIndex(i).intValue = newEntrances[i];
            entranceIndexLabel += newEntrances[i].ToString();

            if (i < newEntrances.Count - 1)
                entranceIndexLabel += ", ";
        }
        
        EditorGUILayout.LabelField(entranceIndexLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("FILL")) FillFootprintArray(true);

        if (GUILayout.Button("CLEAR")) FillFootprintArray(false);

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void FillFootprintArray(bool fill)
    {
        for(int i = 0; i < footprint.arraySize; i++) 
        {
            footprint.GetArrayElementAtIndex(i).boolValue = fill;
        }
    }
}