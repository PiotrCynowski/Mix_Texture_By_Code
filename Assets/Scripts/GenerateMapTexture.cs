using GameMap.Generator;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateMapTexture : MonoBehaviour {
    [SerializeField] int mapSideSize;
    [SerializeField] float noiseScale = 45;
    [SerializeField] float smoothEdgesValue = 10f;
    [SerializeField] Renderer colorMapRenderer, maskMapRenderer, noiseMapRenderer, textureMapRenderer;

    [SerializeField] MapDataManager dataManager;
    [SerializeField] tilesGeneratorExperimental tileGenerator;
    
    public void GenerateTexture() {

        if (tileGenerator.tileExistMaterials == null || tileGenerator.tileExistMaterials.Count < 1)
        {
            return;
        }
        
        StartCoroutine(CreateChunkTextures(tileGenerator.sideValueNumberOfTiles));
    }

    IEnumerator CreateChunkTextures(int numberOfTilesSide = 1) {
        MapGenerator mapGenerator = new(Random.Range(0, 10000), dataManager.GetTerrains(), mapSideSize, noiseScale, smoothEdgesValue);
        
        int chunkSize = mapSideSize / numberOfTilesSide;
        Texture2D chunkTexture;
        int chunkIndex = 0;
        for (int chunkX = 0; chunkX < numberOfTilesSide; chunkX++)
        {
            for (int chunkY = 0; chunkY < numberOfTilesSide; chunkY++)
            {
                chunkTexture = mapGenerator.GenerateChunkMapTexture(chunkSize, new Vector2Int(chunkX, chunkY));
                yield return new WaitUntil(() => chunkTexture != null);
                tileGenerator.AssignTextureToTile(chunkTexture,chunkIndex);
                chunkIndex++;
                yield return null;
            }
        }
        
        yield return null;
        
        (Texture2D colorMap, Texture2D maskMap, Texture2D noiseMap) = mapGenerator.GenerateColorMaskNoiseMap();
        colorMapRenderer.sharedMaterial.mainTexture = colorMap;
        maskMapRenderer.sharedMaterial.mainTexture = maskMap;
        noiseMapRenderer.sharedMaterial.mainTexture = noiseMap;

    }
    
    IEnumerator CreateTextures() {
        MapGenerator mapGenerator = new(Random.Range(0, 10000), dataManager.GetTerrains(), mapSideSize, noiseScale, smoothEdgesValue);
        (Texture2D colorMap, Texture2D maskMap, Texture2D noiseMap, Texture2D textureMap) = mapGenerator.GenerateColorMaskNoiseTextureMap();

        colorMapRenderer.sharedMaterial.mainTexture = colorMap;
        maskMapRenderer.sharedMaterial.mainTexture = maskMap;
        noiseMapRenderer.sharedMaterial.mainTexture = noiseMap;
        textureMapRenderer.sharedMaterial.mainTexture = textureMap;

        yield return null;
    }
}