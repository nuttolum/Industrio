using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethodsName;
public class SplitterBase : BaseScript
{
    public Arm dropArmLeft;
    public Arm dropArmRight;
    public Arm pickupArm;

    void LateUpdate()
    {
        switch (state)
        {
            case "unpowered":
                break;
            case "idle":
                if (pickupArm.state == "waiting")
                {
                    GameObject[] newObjects = Split(pickupArm.holdingObject.GetComponent<ProductionObject>().parent);
                    print(newObjects.Length);
                    dropArmLeft.GrabObject(newObjects[0].GetComponent<ProductionObject>());
                    dropArmRight.GrabObject(newObjects[1].GetComponent<ProductionObject>());
                    dropArmLeft.onGrabObject += DropObjectLeft;
                    dropArmRight.onGrabObject += DropObjectRight;
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
    void DropObjectLeft()
    {
        dropArmLeft.Drop();
        dropArmLeft.onGrabObject -= DropObjectLeft;
    }
    void DropObjectRight()
    {
        dropArmRight.Drop();
        dropArmRight.onGrabObject -= DropObjectRight;
    }
    GameObject[] Split(GameObject splitObj)
    {
        List<GameObject> results = new List<GameObject>();
        Transform[] children = getImmediateChildren(splitObj.transform);
        foreach(FixedJoint joint in splitObj.GetComponentsInChildren<FixedJoint>()) {
            Destroy(joint);
        }
        foreach (Transform child in children)
        {
            child.parent = null;
        }
        Destroy(pickupArm.holdingObject);
        pickupArm.DropInstant();
        foreach (Transform child in children)
        {
            child.GetComponent<ProductionObject>().ReJoin(child.GetComponent<ProductionObject>());
            results.Add(child.gameObject);
        }
        CustomMethods.IgnoreCollisionBetweenObjects(results[0], results[1], true);
        return results.ToArray();

    }
    public static Transform[] getImmediateChildren(Transform transform)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.parent == transform && child.GetComponent<ProductionObject>()) children.Add(child);

        }
        return (children.ToArray());
    }
}
