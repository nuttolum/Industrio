using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public GameObject target;
    public GameObject holdingPosObj;
    public bool moving = false;
    public bool holding = false;
    public GameObject dropPosObj;
    Vector3 dropPos;
    public GameObject Obj;
    void Start() {
        dropPos = dropPosObj.transform.position;
        target.transform.position = holdingPosObj.transform.position;
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "factoryObj" && holding == false) {
            Obj = other.gameObject;
            pickupObj(Obj);
        }
    }
    void pickupObj(GameObject pickupObj) {
        pickupObj.GetComponent<Rigidbody>().isKinematic = true;
        Vector3 holdingPos = holdingPosObj.transform.position;
        Move(pickupObj.transform.position, 2f);
    }

    void Update() {
        if(Obj != null) {
        if(Obj.transform.position == target.transform.position && holding == false) {
            holding = true;
            Move(holdingPosObj.transform.position, 2f);
        } else if(holding == true) {
            Obj.transform.position = target.transform.position;
        } else if(holding == true && Obj.transform.position == dropPos) {
            DropInstant();
        }
        }
        }
    IEnumerator MoveCo(Vector3 endPos, float duration) {
        moving = true;
        Vector3 startPos = target.transform.position;
        Quaternion startRot = target.transform.rotation;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {

            float normalizedTime = t / duration;
            if(Obj) {
            Quaternion desiredRot = Quaternion.LookRotation(target.transform.position - Obj.transform.position);
            target.transform.rotation = Quaternion.Lerp(startRot, desiredRot, normalizedTime);
            }
            target.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime);
            yield return null;
        }
        moving = false;
        target.transform.position = endPos;
    }
    public void Move(Vector3 endPos, float duration) {
        StartCoroutine(MoveCo(endPos, duration));
    }
    public void DropInstant() {
        print("w");
        holding = false;
        if(Obj.GetComponent<Rigidbody>() != null) Obj.GetComponent<Rigidbody>().isKinematic = false;
        Obj = null;
        
    }
    public void Drop() {
        StartCoroutine(DropCo(2f));
    }
    IEnumerator DropCo(float duration) {
        moving = true;
        Vector3 startPos = target.transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            target.transform.position = Vector3.Lerp(startPos, dropPos, normalizedTime);
            yield return null;
        }
        print("dropping instantly");
        target.transform.position = dropPos;
        DropInstant();
        moving = false; 
    }
    IEnumerator RotateObj(Vector3 desiredEuler, float duration) {
        Vector3 startRot = Obj.transform.eulerAngles;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Obj.transform.eulerAngles = Vector3.Lerp(startRot, desiredEuler, normalizedTime);
            yield return null;
        }
        target.transform.eulerAngles = desiredEuler;
    }

}
