using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[CustomEditor(typeof(MapDisplay))]
public class MapDisplayGeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Regenerate Map Data"))
        {
            foreach(GameObject go in ((MapDisplay)target).GenerateMap())
            {
                DestroyImmediate(go);
            }
            EditorSceneManager.MarkSceneDirty(((MapDisplay)target).gameObject.scene);
            EditorUtility.SetDirty(target);
        }
    }
}
