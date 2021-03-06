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

    public const int mapChunckSize = 241 / 10;
    [Range(0, 6)]
    public int levelOfDetail;

    public GameObject Chunck;
    public List<PosBuildingValues> Pos = new List<PosBuildingValues>();

    [Space]
    public bool UpdateRealTime;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<PosThreadInfo<PosData>> posDataThreadInfoQueue = new Queue<PosThreadInfo<PosData>>();

    public void RequestMapData(System.Action<MapData> callback, Vector2 centre, GameObject _object, EndlessTerrain.TerrainChunck terrain = null)
    {
        ThreadStart threadStart = delegate
        {


            MapDataThread(callback, centre, _object, terrain);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(System.Action<MapData> callback, Vector2 centre, GameObject _object, EndlessTerrain.TerrainChunck terrain = null)
    {
        MapData mapData = GenerateMapData(centre);
        mapData.gameObject = _object;

        if (terrain != null)
        {
            mapData.terrain = terrain;
        }

        lock (mapDataThreadInfoQueue)
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
    }

    public void RequestPositionData(MapData mapData, System.Action<PosData> callback)
    {

    }

    //void positiondatathread(mapdata mapdata, system.action<posdata> callback)
    //{
    //    posdata newposdata = new posdata();
    //    newposdata.pos = mapdata.pos;
    //    lock(posdatathreadinfoqueue)
    //    {
    //        posdatathreadinfoqueue.enqueue(new mapthreadinfo<posdata>(callback, newposdata));
    //    }
    //}

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        //if(posdatathreadinfoqueue.count > 0)
        //{
        //    for(int i = 0; i <mapdatathreadinfoqueue.count; i++)
        //    {
        //        mapthreadinfo<posdata> threadinfo = posdatathreadinfoqueue.dequeue();
        //        threadinfo.callback(threadinfo.parameter);
        //    }
        //}
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
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

    public MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunckSize, mapChunckSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset);

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

        //if (Chunck != null)
        //{
        //    DestroyImmediate(Chunck);
        //}



        //GameObject newChunck = new GameObject("Chunck");
        //newChunck.transform.position = transform.position;
        //newChunck.transform.localScale = new Vector3(-10, 10, 10);
        //Chunck = newChunck;


        Pos = BuildingGenerator.GenerateBuildings(noiseMap, heightMultiplier, BuildingHeightCurve, levelOfDetail, regions);



        return new MapData(noiseMap, ColourMap, Pos, Buildings, null);

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
    struct PosThreadInfo<T>
    {
        public System.Action<T> callback;
        public T parameter;

        public PosThreadInfo(System.Action<T> callback, T parameter)
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
    public readonly List<PosBuildingValues> values;
    public readonly List<GameObject> buildings;
    public GameObject gameObject;
    public EndlessTerrain.TerrainChunck terrain;

    public MapData(float[,] heightMap, Color[] colourMap, List<PosBuildingValues> values, List<GameObject> buildings, GameObject _gameObject, EndlessTerrain.TerrainChunck terrain = null)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
        this.values = values;
        this.buildings = buildings;
        this.gameObject = _gameObject;
        this.terrain = terrain;
    }
}

public class PosBuildingValues
{
    public Vector3 pos;
    public enum buildType
    {
        none,smallBuild, medBuild, tallBuild
    };

    public buildType currentBuildType = buildType.none;
}

public struct PosData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;
    public List<Vector3> pos;
    public List<GameObject> buildings;

    public PosData(float[,] heightMap, Color[] colourMap, List<Vector3> pos, List<GameObject> buildings)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
        this.pos = pos;
        this.buildings = buildings;
    }
}
