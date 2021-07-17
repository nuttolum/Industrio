using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Arm : MonoBehaviour
{
    public Transform targetPoint;
    public Transform dropPoint;
    public GameObject holdingObject;
    public Transform holdPoint;
    public TMPro.TextMeshPro text;
    Rigidbody holdingRB;
    public float speed = 2f;
    public string state = "idle";
    public bool powered = true;
    float duration;
    public delegate void GrabObjectEvent();
    public delegate void StartMovingEvent();
    public delegate void StopMovingEvent();
    public delegate void DropEvent();
    public event GrabObjectEvent onGrabObject;
    public event StartMovingEvent onStartMoving;
    public event StopMovingEvent onStopMoving;
    public event DropEvent onDrop;
    void Start()
    {
        duration = 1 / speed;
    }
    void Update()
    {
        text.text = state;
        if (!powered)
        {
            return;
        }

    }
    public void GrabObject(ProductionObject objectToGrab)
    {
        state = "moving";
        holdingObject = objectToGrab.transform.root.gameObject;
        holdingObject.GetComponent<ProductionObject>().holdingArm = this;
        CenterParent(holdingObject);
        foreach (ProductionObject po in holdingObject.GetComponentsInChildren<ProductionObject>())
        {
            if (po.GetComponent<Rigidbody>()) po.GetComponent<Rigidbody>().isKinematic = true;
            po.ToggleCollision(false);
        }
        StartCoroutine(GrabObjectCo(holdingObject));
    }
    public void Drop()
    {
        print("drop");
        state = "idle";
        StartCoroutine(DropCo());
    }
    public void DropInstant()
    {

        if (holdingObject)
        {
            onDrop?.Invoke();
            state = "idle";
            print("w");
            foreach (ProductionObject po in holdingObject.GetComponentsInChildren<ProductionObject>())
            {
                if (po.GetComponent<Rigidbody>()) po.GetComponent<Rigidbody>().isKinematic = false;
                po.ToggleCollision(true);
            }
            CenterParent(holdingObject);
            holdingObject.GetComponent<ProductionObject>().holdingArm = null;
            holdingObject.GetComponent<ProductionObject>().transform.parent = null;
            holdingObject = null;
        }
    }
    public void MoveToPos(Vector3 position)
    {
        StartCoroutine(MoveToPosCo(position));
    }
    public void MoveWithRot(Vector3 position, Quaternion rotation)
    {
        StartCoroutine(MoveWithRotCo(position, rotation));
    }
    IEnumerator DropCo()
    {
        onStartMoving?.Invoke();
        state = "moving";
        Vector3 startPos = targetPoint.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float interval = t / duration;
            targetPoint.position = Vector3.Lerp(startPos, dropPoint.position, interval);
            yield return null;
        }
        onStopMoving?.Invoke();
        targetPoint.position = dropPoint.position;
        DropInstant();
        state = "idle";
    }
    IEnumerator GrabObjectCo(GameObject objectToGrab)
    {
        onStartMoving?.Invoke();
        state = "moving";
        Vector3 startPos = targetPoint.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float interval = t / duration;
            targetPoint.position = Vector3.Lerp(startPos, objectToGrab.transform.position, interval);
            yield return null;
        }
        onStopMoving?.Invoke();
        onGrabObject?.Invoke();
        targetPoint.position = objectToGrab.transform.position;
        holdingObject.GetComponent<ProductionObject>().parent.transform.parent = targetPoint;
        MoveWithRot(holdPoint.position, holdPoint.rotation);
    }

    IEnumerator MoveToPosCo(Vector3 position)
    {
        onStartMoving?.Invoke();
        state = "moving";
        Vector3 startPos = targetPoint.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float interval = t / duration;
            targetPoint.position = Vector3.Lerp(startPos, position, interval);
            yield return null;
        }
        state = "idle";
        targetPoint.position = position;
    }
    IEnumerator MoveWithRotCo(Vector3 position, Quaternion rotation)
    {
        onStartMoving?.Invoke();
        state = "moving";
        Vector3 startPos = targetPoint.position;
        Quaternion startRot = targetPoint.rotation;
        Quaternion startObjRot = holdingObject.transform.rotation;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float interval = t / duration;
            targetPoint.position = Vector3.Lerp(startPos, position, interval);
            targetPoint.rotation = Quaternion.Lerp(startRot, rotation, interval);
            holdingObject.transform.rotation = Quaternion.Lerp(startObjRot, rotation, interval);
            yield return null;
        }
        onStopMoving?.Invoke();
        state = "waiting";
        targetPoint.rotation = rotation;
        targetPoint.position = position;
    }
    public void CenterParent(GameObject objectToSave)
    {
        List<Vector3> positions = new List<Vector3>();
        List<ProductionObject> children = new List<ProductionObject>();
        foreach (ProductionObject child in objectToSave.GetComponentsInChildren<ProductionObject>())
        {
            if (child.gameObject != objectToSave && child.GetComponent<Rigidbody>())
            {
                positions.Add(child.transform.position);

            }
            if (child.transform.parent == objectToSave.transform)
            {

                children.Add(child);

            }
        }
        foreach (ProductionObject child in children)
        {
            child.transform.parent = null;
        }

        if (positions.Count == 0)
        {
            positions.Add(objectToSave.transform.position);
        }
        Vector3 averagePos = GetMeanVector(positions);
        objectToSave.transform.position = averagePos;
        foreach (ProductionObject child in children)
        {
            child.transform.parent = objectToSave.transform;
        }
    }
    private Vector3 GetMeanVector(List<Vector3> positions)
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

[Serializable]
public class ArmData
{
    public string name;
    public Vector3 holdPos;
    public Quaternion holdRot;

}
