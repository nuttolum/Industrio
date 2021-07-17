using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SaveObject : MonoBehaviour
{
    string data;
    static ObjectComponent setupObject(GameObject obj)
    {
        ObjectComponent saveObj = new ObjectComponent();
        if (obj.GetComponent<ProductionObject>())
        {
            saveObj.name = obj.GetComponent<ProductionObject>().objectName;
        }
        else
        {
            saveObj.name = obj.name;
        }
        saveObj.position = obj.transform.localPosition;
        saveObj.rotation = obj.transform.rotation;
        List<ObjectComponent> objectChildren = new List<ObjectComponent>();
        Transform[] children = getImmediateChildren(obj.transform);
        foreach (Transform child in children)
        {
            if (child.GetComponent<ProductionObject>())
            {
                objectChildren.Add(setupObject(child.gameObject));
            }
        }
        saveObj.children = objectChildren;
        return saveObj;
    }
    static GameObject loadObject(GameObject parent, ObjectComponent ObjToLoad, Vector3 position)
    {
        GameObject obj;
        if (ObjToLoad.name == "Joint")
        {
            obj = new GameObject();
            obj.AddComponent<JointObject>();
            obj.GetComponent<JointObject>().objectName = "Joint";
            if (parent != null)
            {
                obj.transform.position = parent.transform.TransformPoint(ObjToLoad.position);
            }
            else
            {
                obj.transform.position = position;
            }
            obj.transform.rotation = ObjToLoad.rotation;
        }
        else
        {
            if (parent != null)
            {
                obj = Instantiate(Resources.Load<GameObject>("Objects/" + ObjToLoad.name), parent.transform.TransformPoint(ObjToLoad.position), ObjToLoad.rotation);
            } else {
                obj = Instantiate(Resources.Load<GameObject>("Objects/" + ObjToLoad.name), position, ObjToLoad.rotation);
            }
        }
        if(parent != null) { 
            obj.transform.parent = parent.transform;
        }
        foreach (ObjectComponent childObj in ObjToLoad.children)
        {
            loadObject(obj, childObj, Vector3.zero);
        }
        return obj;

    }
    public static Transform[] getImmediateChildren(Transform transform)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            print(child + ": " + child.parent);
            if (child.parent == transform) children.Add(child);

        }
        return (children.ToArray());
    }



    public static void Save(GameObject objectToSave, string objectName)
    {
        ObjectComponent saveObj = setupObject(objectToSave);
        string saveFile = Application.persistentDataPath + "/Objects/" + objectName + ".xml";
        var serializer = new XmlSerializer(typeof(ObjectComponent));
        var stream = new FileStream(saveFile, FileMode.Create);
        serializer.Serialize(stream, saveObj);
        stream.Close();
    }

    public static void Load(string fileToLoad, Vector3 position)
    {
        string savePath = Application.persistentDataPath + "/Objects/" + fileToLoad + ".xml";
        var serializer = new XmlSerializer(typeof(ObjectComponent));
        var stream = new FileStream(savePath, FileMode.Open);
        ObjectComponent saveObj = serializer.Deserialize(stream) as ObjectComponent;
        stream.Close();
        GameObject loadedObj = loadObject(null, saveObj, position);
        ProductionObject[] children = loadedObj.GetComponentsInChildren<ProductionObject>();
        foreach (ProductionObject child in children)
        {
            foreach (ProductionObject childToConnect in children)
            {
                if (childToConnect != child && child.GetComponent<Rigidbody>() && childToConnect.GetComponent<Rigidbody>())
                {
                    child.connectedObjects.Add(childToConnect);
                    FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = childToConnect.GetComponent<Rigidbody>();
                }

            }
        }
    }
    public static void CenterParent(GameObject objectToSave)
    {
        List<Vector3> positions = new List<Vector3>();
        List<ProductionObject> children = objectToSave.GetComponentsInChildren<ProductionObject>().ToList();
        foreach (ProductionObject child in children)
        {
            if (child.transform.parent = objectToSave.transform)
            {
                positions.Add(child.transform.position);
                child.transform.parent = null;
            }
        }
        Vector3 averagePos = GetMeanVector(positions);
        objectToSave.transform.position = averagePos;
        foreach (ProductionObject child in children)
        {
            if (child.transform.parent = objectToSave.transform)
            {
                child.transform.parent = objectToSave.transform;
            }
        }

    }
    private static Vector3 GetMeanVector(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 meanVector = Vector3.zero;

        foreach (Vector3 pos in positions)
        {
            meanVector += pos;
        }

        return (meanVector / positions.Count);
    }
}