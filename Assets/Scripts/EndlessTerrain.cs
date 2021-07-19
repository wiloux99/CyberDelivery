using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDist = 450 / 15;
    public Transform viewer;


    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    public int chunckSize;
    int chunckVisibleInViewDist;

    Dictionary<Vector2, TerrainChunck> terrainChunckDictionary = new Dictionary<Vector2, TerrainChunck>();
    List<TerrainChunck> terrainChuncksVisibleLastUpdate = new List<TerrainChunck>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunckSize = MapGenerator.mapChunckSize - 1;
        chunckVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunckSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChuncks();
    }

    public static bool IsBetween(double testValue, double bound1, double bound2)
    {
        if (bound1 > bound2)
            return testValue >= bound2 && testValue <= bound1;
        return testValue >= bound1 && testValue <= bound2;
    }

    void UpdateVisibleChuncks()
    {
        for (int i = 0; i < terrainChuncksVisibleLastUpdate.Count; i++)
        {
            terrainChuncksVisibleLastUpdate[i].SetVisible(false);
        }

        terrainChuncksVisibleLastUpdate.Clear();
        int currentChunckCoordX = Mathf.RoundToInt(viewerPosition.x / chunckSize);
        int currentChunckCoordY = Mathf.RoundToInt(viewerPosition.y / chunckSize);

        for (int yOffset = -chunckVisibleInViewDist; yOffset <= chunckVisibleInViewDist;  yOffset++)
        {
            for (int xOffset = -chunckVisibleInViewDist; xOffset <= chunckVisibleInViewDist; xOffset++) 
            {
                Vector2 viewedChunckCoord = new Vector2(currentChunckCoordX + xOffset, currentChunckCoordY + yOffset);

                if (terrainChunckDictionary.ContainsKey(viewedChunckCoord))
                {
                    terrainChunckDictionary[viewedChunckCoord].UpdateChunck();

                    if (terrainChunckDictionary[viewedChunckCoord].isVisible())
                    {
                        terrainChuncksVisibleLastUpdate.Add(terrainChunckDictionary[viewedChunckCoord]);
                    }
                }
                else
                {
                    terrainChunckDictionary.Add(viewedChunckCoord, new TerrainChunck(viewedChunckCoord, chunckSize, transform));
                }
            }
        }
    }

    public class TerrainChunck
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;


        public TerrainChunck(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);


            meshObject = new GameObject("CityChunck");
            //meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;


            //   OnPositionDataReceived(mapData, meshObject);


            SetVisible(false);
            //   Debug.Log(meshObject);
            mapGenerator.RequestMapData(OnMapDataReceived, position, meshObject);
        }

        void OnMapDataReceived(MapData posData)
        {
            foreach (PosBuildingValues values in posData.values)
            {
                //i++;
                GameObject newBuilding;
                switch (values.currentBuildType)
                {

                    case PosBuildingValues.buildType.smallBuild:
                        newBuilding = Instantiate(posData.buildings[0], posData.gameObject.transform);
                        newBuilding.name = "small";
                        newBuilding.transform.localPosition = values.pos;
                        break;
                    case PosBuildingValues.buildType.medBuild:
                        newBuilding = Instantiate(posData.buildings[1], posData.gameObject.transform);
                        newBuilding.name = "medium";
                        newBuilding.transform.localPosition = values.pos;
                        break;
                    case PosBuildingValues.buildType.tallBuild:
                        newBuilding = Instantiate(posData.buildings[2], posData.gameObject.transform);
                        newBuilding.name = "tall";
                        newBuilding.transform.localPosition = values.pos;
                        break;
                }
            }
        }

        void OnPositionDataReceived(MapData posData, GameObject _meshObject = null)
        {
        }

        public void UpdateChunck()
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }


        public bool isVisible()
        {
            return meshObject.activeSelf;
        }

    }
}
