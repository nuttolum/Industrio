using System.Collections.Generic;
using System;
using UnityEngine;
public class CustomSerialize : MonoBehaviour, ISerializationCallbackReceiver {
    // ComponentClass class that is used at runtime.
    // This is internal to the BehaviourWithTree class and is not serialized.
    public class ComponentClass {
        public Vector3 position;
        public Quaternion rotation;
        public List<ComponentClass> children = new List<ComponentClass>();
    }
    // ComponentClass class that we will use for serialization.
    [Serializable]
    public struct SerializableComponentClass {
        public Vector3 position;
        public Quaternion rotation;
        public int childCount;
        public int indexOfFirstChild;
    }
    // The root ComponentClass used for runtime tree representation. Not serialized.
    ComponentClass root = new ComponentClass();
    // This is the field we give Unity to serialize.
    public List<SerializableComponentClass> serializedComponentClasses;
    public void OnBeforeSerialize() {
        // Unity is about to read the serializedComponentClasses field's contents.
        // The correct data must now be written into that field "just in time".
        if (serializedComponentClasses == null) serializedComponentClasses = new List<SerializableComponentClass>();
        if (root == null) root = new ComponentClass ();
        serializedComponentClasses.Clear();
        AddComponentClassToSerializedComponentClasses(root);
        // Now Unity is free to serialize this field, and we should get back the expected 
        // data when it is deserialized later.
    }
    void AddComponentClassToSerializedComponentClasses(ComponentClass n) {
        var serializedComponentClass = new SerializableComponentClass () {
            position = n.position,
            rotation = n.rotation,
            childCount = n.children.Count,
            indexOfFirstChild = serializedComponentClasses.Count+1
        };
        serializedComponentClasses.Add (serializedComponentClass);
        foreach (var child in n.children)
        AddComponentClassToSerializedComponentClasses (child);
    }
    public void OnAfterDeserialize() {
        //Unity has just written new data into the serializedComponentClasses field.
        //let's populate our actual runtime data with those new values.
        if (serializedComponentClasses.Count > 0) {
            ReadComponentClassFromSerializedComponentClasses (0, out root);
        } else
        root = new ComponentClass ();
    }
    int ReadComponentClassFromSerializedComponentClasses(int index, out ComponentClass ComponentClass) {
        var serializedComponentClass = serializedComponentClasses [index];
        // Transfer the deserialized data into the internal ComponentClass class
        ComponentClass newComponentClass = new ComponentClass() {
            position = serializedComponentClass.position,
            rotation = serializedComponentClass.rotation,
            children = new List<ComponentClass> ()
        }
        ;
        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i != serializedComponentClass.childCount; i++) {
            ComponentClass childComponentClass;
            index = ReadComponentClassFromSerializedComponentClasses (++index, out childComponentClass);
            newComponentClass.children.Add (childComponentClass);
        }
        ComponentClass = newComponentClass;
        return index;
    }
    void saveObject() {

    }   


}
