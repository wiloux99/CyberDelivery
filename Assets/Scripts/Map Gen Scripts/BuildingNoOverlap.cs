using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingNoOverlap : MonoBehaviour
{
    public GameObject[] itemsToPickFrom;
    public LayerMask spawnedObjectLayer;
    int randomIndex;
    BoxCollider boxCollider;
    Quaternion spawnRotation;

    // Start is called before the first frame update
    void Start()
    {
        PositionRaycast();
        transform.rotation = spawnRotation;

    }

    void PositionRaycast()
    {
        randomIndex = Random.Range(0, itemsToPickFrom.Length);
        //spawnRotation = Random.rotation;
        //spawnRotation.x = 0;
        //spawnRotation.z = 0;

        boxCollider = itemsToPickFrom[randomIndex].GetComponent<BoxCollider>();
        Vector3 overlapTestBoxScale = boxCollider.size;
        Collider[] collidersInsideOverlapBox = new Collider[1];
        int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(transform.position, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedObjectLayer);

        if (numberOfCollidersFound == 0)
        {
            Pick(transform.position, spawnRotation, randomIndex);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(boxCollider != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;


                Gizmos.DrawCube(Vector3.zero + boxCollider.center, boxCollider.size);
            
          //  Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }

    void Pick(Vector3 positionToSpawn, Quaternion rotationToSpawn, int Index)
    {
        GameObject clone = Instantiate(itemsToPickFrom[Index], positionToSpawn, rotationToSpawn);
    }
}
