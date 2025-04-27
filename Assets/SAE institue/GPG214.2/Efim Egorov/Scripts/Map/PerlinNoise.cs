using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public Terrain terrain;
    public int width = 512;
    public int height = 512;
    public float scale = 20;
    public float heightMulti = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyNoiseToTerrain();
    }

    public void ApplyNoiseToTerrain()
    {
        if(terrain == null)
        {
            Debug.Log("No terrain");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        int terrainWidth = terrainData.heightmapResolution;
        int terrainHeight = terrainData.heightmapResolution;
        float[,] heights = new float[terrainWidth, terrainHeight];

        for(int y = 0; y < terrainWidth; y++)
        {
            for( int x = 0; x < terrainHeight; x++)
            {
                float xCoordinate = (float)x / terrainWidth * scale;
                float yCoordinate = (float)y / terrainHeight * scale;
                float sample = Mathf.PerlinNoise(xCoordinate, yCoordinate);

                heights[x, y] = sample * heightMulti / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0,0,heights);
    }
}
