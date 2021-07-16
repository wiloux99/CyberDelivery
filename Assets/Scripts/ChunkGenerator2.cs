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

    // Start is called before the first frame update
    void Start()
    {

            StartCoroutine(SpreadItem());
    }

    IEnumerator SpreadItem()
    {
        while (currentItem < numItemsToSpawn)
        {
            Vector3 randPosition = new Vector3(Random.Range(-itemXSpread, itemXSpread), Random.Range(-itemYSpread, itemYSpread), Random.Range(-itemZSpread, itemZSpread)) + transform.position;
            GameObject clone = Instantiate(itemToSpread, randPosition, itemToSpread.transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }

    }
}
