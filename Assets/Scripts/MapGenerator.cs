using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode {NoiseMap, ColourMap}
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public float heightMultiplier;
    public AnimationCurve BuildingHeightCurve;

    public Vector2 offset;
    public List<GameObject> Buildings = new List<GameObject>();

    public TerrainType[] regions;

    public GameObject Chunck;
    public List<Vector3> Pos = new List<Vector3>();

    [Space]
    public bool UpdateRealTime;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] ColourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        ColourMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        if(Chunck != null)
        {
            DestroyImmediate(Chunck);
        }



        GameObject newChunck = new GameObject("Chunck");
        newChunck.transform.position = transform.position;
        newChunck.transform.localScale = new Vector3(-10,10,10) ;
        Chunck = newChunck;
        

        Pos = BuildingGenerator.GenerateBuildings(noiseMap, heightMultiplier, BuildingHeightCurve, regions);



        foreach(Vector3 position in Pos)
        {
            GameObject newBuilding = Instantiate(Buildings[Random.Range(0, Buildings.Count)], Chunck.transform);
            newBuilding.transform.localPosition = position;
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(ColourMap, mapWidth,mapHeight));
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }



}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
