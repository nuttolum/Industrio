using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        objectToGrab.holdingArm = this;
        holdingObject = objectToGrab.gameObject;
        objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
                foreach(ProductionObject obj in holdingObject.GetComponent<ProductionObject>().connectedObjects) {
            foreach(Collider col in obj.GetComponents<Collider>()) {
                if(!col.isTrigger)
                col.enabled = false;
            }
        }
        StartCoroutine(GrabObjectCo(objectToGrab.gameObject));
    }
    public void Drop()
    {
        state = "idle";
        StartCoroutine(DropCo());
    }
    public void DropInstant()
    {
        onDrop?.Invoke();
        state = "idle";
        if (holdingObject)
        {
            foreach (ProductionObject obj in holdingObject.GetComponent<ProductionObject>().connectedObjects)
            {
                foreach (Collider col in obj.GetComponents<Collider>())
                {
                    col.enabled = true;
                }
            }
            holdingObject.GetComponent<ProductionObject>().holdingArm = null;
            if (holdingObject.GetComponent<ProductionObject>().parent)
            {
                holdingObject.transform.parent = holdingObject.GetComponent<ProductionObject>().parent.transform;
            }
            else
            {
                holdingObject.transform.parent = null;
            }
            holdingObject = null;
            holdingRB.isKinematic = false;
            holdingRB = null;
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
        holdingObject = objectToGrab;
        holdingObject.transform.parent = targetPoint;
        holdingRB = holdingObject.GetComponent<Rigidbody>();
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
    public void Test()
    {
        return;
    }
}
