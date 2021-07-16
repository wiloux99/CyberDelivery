using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MapGenerator))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            if (mapGen.UpdateRealTime)
            {
                mapGen.GenerateMap();
            }
        }

            if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
