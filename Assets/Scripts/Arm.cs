using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public GameObject target;
    public GameObject holdingPosObj;
    bool holding = false;
    GameObject Obj;
    void Start() {
        target.transform.position = holdingPosObj.transform.position;
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "factoryObj") {
            pickupCube(other.gameObject);
        }
    }
    void pickupCube(GameObject cube) {
        Obj = cube;
        cube.GetComponent<Rigidbody>().isKinematic = true;
        Vector3 holdingPos = holdingPosObj.transform.position;
        StartCoroutine(MoveTo(cube.transform.position, 2f));
    }
    void Update() {
        if(Obj != null) {
        if(Obj.transform.position == target.transform.position && holding == false) {
            holding = true;
            StartCoroutine(MoveTo(holdingPosObj.transform.position, 2f));
        } else if(holding == true) {
            Obj.transform.position = target.transform.position;
        }
        }
        }
    IEnumerator MoveTo(Vector3 endPos, float duration) {
        Vector3 startPos = target.transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            target.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime);
            yield return null;
        }
        target.transform.position = endPos;
    }

}
