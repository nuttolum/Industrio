using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using CustomMethodsName;
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
        if(pickupArmLeft.state == "waiting" && pickupArmRight.state == "waiting" && weldingArm.state == "idle") {
            GameObject newObject = Weld();
            weldingArm.GrabObject(newObject.GetComponent<ProductionObject>());
            weldingArm.onGrabObject += DropObject;
        }
        
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
        print("grabbed");
        weldingArm.onGrabObject -= DropObject;
        weldingArm.Drop();
        
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
        foreach(ProductionObject obj in Objects) {
            obj.parent.transform.parent = parent.transform;
            obj.parent = parent;
        }
        parent.GetComponent<ProductionObject>().parent = parent;
        parent.GetComponent<ProductionObject>().ReJoin(null);
        CustomMethods.IgnoreCollisionBetweenObjects(parent, parent, true);


        return rootPart;

    }

    

}
