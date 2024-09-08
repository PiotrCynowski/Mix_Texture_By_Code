using UnityEngine;

public class MapGenerator
{
    int octaves = 1;
    float persistance = 0.5f;
    float lacunarity = 1;

    float[,] examplePathArray;
    
    int seed;
    TerrainType[] regions;
    Vector2 offset = Vector2.zero;

    public MapGenerator(int _seed) {
        seed = _seed;
    }

    /*#region Chunks
    void GenerateMapInChunks(TerrainType[] regions) 
    {
        int chunkSize = 128;
        int numberOfChunks = mapSideSize / chunkSize;

        for (int chunkY = 0; chunkY < numberOfChunks; chunkY++) {
            for (int chunkX = 0; chunkX < numberOfChunks; chunkX++) {
                ProcessChunk(chunkX, chunkY, chunkSize, regions);
            }
        }
    }
    #endregion*/

    public (Texture2D, Texture2D, Texture2D, Texture2D) GenerateMap(TerrainType[] regions, int mapSideSize, float noiseScale, bool smoothEdges = false) {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSideSize, mapSideSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapSideSize * mapSideSize];
        Color[] maskMap = new Color[mapSideSize * mapSideSize];
        Color[] noiseMapTex = new Color[mapSideSize * mapSideSize];
        Color[] textureMap = new Color[mapSideSize * mapSideSize];
        
        for (int y = 0; y < mapSideSize; y++) {
            for (int x = 0; x < mapSideSize; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {

                        colorMap[y * mapSideSize + x] = regions[i].color;
                        maskMap[y * mapSideSize + x] = regions[i].maskColor;
                        noiseMapTex[y * mapSideSize + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

                        if (smoothEdges)
                        {
                            if (i - 1 >= 0)
                            {
                                float diff = currentHeight - regions[i - 1].height;
                                if (diff <= 0.1)
                                {
                                    textureMap[y * mapSideSize + x] = LerpTextures(x, y, mapSideSize, i, diff, regions);
                                    break;
                                }
                            }
                        }

                        Texture2D texture = regions[i].texture;
                        int tiling = regions[i].tiling;
                        float u = (x / (float)mapSideSize) * tiling;
                        float v = (y / (float)mapSideSize) * tiling;
                        textureMap[y * mapSideSize + x] = texture.GetPixelBilinear(u % 1, v % 1);
                        break;
                    }
                }
            }
        }

        return (
            TextureFromColourMap(colorMap, mapSideSize, mapSideSize),
            TextureFromColourMap(maskMap, mapSideSize, mapSideSize),
            TextureFromColourMap(noiseMapTex, mapSideSize, mapSideSize),
            TextureFromColourMap(textureMap, mapSideSize, mapSideSize)
        );
    }

    Color LerpTextures(int x, int y, int mapSideSize, int region, float difference, TerrainType[] regions) {
        Texture2D texture1 = regions[region].texture;
        int tiling1 = regions[region].tiling;
        
        Texture2D texture2 = regions[region-1].texture;
        int tiling2 = regions[region-1].tiling;

        float u1 = (x / (float)mapSideSize) * tiling1;
        float v1 = (y / (float)mapSideSize) * tiling1;
        float u2 = (x / (float)mapSideSize) * tiling2;
        float v2 = (y / (float)mapSideSize) * tiling2;

        Color color1 = texture1.GetPixelBilinear(u1 % 1, v1 % 1);
        Color color2 = texture2.GetPixelBilinear(u2 % 1, v2 % 1);

        Color lerpedColor = Color.Lerp(color1, color2, 1- (difference*10));
        return lerpedColor;
    }

    Texture2D TextureFromColourMap(Color[] colorMap, int width, int height, bool mipMaps = false, FilterMode filterMode = FilterMode.Point) {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, mipMaps);
        texture.filterMode = filterMode;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
    public Color maskColor;
    public Texture2D texture;
    public int tiling;
}