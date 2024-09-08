using GameMap.Generator;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateMapTexture : MonoBehaviour {
    public int mapSideSize;
    public float noiseScale = 45;
    public bool smoothEdges = false;
    public Renderer colorMapRenderer;
    public Renderer maskMapRenderer;
    public Renderer noiseMapRenderer;
    public Renderer textureMapRenderer;
    public MapDataManager dataManager;
    
    public void GenerateTexture() {
        StartCoroutine(CreateTexture());
    }

    IEnumerator CreateTexture() {
        MapGenerator mapGenerator = new(Random.Range(0, 10000));
        
        (Texture2D colorMap, Texture2D maskMap, Texture2D noiseMap, Texture2D textureMap) = mapGenerator.GenerateMap(dataManager.GetTerrains(), mapSideSize, noiseScale, smoothEdges);

        colorMapRenderer.sharedMaterial.mainTexture = colorMap;
        maskMapRenderer.sharedMaterial.mainTexture = maskMap;
        noiseMapRenderer.sharedMaterial.mainTexture = noiseMap;
        textureMapRenderer.sharedMaterial.mainTexture = textureMap;

        yield return null;
    }
}