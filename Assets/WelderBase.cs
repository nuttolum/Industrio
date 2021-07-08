using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelderBase : MonoBehaviour
{
    public Arm arm1;
    public Arm arm2;
    public Arm arm3;
    public GameObject holdingPos;
    void Update() {
        if(arm3.moving == false && arm1.holding == true && arm2.holding == true && arm1.Obj.transform.position == arm1.holdingPosObj.transform.position && arm2.Obj.transform.position == arm2.holdingPosObj.transform.position) {
            GameObject newObj = Weld();
            newObj.GetComponent<Rigidbody>().isKinematic = true;
            print("ww");
            if(arm3.target.transform.position != newObj.transform.position) {
                arm3.Move(newObj.transform.position, 2f);
                print("moving to object");
            } else if (arm3.target.transform.position == newObj.transform.position) {
                print("dropping object");
                arm3.Obj = newObj;
                arm3.holding = true;
                arm1.DropInstant();
                arm2.DropInstant();
                arm3.Drop();
            }
        } else {
            if(arm3.holding == false && arm3.target.transform.position == arm3.dropPosObj.transform.position) {
                arm3.Move(holdingPos.transform.position, 2f);
                print("returning to neutral pos");
            }
        }
    }

    GameObject Weld() {
        GameObject obj1 = arm1.Obj;
        GameObject obj2 = arm2.Obj;
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
        Renderer[] boundsToCombine = new Renderer[obj1.GetComponentsInChildren<Renderer>().Length + obj2.GetComponentsInChildren<Renderer>().Length];
        obj1.GetComponentsInChildren<Renderer>().CopyTo(boundsToCombine, 0);
        obj2.GetComponentsInChildren<Renderer>().CopyTo(boundsToCombine, obj1.GetComponentsInChildren<Renderer>().Length);
        foreach(Renderer render in boundsToCombine) {
        combinedBounds.Encapsulate(render.bounds);
        }
        obj1.transform.SetParent(parent.transform, true);
        obj2.transform.SetParent(parent.transform, true);
                parent.AddComponent<BoxCollider>();
                parent.GetComponent<BoxCollider>().size = combinedBounds.size;
        return parent;

    }
}
