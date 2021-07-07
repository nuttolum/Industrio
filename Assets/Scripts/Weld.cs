using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weld : MonoBehaviour
{
    public GameObject firstObj;
    public GameObject secondObj;
    void Start() {
        WeldObjects(firstObj, secondObj);
    }
    public void WeldObjects(GameObject obj1, GameObject obj2) {
        obj1.tag = "Untagged";
        obj2.tag = "Untagged";
        GameObject parent = new GameObject();
        parent.tag = "factoryObj";
        parent.transform.position = (obj1.transform.position + obj2.transform.position) / 2f;
        parent.transform.SetParent(null, true);
        Destroy(obj1.GetComponent<Rigidbody>());
        Destroy(obj2.GetComponent<Rigidbody>());
        parent.AddComponent<Rigidbody>();

        Bounds combinedBounds = new Bounds(parent.transform.position, Vector3.zero);
        combinedBounds.Encapsulate(obj1.GetComponent<Renderer>().bounds);
        combinedBounds.Encapsulate(obj2.GetComponent<Renderer>().bounds);
        obj1.transform.SetParent(parent.transform, true);
        obj2.transform.SetParent(parent.transform, true);
                parent.AddComponent<BoxCollider>();
                parent.GetComponent<BoxCollider>().size = combinedBounds.size;


    }

}
