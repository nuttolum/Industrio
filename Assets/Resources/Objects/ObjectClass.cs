using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
 [XmlRoot("ObjectCollection")]
public class ObjectComponent {
    public List<ObjectComponent> children;
    [XmlAttribute("name")]
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}



