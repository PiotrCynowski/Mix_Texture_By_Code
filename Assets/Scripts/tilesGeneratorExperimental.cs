using System.Collections.Generic;
using UnityEngine;

public class tilesGeneratorExperimental : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize;
    public float spaceBetween = 1;
    public Material tileMaterialMain;

    public int sideValueNumberOfTiles = 1;

    public List<GameObject> tiles;
    public List<Material> tileExistMaterials = new List<Material>();

    public void GenerateTilesMatrix()
    {
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile);
        }
        tiles.Clear();
        tileExistMaterials.Clear();
        
        int totalTiles = sideValueNumberOfTiles * sideValueNumberOfTiles;
        tiles.Capacity = totalTiles;
        tileExistMaterials.Capacity = totalTiles;
        
        float adjustedTileSize = tileSize + spaceBetween;
        for (int x = 0; x < sideValueNumberOfTiles; x++)
        {
            for (int z = 0; z < sideValueNumberOfTiles; z++)
            {
                Vector3 tilePos = new Vector3(x * adjustedTileSize, 0, z * adjustedTileSize);
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, transform);
                tiles.Add(tile);
                
                Renderer tileRenderer = tile.GetComponent<Renderer>();
                Material tileMat = new Material(tileMaterialMain);
                tileRenderer.material = tileMat;

                tileExistMaterials.Add(tileMat);
            }
        }
        tileExistMaterials.Reverse();
    }

    public void AssignTextureToTile(Texture2D textureToAssign, int tileIndex)
    {
        tileExistMaterials[tileIndex].mainTexture = textureToAssign;
    }
}