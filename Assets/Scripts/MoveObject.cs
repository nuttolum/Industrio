using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Material handleMat;
    public float sensitivity = .05f;
    GameObject heldHandle = null;
    public GameObject[] moveHandles;
    bool rotation = false;
    GameObject movingObj;


    void Update()
    {
        movingObj = transform.parent.gameObject;
        if(Input.GetKeyDown(KeyCode.R)) {
            rotation = !rotation;
        }

        if (Input.GetMouseButtonDown(0))
        {
            LayerMask Layermask = LayerMask.GetMask("Handle");
            Ray ray = FindObjectOfType<CamSwitch>().activeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Layermask))
            {
                if (hit.collider.tag == "moveHandle")
                {
                    heldHandle = hit.collider.gameObject;
                }
            }

        }
        if (Input.GetMouseButton(0) && heldHandle != null)
        {
            if(!rotation) {
            Vector3 direction = new Vector3(Mathf.Abs(heldHandle.transform.up.x), Mathf.Abs(heldHandle.transform.up.y), Mathf.Abs(heldHandle.transform.up.z));
            if (heldHandle.gameObject == moveHandles[0])
            {
                movingObj.transform.position += heldHandle.transform.up * Input.GetAxis("Mouse Y") * sensitivity;
            }
            else if (heldHandle.gameObject == moveHandles[1])
            {
                movingObj.transform.position += heldHandle.transform.up * -Input.GetAxis("Mouse X") * sensitivity;
            }
            else if (heldHandle.gameObject == moveHandles[2])
            {
                movingObj.transform.position += heldHandle.transform.up * Input.GetAxis("Mouse X") * sensitivity;
            }
            } else {
                movingObj.transform.Rotate(heldHandle.transform.up * Input.GetAxis("Mouse X"), Space.World);
            }
        }

    }
}


