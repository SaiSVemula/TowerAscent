using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 4096;
    public int terrainLength = 4096;
    public int terrainHeight = 200;
    public float noiseScale = 0.003f;

    [Header("Bump Settings")]
    public float bumpFrequency = 0.01f;
    public float bumpHeight = 15f;

    [Header("Rock Placement")]
    public GameObject rockPrefab1;
    public GameObject rockPrefab2;
    public int numberOfRocks1 = 500;
    public int numberOfRocks2 = 300;

    [Header("Tree Placement")]
    public GameObject treePrefab;
    public int numberOfTrees = 1000;

    [Header("Flower Placement")]
    public GameObject flowerPrefab1;
    public GameObject flowerPrefab2;
    public int numberOfFlowers1 = 800;
    public int numberOfFlowers2 = 800;

    [Header("Bush Placement")]
    public GameObject bushPrefab;
    public int numberOfBushes = 400;

    [Header("Center Area Exclusion Settings")]
    public float exclusionRadius = 500f; // Radius of the excluded central area.

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

        // Place objects with the exclusion logic
        PlaceObjects(rockPrefab1, numberOfRocks1);
        PlaceObjects(rockPrefab2, numberOfRocks2);
        PlaceObjects(treePrefab, numberOfTrees);
        PlaceObjects(flowerPrefab1, numberOfFlowers1);
        PlaceObjects(flowerPrefab2, numberOfFlowers2);
        PlaceObjects(bushPrefab, numberOfBushes);
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
                // Base Perlin noise for general height variation
                float baseHeight = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);

                // Additional bumps for small hills
                float bumps = Mathf.PerlinNoise(x * bumpFrequency, z * bumpFrequency) * bumpHeight / terrainHeight;

                heights[x, z] = Mathf.Clamp(baseHeight + bumps, 0f, 1f);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void PlaceObjects(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0)
        {
            Debug.LogWarning("Prefab or count invalid for placement.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        // Center of the terrain
        Vector3 terrainCenter = new Vector3(terrainData.size.x / 2, 0f, terrainData.size.z / 2);

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(0f, 1f);
            float z = Random.Range(0f, 1f);

            float worldX = x * terrainData.size.x;
            float worldZ = z * terrainData.size.z;
            float y = terrain.SampleHeight(new Vector3(worldX, 0f, worldZ));

            Vector3 position = new Vector3(worldX, y, worldZ);

            // Exclude objects from the center area
            if (Vector3.Distance(position, terrainCenter) < exclusionRadius)
            {
                continue; // Skip placing objects in the center
            }

            // Slightly randomize rotation for variation
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            Instantiate(prefab, position, rotation, this.transform);
        }
    }
}
