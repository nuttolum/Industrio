using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class ProductionObject : MonoBehaviour
{
    public Arm holdingArm;
    void OnValidate()
    {

    }
    public void Rebound()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }
        bc.isTrigger = true;
        bc.size = combinedBounds.size;
    }
}
