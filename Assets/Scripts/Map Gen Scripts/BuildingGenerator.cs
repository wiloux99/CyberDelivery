using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGenerator
{

    public static List<PosBuildingValues> GenerateBuildings(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, TerrainType[] regions)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int simplification = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        List<PosBuildingValues> posValues = new List<PosBuildingValues>();
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;


        // int streetInt = Random.Range(height / 3, height);
        // int streetWidth = Random.Range(1, 5);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightMap[x, y] > regions[0].height)
                {

                    //if(EndlessTerrain.IsBetween(y, streetInt - streetWidth, streetInt + streetWidth))
                    //{
                    //    break;
                    //}
                    PosBuildingValues newPosValue = new PosBuildingValues();

                    if (heightMap[x, y] < regions[1].height)
                    {
                        newPosValue.currentBuildType = PosBuildingValues.buildType.smallBuild;
                    }
                    else if (heightMap[x, y] >= regions[1].height && heightMap[x, y] < regions[2].height)
                    {
                        newPosValue.currentBuildType = PosBuildingValues.buildType.medBuild;
                    }
                    else if (heightMap[x, y] >= regions[2].height)
                    {
                        newPosValue.currentBuildType = PosBuildingValues.buildType.tallBuild;
                    }
                    newPosValue.pos = new Vector3(x + topLeftX, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, y - topLeftZ);

                    if (newPosValue.currentBuildType != PosBuildingValues.buildType.none)
                    posValues.Add(newPosValue);

                }
            }
        }
        return posValues;
    }

}
