using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
public class WelderBase : BaseScript
{
    public Arm pickupArmLeft;
    public Arm pickupArmRight;
    public Arm weldingArm;

    void LateUpdate() {
        switch(state) {
        case "unpowered":
        break;
        case "idle":
        if(pickupArmLeft.state == "waiting" && pickupArmRight.state == "waiting") {
            GameObject newObject = Weld();
            weldingArm.GrabObject(newObject.GetComponent<ProductionObject>());
        }
        weldingArm.onGrabObject += DropObject;
        break;
        case "working":
        break;
        case "editing":
        break;
        case "paused":
        break;
        default:
        break;
        
    }
    }
    void DropObject() {
        weldingArm.Drop();
        weldingArm.onGrabObject -= DropObject;
    }
    GameObject Weld() {
        GameObject parent = new GameObject();
        parent.AddComponent<JointObject>();
        parent.GetComponent<JointObject>().objectName = "Joint";
        parent.layer = LayerMask.NameToLayer("jointObject");
        GameObject rootPart = pickupArmRight.holdingObject;
        pickupArmLeft.holdingObject.GetComponent<ProductionObject>().connectedObjects.Add(pickupArmRight.holdingObject.GetComponent<ProductionObject>());
        pickupArmRight.holdingObject.GetComponent<ProductionObject>().connectedObjects.Add(pickupArmLeft.holdingObject.GetComponent<ProductionObject>());
        List<ProductionObject> Objects = new List<ProductionObject>();
        Objects = pickupArmLeft.holdingObject.GetComponent<ProductionObject>().connectedObjects.Union(pickupArmRight.holdingObject.GetComponent<ProductionObject>().connectedObjects).ToList();
        pickupArmRight.DropInstant();
        pickupArmLeft.DropInstant();
        List<GameObject> gameObjects = new List<GameObject>();
        foreach(ProductionObject obj in Objects) {
            gameObjects.Add(obj.gameObject);
            obj.parent.transform.parent = parent.transform;
            obj.parent = parent;
        }
        foreach (GameObject child in gameObjects)
        {
            foreach (GameObject childToConnect in gameObjects)
            {
                if(childToConnect != child) {
                FixedJoint joint = child.AddComponent<FixedJoint>();
                joint.connectedBody = childToConnect.GetComponent<Rigidbody>();
                }
            }
            
        }


        return rootPart;

    }

    

}
