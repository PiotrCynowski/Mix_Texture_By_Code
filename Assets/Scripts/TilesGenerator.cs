using System.Collections.Generic;
using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    [SerializeField]  float tileSize;
    [SerializeField]  float spaceBetween = 1;
    [SerializeField]  int sideValueNumberOfTiles = 1;
    [SerializeField]  Material tileMaterialMain;
    [SerializeField]  Transform container;
    List<GameObject> tiles = new List<GameObject>();
    List<Material> tileExistMaterials = new List<Material>();

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
        
        GameObject TileObj = GetNewTile();
        float adjustedTileSize = tileSize + spaceBetween;
        for (int x = 0; x < sideValueNumberOfTiles; x++)
        {
            for (int z = 0; z < sideValueNumberOfTiles; z++)
            {
                Vector3 tilePos = new Vector3(x * adjustedTileSize, 0, z * adjustedTileSize);
                GameObject tile = Instantiate(TileObj, tilePos, Quaternion.identity, container);
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
        if (tileIndex >= tileExistMaterials.Count)
        {
            Debug.LogWarning($"Tile at index {tileIndex} does not exist");
            return;
        }
        tileExistMaterials[tileIndex].mainTexture = textureToAssign;
    }

    public int GetSideValueNumberOfTiles()
    {
        return sideValueNumberOfTiles;
    }

    public bool IsTilesReady()
    {
        return tileExistMaterials != null && tileExistMaterials.Count > 0;
    }
    
    #region create tile
    GameObject GetNewTile()
    {
        GameObject tileObj = new("tile");
        tileObj.AddComponent<MeshRenderer>();
        tileObj.AddComponent<MeshFilter>().mesh = GetNewTileMesh(tileSize);
        return tileObj;
    }
    
    Mesh GetNewTileMesh(float tileSize)
    {
        Mesh mesh = new();

        float halfWidth = tileSize * 0.5f;
        float halfHeight = tileSize * 0.5f;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-halfWidth, 0f, -halfHeight);
        vertices[1] = new Vector3(halfWidth, 0f, -halfHeight);
        vertices[2] = new Vector3(-halfWidth, 0f, halfHeight);
        vertices[3] = new Vector3(halfWidth, 0f, halfHeight);

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }
    #endregion
}