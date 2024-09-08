using UnityEngine;

namespace GameMap.Generator {
    public class MapDataManager : MonoBehaviour {
        [Header("Regions")]
        [SerializeField] TerrainType[] regions;
        
        public TerrainType[] GetTerrains() {
            return regions;
        }
    }
}