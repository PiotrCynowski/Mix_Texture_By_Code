using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GenerateMapTexture))]
public class BallSettupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateMapTexture GMT = (GenerateMapTexture)target;

        if (GUILayout.Button("Generate Map Texture"))
        {
            GMT.GenerateTexture();
        }
    }
}
#endif
