using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class ProductionObject : MonoBehaviour
{
    public Arm holdingArm;
    public GameObject parent = null;
    public bool root = true;
    public BoxCollider boundingBox;
    public List<ProductionObject> connectedObjects = new List<ProductionObject>();
    public string objectName;
    void OnValidate()
    {

    }
    void Start() {
        connectedObjects.Add(this);
    }
    public void Rebound()
    {
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }
        boundingBox.isTrigger = true;
        boundingBox.size = combinedBounds.size;
    }
}
