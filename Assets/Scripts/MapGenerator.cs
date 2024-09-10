using UnityEngine;

public class MapGenerator
{
    int mapSideSize;
    float smoothEdgesValue;
    
    TerrainType[] regions;
    
    readonly Vector2 offset = Vector2.zero;
    readonly int octaves = 1;
    readonly float persistance = 0.5f;
    readonly float lacunarity = 1;

    float[,] noiseMap;

    public MapGenerator(int seed, TerrainType[] regions, int mapSideSize, float noiseScale, float smoothEdgesValue) {
        this.regions = regions;
        this.mapSideSize = mapSideSize;
        this.smoothEdgesValue = smoothEdgesValue;
        
        noiseMap = NoiseGenerator.GenerateNoiseMap(mapSideSize, mapSideSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    #region Generate Texture
    public Texture2D GenerateChunkMapTexture(int chunkSize, Vector2Int chunkLocCoord)
    {
        Color[] textureMap = new Color[chunkSize * chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int mapX = chunkLocCoord.x * chunkSize + x;
                int mapY = chunkLocCoord.y * chunkSize + y;

                float currentHeight = noiseMap[mapX, mapY];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        if (smoothEdgesValue > 0 && i - 1 >= 0)
                        {
                            float diff = currentHeight - regions[i - 1].height;
                            if (diff <= 0.1f)
                            {
                                textureMap[y * chunkSize + x] = LerpTextures(mapX, mapY, i, diff, chunkSize);
                                break;
                            }
                        }

                        Texture2D texture = regions[i].texture;
                        int tiling = regions[i].tiling;
                        float u = (x / (float)chunkSize) * tiling;
                        float v = (y / (float)chunkSize) * tiling;
                        textureMap[y * chunkSize + x] = texture.GetPixelBilinear(u % 1, v % 1);
                        break;
                    }
                }
            }
        }

        return TextureFromColourMap(textureMap, chunkSize, chunkSize);
    }
    
    public (Texture2D, Texture2D, Texture2D) GenerateColorMaskNoiseTextureMap() {
        Color[] colorMap = new Color[mapSideSize * mapSideSize];
        Color[] noiseMapTex = new Color[mapSideSize * mapSideSize];
        Color[] textureMap = new Color[mapSideSize * mapSideSize];
        
        for (int y = 0; y < mapSideSize; y++) {
            for (int x = 0; x < mapSideSize; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {

                        colorMap[y * mapSideSize + x] = regions[i].color;
                        noiseMapTex[y * mapSideSize + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

                        if (smoothEdgesValue > 0 && i - 1 >= 0)
                        {
                            float diff = currentHeight - regions[i - 1].height;
                            if (diff <= 0.1)
                            {
                                textureMap[y * mapSideSize + x] = LerpTextures(x, y, i, diff);
                                break;
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
            TextureFromColourMap(noiseMapTex, mapSideSize, mapSideSize),
            TextureFromColourMap(textureMap, mapSideSize, mapSideSize)
        );
    }
    
    public (Texture2D, Texture2D) GenerateColorMaskNoiseMap() {
        Color[] colorMap = new Color[mapSideSize * mapSideSize];
        Color[] noiseMapTex = new Color[mapSideSize * mapSideSize];
        
        for (int y = 0; y < mapSideSize; y++) {
            for (int x = 0; x < mapSideSize; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {
                        colorMap[y * mapSideSize + x] = regions[i].color;
                        noiseMapTex[y * mapSideSize + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                        break;
                    }
                }
            }
        }

        return (
            TextureFromColourMap(colorMap, mapSideSize, mapSideSize),
            TextureFromColourMap(noiseMapTex, mapSideSize, mapSideSize)
        );
    }
    #endregion
    
    Color LerpTextures(int x, int y, int region, float difference) {
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

        Color lerpedColor = Color.Lerp(color1, color2, 1 - (difference*smoothEdgesValue));
        return lerpedColor;
    }
    
    Color LerpTextures(int x, int y, int region, float difference, int chunkSize) {
        Texture2D texture1 = regions[region].texture;
        int tiling1 = regions[region].tiling;
        
        Texture2D texture2 = regions[region-1].texture;
        int tiling2 = regions[region-1].tiling;

        float u1 = (x / (float)chunkSize) * tiling1;
        float v1 = (y / (float)chunkSize) * tiling1;
        float u2 = (x / (float)chunkSize) * tiling2;
        float v2 = (y / (float)chunkSize) * tiling2;

        Color color1 = texture1.GetPixelBilinear(u1 % 1, v1 % 1);
        Color color2 = texture2.GetPixelBilinear(u2 % 1, v2 % 1);

        Color lerpedColor = Color.Lerp(color1, color2, 1 - (difference*smoothEdgesValue));
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
    public float height;
    public Color color;
    public Texture2D texture;
    public int tiling;
}