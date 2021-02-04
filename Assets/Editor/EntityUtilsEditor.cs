using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomColor))]
public class RandomColorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Pick material"))
            ((RandomColor)target).PickMaterial();
    }
}
