using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapDisplay))]
public class MapDisplayGeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Regenerate Map Data"))
        {
            ((MapDisplay)target).GenerateMap();
        }
    }
}
