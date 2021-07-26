using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySystem : MonoBehaviour
{
    EndlessTerrain endlessTerrain;
    MapGenerator mapGen;
    public float maxSpawnRange = 100f;
    public GameObject player;
    public GameObject deliveryGoal;
    void Start()
    {
        endlessTerrain = FindObjectOfType<EndlessTerrain>();
        mapGen = FindObjectOfType<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 newPos = new Vector3(player.transform.position.x + Random.Range(0, maxSpawnRange), 0, player.transform.position.z + Random.Range(0, maxSpawnRange));
            //MapData _map = mapGen.GenerateMapData(newPos);
            EndlessTerrain.TerrainChunck terrain = endlessTerrain.SpawnChunck(newPos);
            Debug.Log(terrain.buildingPos.Count);
            int random = Random.Range(0, terrain.buildingPos.Count);
            newPos = terrain.buildingPos[random];
            Instantiate(deliveryGoal, newPos, Quaternion.identity);
        }
    }
}
