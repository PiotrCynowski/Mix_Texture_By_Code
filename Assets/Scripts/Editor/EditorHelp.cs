using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GenerateMapTexture))]
public class GenerateMapTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateMapTexture GMT = (GenerateMapTexture)target;

        if (GUILayout.Button("Generate Chunk Textures"))
        {
            GMT.GenerateChunkTextures();
        }
        
        if (GUILayout.Button("Generate One Texture"))
        {
            GMT.GenerateOneTexture();
        }
    }
}
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(tilesGeneratorExperimental))]
public class tilesGeneratorExperimentalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        tilesGeneratorExperimental TGE = (tilesGeneratorExperimental)target;

        if (GUILayout.Button("Generate Tiles"))
        {
            TGE.GenerateTilesMatrix();
        }
    }
}
#endif

