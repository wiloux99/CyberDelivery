using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap }
    public DrawMode drawMode;
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

    public const int mapChunckSize = 241/4;
    [Range(0, 6)]
    public int levelOfDetail;

    public GameObject Chunck;
    public List<Vector3> Pos = new List<Vector3>();

    [Space]
    public bool UpdateRealTime;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

    public void RequestMapData(System.Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(System.Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
    }

    private void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i<mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunckSize, mapChunckSize));
        }
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunckSize, mapChunckSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] ColourMap = new Color[mapChunckSize * mapChunckSize];
        for (int y = 0; y < mapChunckSize; y++)
        {
            for (int x = 0; x < mapChunckSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        ColourMap[y * mapChunckSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        if (Chunck != null)
        {
            DestroyImmediate(Chunck);
        }



        GameObject newChunck = new GameObject("Chunck");
        newChunck.transform.position = transform.position;
        newChunck.transform.localScale = new Vector3(-10, 10, 10);
        Chunck = newChunck;


        Pos = BuildingGenerator.GenerateBuildings(noiseMap, heightMultiplier, BuildingHeightCurve, levelOfDetail, regions);



        foreach (Vector3 position in Pos)
        {
            GameObject newBuilding = Instantiate(Buildings[Random.Range(0, Buildings.Count)], Chunck.transform);
            newBuilding.transform.localPosition = position;
        }

        return new MapData(noiseMap, ColourMap);

    }

    private void OnValidate()
    {

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public System.Action<T> callback;
        public T parameter;

        public MapThreadInfo(System.Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
