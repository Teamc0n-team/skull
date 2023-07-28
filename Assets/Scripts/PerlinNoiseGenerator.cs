using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public int Width = 256;
    public int Height = 256;

    public int Depth = 20;

    public float Scale = 10f;

    private void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData data)
    {
        data.size = new Vector3(Width, Depth, Height);

        data.SetHeights(0, 0, GenerateHeights());

        return data;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[Width, Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / Width * Scale;
        float yCoord = (float)y / Height * Scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
