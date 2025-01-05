using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    public int terrainWidth = 4096;
    public int terrainLength = 4096;
    public int terrainHeight = 200;
    public float noiseScale = 0.003f;

    public float bumpFrequency = 0.01f;
    public float bumpHeight = 15f;
    public GameObject rockPrefab1;
    public GameObject rockPrefab2;
    public GameObject treePrefab;
    public int rockCount1 = 500;
    public int rockCount2 = 300;
    public int treeCount = 1000;
    public float exclusionRadius = 500f;

    private Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("No Terrain component found on this GameObject!");
            return;
        }

        GenerateTerrain();

        // Place objects with exclusion logic
        PlaceObjects(rockPrefab1, rockCount1);  // Directly specify counts
        PlaceObjects(rockPrefab2, rockCount2);
        PlaceObjects(treePrefab, treeCount);
    }

    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

        float[,] heights = new float[terrainWidth, terrainLength];

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainLength; z++)
            {
                float baseHeight = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                float bumps = Mathf.PerlinNoise(x * bumpFrequency, z * bumpFrequency) * bumpHeight / terrainHeight;
                heights[x, z] = Mathf.Clamp(baseHeight + bumps, 0f, 1f);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void PlaceObjects(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainCenter = new Vector3(terrainData.size.x / 2, 0f, terrainData.size.z / 2);

        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetRandomPosition(terrainData);

            if (Vector3.Distance(position, terrainCenter) < exclusionRadius) continue;

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(prefab, position, rotation, transform);
        }
    }

    Vector3 GetRandomPosition(TerrainData terrainData)
    {
        float x = Random.Range(0f, 1f);
        float z = Random.Range(0f, 1f);

        float worldX = x * terrainData.size.x;
        float worldZ = z * terrainData.size.z;
        float y = terrain.SampleHeight(new Vector3(worldX, 0f, worldZ));

        return new Vector3(worldX, y, worldZ);
    }
}
