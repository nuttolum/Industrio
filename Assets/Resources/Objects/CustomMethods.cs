using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomMethodsName
{
    public static class CustomMethods
    {
        public static void IgnoreCollisionBetweenObjects(GameObject home, GameObject other, bool toggled)
        {
            foreach (Collider col in home.GetComponentsInChildren<Collider>())
            {
                if (col.isTrigger == false)
                {
                    foreach (Collider otherCol in other.GetComponentsInChildren<Collider>())
                    {
                        if (otherCol.isTrigger == false)
                        {
                            Physics.IgnoreCollision(col, otherCol, toggled);
                        }
                    }
                }
            }
        }
        public static void CenterParent(GameObject objectToSave)
        {
            List<Vector3> positions = new List<Vector3>();
            List<GameObject> children = new List<GameObject>();
            foreach (ProductionObject childT in objectToSave.GetComponentsInChildren<ProductionObject>())
            {
                GameObject child = childT.gameObject;
                if (child != objectToSave && child.GetComponent<Rigidbody>())
                {
                    positions.Add(child.transform.position);

                }
                if (child.transform.parent == objectToSave.transform)
                {

                    children.Add(child);

                }
            }
            foreach (GameObject child in children)
            {
                child.transform.parent = null;
            }

            if (positions.Count == 0)
            {
                positions.Add(objectToSave.transform.position);
            }
            Vector3 averagePos = GetMeanVector(positions);
            objectToSave.transform.position = averagePos;
            foreach (GameObject child in children)
            {
                child.transform.parent = objectToSave.transform;
            }
        }
        public static Vector3 GetMeanVector(List<Vector3> positions)
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
}