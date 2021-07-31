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

        int maxAd = 5;

        // int streetInt = Random.Range(height / 3, height);
        // int streetWidth = Random.Range(1, 5);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightMap[x, y] > regions[0].height && heightMap[x, y] != 256)
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


                    newPosValue.heightMapValue = new Vector2(x, y);
                }
                else if (maxAd >= 0)
                {
                    // maxAd--;
                    int leftX = (x - 1 + width) % width;
                    int rightX = (x + 1) % width;
                    int aboveY = (y - 1 + height) % height;
                    int belowY = (y + 1) % height;
                    bool valid = true;

                    for (int h = leftX; h < rightX; h++)
                    {
                        for (int g = belowY; g < aboveY; g++)
                        {
                            if (heightMap[h, g] > regions[0].height && valid && heightMap[h, g] != heightMap[x, y])
                            {
                                Debug.Log(h + "/" + g);
                                valid = false;
                                break;
                            }
                        }

                    }
                    if (valid)
                    {
                        PosBuildingValues newPosValue = new PosBuildingValues();
                        newPosValue.heightMapValue = new Vector2(x, y);
                        heightMap[x, y] = 256;
                        newPosValue.currentBuildType = PosBuildingValues.buildType.ad;
                        newPosValue.pos = new Vector3(x + topLeftX, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, y - topLeftZ);
                        posValues.Add(newPosValue);
                    }
                    //if (heightMap[leftX, y] < regions[1].height && heightMap[rightX, y] < regions[1].height && heightMap[x, aboveY] < regions[1].height && heightMap[x, belowY] < regions[1].height)
                    //{
                    //    if (heightMap[leftX, aboveY] < regions[1].height && heightMap[leftX, belowY] < regions[1].height && heightMap[rightX, belowY] < regions[1].height && heightMap[rightX, aboveY] < regions[1].height)
                    //    {

                    //    }
                    //}




                }
            }
        }
        return posValues;
    }

}
