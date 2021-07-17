using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGenerator
{

    public static List<Vector3> GenerateBuildings(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, TerrainType[] regions)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int simplification = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        List<Vector3> allPos = new List<Vector3>();
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;


        int streetInt = Random.Range(height / 3, height);
        int streetWidth = Random.Range(1, 5);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightMap[x, y] > regions[0].height)
                {
                    
                    if(EndlessTerrain.IsBetween(y, streetInt - streetWidth, streetInt + streetWidth))
                    {
                        break;
                    }

                    allPos.Add(new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y));
                }
            }
        }

        return allPos;
    }

}
