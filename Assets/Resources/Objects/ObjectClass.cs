using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ObjectBase {

    public ObjectComponent[] components;
}
[Serializable]
public class ObjectComponent {
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}


