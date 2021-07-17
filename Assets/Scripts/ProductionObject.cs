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
    void Awake() {
        connectedObjects.Add(this);
    }
    void Update() {
        if(parent == null) {
            parent = gameObject;
        }

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
    public void ToggleCollision(bool toggled) {
        foreach(ProductionObject obj in connectedObjects) {
        foreach(Collider col in obj.GetComponentsInChildren<Collider>()) {
            if(!col.isTrigger) {
                col.enabled = toggled;
            }
        }
        }
    }
}
