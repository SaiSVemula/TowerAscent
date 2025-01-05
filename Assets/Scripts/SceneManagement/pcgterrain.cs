using UnityEngine;
/*This code is used to make the pcg map for the start cutscene
using randomisation, self made assets and perlin noise for natural bumps*/
public class pcgterrain : MonoBehaviour
{
    // These are the terrain sizes
    public int Wterrain = 1000;
    public int Lterrain = 1000;
    public int Hterrain = 50;
    // these are terain bump sizes
    public float noiseness = 0.003f;
    public float freqofbumps = 0.025f;
    public float heightofbump = 8f;
    // these are refrences to prefabs
    public GameObject rockPrefab1;
    public GameObject rockPrefab2;
    public GameObject treePrefab;
    // area where tower will be 
    public float squarewithnoprefab = 100f;

    private Terrain terrain; //the terrain

    void Start() //gets the terrain, sizes it up and then adds in perfabs
    {
        terrain = GetComponent<Terrain>();


        maketerrain(); // make terrain structure

        AddPrefab(rockPrefab1, 500); 
        AddPrefab(rockPrefab2, 300); // add in prefabs
        AddPrefab(treePrefab, 1000);
    }

    void maketerrain() // this method makes the terrain
    {
        TerrainData terrainData = terrain.terrainData;
        // heightmap of terrain
        terrainData.heightmapResolution = Wterrain + 1;
        terrainData.size = new Vector3(Wterrain, Hterrain, Lterrain); // sets size 

        float[,] heights = new float[Wterrain, Lterrain]; // wxl in a arrua

        for (int x = 0; x < Wterrain; x++) // loop though x points
        {
            for (int z = 0; z < Lterrain; z++) // loop sthrough z points (means we go to each point)
            {
                float baseHeight = Mathf.PerlinNoise(x * noiseness, z * noiseness);
                // using perlin noise to make up natural terrain 
                float bumps = Mathf.PerlinNoise(x * freqofbumps, z * freqofbumps) * heightofbump / Hterrain;
                heights[x, z] = Mathf.Clamp(baseHeight + bumps, 0f, 1f);
            }
        }

        terrainData.SetHeights(0, 0, heights); // sets the height to the terrain
    }

    void AddPrefab(GameObject addprefab, int numOfItems) // adds the prefab
    {
        if (addprefab == null || numOfItems <= 0) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainCenter = new Vector3(terrainData.size.x / 2, 0f, terrainData.size.z / 2);

        for (int i = 0; i < numOfItems; i++) // goes through amount of times we define to maek amount of prefabs
        {
            Vector3 postoaddprefab = randomPos(terrainData); // finds a random poaition 

            if (Vector3.Distance(postoaddprefab, terrainCenter) < squarewithnoprefab) continue; // check if we are in area where no prefabs allowed

            Quaternion applyrotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(addprefab, postoaddprefab, applyrotation, transform); // set the addprefab
        }
    }

    Vector3 randomPos(TerrainData terrainData)
    {
        float xcoord = Random.Range(0f, 1f);
        float zcoord = Random.Range(0f, 1f);

        float randomx = xcoord * terrainData.size.x;
        float randomz = zcoord * terrainData.size.z;
        float yofterrain = terrain.SampleHeight(new Vector3(randomx, 0f, randomz));

        return new Vector3(randomx, yofterrain, randomz);
    }
}
