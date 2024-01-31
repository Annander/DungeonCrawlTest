using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TileFootprint))]
public class TileFootprintInspector : Editor
{
    private SerializedProperty size;
    private SerializedProperty footprint;
    private SerializedProperty entrances;

    private void OnEnable()
    {
        size = serializedObject.FindProperty("Size");
        footprint = serializedObject.FindProperty("Footprint");
        entrances = serializedObject.FindProperty("Entrances");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(size);

        if (size.intValue == 0)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        footprint.arraySize = size.intValue * size.intValue;

        EditorGUILayout.Space(20f);

        var newEntrances = new List<int>();

        for (int y = 0; y < size.intValue; y++) 
        {
            EditorGUILayout.BeginHorizontal();

            for(int x = 0; x < size.intValue; x++)
            {
                var index = y * size.intValue + x;
                var element = footprint.GetArrayElementAtIndex(index);

                GUI.color = Color.white;

                if(element.boolValue)
                {
                    for (int i = 0; i < entrances.arraySize; i++)
                    {
                        if(entrances.GetArrayElementAtIndex(i).intValue == index)
                            GUI.color = Color.green;
                    }
                }

                var e = Event.current;

                var boolValue = element.boolValue;
                
                element.boolValue = EditorGUILayout.Toggle(element.boolValue, GUILayout.Width(20f));
                
                // If the bool is not a valid entrance
                var validEntrance = x == 0 || x == size.intValue - 1 || y == 0 || y == size.intValue -1;
                
                // If a change happened and you held shift down
                if (validEntrance &&
                    element.boolValue != boolValue &&
                    e.shift)
                {
                    // If the bool was unselected
                    if (!element.boolValue)
                    {
                        element.boolValue = true;
                        
                        if(!newEntrances.Contains(index) && !EntranceExists(index))
                            newEntrances.Add(index);
                    }

                    if (EntranceExists(index))
                    {
                        var elementIndexForDeletion = -1;
            
                        for (int i = 0; i < entrances.arraySize; i++)
                        {
                            if (entrances.GetArrayElementAtIndex(i).intValue == index)
                                elementIndexForDeletion = i;
                        }
            
                        if(elementIndexForDeletion > -1)
                            entrances.DeleteArrayElementAtIndex(elementIndexForDeletion);                            
                    }
                }
                
                // If a flag was unchecked, it must also be removed from the entrance array if it exists there
                if (!element.boolValue)
                {
                    var elementIndexForDeletion = -1;
                    
                    for (int i = 0; i < entrances.arraySize; i++)
                    {
                        if (entrances.GetArrayElementAtIndex(i).intValue == index)
                            elementIndexForDeletion = i;
                    }
                    
                    if(elementIndexForDeletion > -1)
                        entrances.DeleteArrayElementAtIndex(elementIndexForDeletion);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.color = Color.white;
        
        if (newEntrances.Count > 0)
        {
            var arraySize = entrances.arraySize;

            for (var i = 0; i < arraySize; i++)
            {
                newEntrances.Add(entrances.GetArrayElementAtIndex(i).intValue);
            }
            
            entrances.ClearArray();
            
            for (int i = 0; i < newEntrances.Count; i++)
            {
                entrances.InsertArrayElementAtIndex(i);
                entrances.GetArrayElementAtIndex(i).intValue = newEntrances[i];
            }
            
            newEntrances.Clear();
        }

        var entranceIndexLabel = "";

        for (var i = 0; i < entrances.arraySize; i++)
        {
            entranceIndexLabel += entrances.GetArrayElementAtIndex(i).intValue.ToString();
            if (i < entrances.arraySize - 1)
                entranceIndexLabel += ", ";
        }
        
        EditorGUILayout.LabelField("Shift-click to create entrance:", EditorStyles.miniLabel);
        
        EditorGUILayout.LabelField(entranceIndexLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("FILL TILES")) FillFootprintArray(true);

        if (GUILayout.Button("CLEAR TILES")) FillFootprintArray(false);
        
        if (GUILayout.Button("CLEAR ENTRANCES")) entrances.ClearArray();
        
        if (GUILayout.Button("ROTATE 180"))
        {
            (target as TileFootprint).RotateFootprint(TileFootprint.RotationOp.Rot180);
            serializedObject.Update();
        }

        if (GUILayout.Button("ROTATE CW"))
        {
            (target as TileFootprint).RotateFootprint(TileFootprint.RotationOp.Rot90);
            serializedObject.Update();
        }
        
        if (GUILayout.Button("ROTATE CCW"))
        {
            (target as TileFootprint).RotateFootprint(TileFootprint.RotationOp.Rot270);
            serializedObject.Update();
        }

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

    private bool EntranceExists(int index)
    {
        for (var i = 0; i < entrances.arraySize; i++)
        {
            if (entrances.GetArrayElementAtIndex(i).intValue == index)
                return true;
        }

        return false;
    }
}