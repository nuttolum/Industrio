using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        default:
        break;
        
    }
    }
    void DropObject() {
        weldingArm.Drop();
        weldingArm.onGrabObject -= DropObject;
    }
    GameObject Weld() {
        GameObject rootPart = pickupArmLeft.holdingObject;
        GameObject childPart = pickupArmRight.holdingObject;
        pickupArmRight.DropInstant();
        pickupArmLeft.DropInstant();
        Destroy(childPart.GetComponent<ProductionObject>());
        rootPart.GetComponent<ProductionObject>().Rebound();
        var rootJoint = rootPart.AddComponent<FixedJoint>();
        var childJoint = childPart.AddComponent<FixedJoint>();
        rootJoint.connectedBody = childPart.GetComponent<Rigidbody>();
        childJoint.connectedBody = rootPart.GetComponent<Rigidbody>();
        childPart.transform.parent = rootPart.transform;
        return rootPart;

    }

    

}
