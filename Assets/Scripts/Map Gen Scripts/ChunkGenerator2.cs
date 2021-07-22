using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator2 : MonoBehaviour
{
    public GameObject itemToSpread;
    public int numItemsToSpawn = 10;

    public float itemXSpread = 10;
    public float itemYSpread = 0;
    public float itemZSpread = 10;
    public Coroutine ik;
    public int currentItem;

    public Vector3 Origin;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCunck(Origin, numItemsToSpawn);
    }

    void SpawnCunck(Vector3 Origin, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            SpreadItem(Origin);
        }
    }

    void SpreadItem(Vector3 Origin)
    {
        Vector3 randPosition = new Vector3(Origin.x + Random.Range(-itemXSpread, itemXSpread), Origin.y + Random.Range(-itemYSpread, itemYSpread), Origin.z+ Random.Range(-itemZSpread, itemZSpread)) + transform.position;
        GameObject clone = Instantiate(itemToSpread, randPosition, itemToSpread.transform.rotation);
    }
}
