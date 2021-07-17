using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class JointObject : ProductionObject
{
    void Awake() {
        connectedObjects.Add(this);
        GetComponent<BoxCollider>().isTrigger = true;
    }
}