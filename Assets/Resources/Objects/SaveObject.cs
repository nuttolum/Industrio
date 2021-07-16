using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
public class SaveObject : MonoBehaviour
{
    string json;
    public static void Save(GameObject objectToSave, string objectName)
    {
        CenterParent(objectToSave);
        ObjectBase saveObj = new ObjectBase();
        List<ObjectComponent> componentList = new List<ObjectComponent>();
        ProductionObject[] children = objectToSave.GetComponentsInChildren<ProductionObject>();
        foreach (ProductionObject child in children)
        {
            ObjectComponent component = new ObjectComponent();
            component.name = child.objectName;
            component.position = child.transform.localPosition;
            component.rotation = child.transform.rotation;
            componentList.Add(component);
        }
        saveObj.components = componentList.ToArray();
        string json = JsonUtility.ToJson(saveObj);
        string saveFile = Application.persistentDataPath + "/Objects/" + objectName + ".data";
        File.WriteAllText(saveFile, json);
    }

    public static void Load(string fileToLoad, Vector3 position)
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/Objects/" + fileToLoad + ".data");
        GameObject loadedObject = new GameObject();
        ObjectBase saveObj = JsonUtility.FromJson<ObjectBase>(json);
        List<GameObject> children = new List<GameObject>();
        foreach (ObjectComponent component in saveObj.components)
        {
            GameObject child = Instantiate(Resources.Load<GameObject>("Objects/" + component.name), loadedObject.transform.TransformPoint(component.position), component.rotation);
            child.name = component.name;
            child.transform.parent = loadedObject.transform;
            children.Add(child);

        }
        foreach (GameObject child in children)
        {
            foreach (GameObject childToConnect in children)
            {
                if (childToConnect != child)
                {
                    child.GetComponent<ProductionObject>().connectedObjects.Add(childToConnect.GetComponent<ProductionObject>());
                    FixedJoint joint = child.AddComponent<FixedJoint>();
                    joint.connectedBody = childToConnect.GetComponent<Rigidbody>();
                }
            }
        }
        loadedObject.transform.position = position;
    }
    public static void CenterParent(GameObject objectToSave)
    {
        List<Vector3> positions = new List<Vector3>();
        List<ProductionObject> children = objectToSave.GetComponentsInChildren<ProductionObject>().ToList();
        foreach (ProductionObject child in children)
        {
            positions.Add(child.transform.position);
            child.transform.parent = null;
        }
        Vector3 averagePos = GetMeanVector(positions);
        objectToSave.transform.position = averagePos;
        foreach (ProductionObject child in children)
        {
            child.transform.parent = objectToSave.transform;
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