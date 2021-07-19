using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Vector3 dir;

    void Update()
    {
        transform.Translate(dir);
    }
}
