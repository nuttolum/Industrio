using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weld : MonoBehaviour
{
    public GameObject firstObj;
    public GameObject secondObj;
    public GameObject WeldObjects() {
        GameObject rootPart = firstObj;
        GameObject childPart = secondObj;
        DestroyImmediate(childPart.GetComponent<ProductionObject>());
        rootPart.GetComponent<ProductionObject>().Rebound();
        var rootJoint = rootPart.AddComponent<FixedJoint>();
        var childJoint = childPart.AddComponent<FixedJoint>();
        rootJoint.connectedBody = childPart.GetComponent<Rigidbody>();
        childJoint.connectedBody = rootPart.GetComponent<Rigidbody>();
        childPart.transform.parent = rootPart.transform;
        return rootPart;

    }

}
