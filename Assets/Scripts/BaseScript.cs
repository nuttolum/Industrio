using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour
{
    public string state = "idle";
    public Camera cam;
    Camera PlayerCam;
    CamSwitch cameraSwitch;
    Vector3 camPos;
    Quaternion camRot;
    Coroutine currentCo;
    Arm[] arms;
    private int workingArms;
    GameObject move;
    List<GameObject> heldObjs = new List<GameObject>();

    void Start()
    {

        arms = GetComponentsInChildren<Arm>();
        PlayerCam = Camera.main;
        camPos = cam.transform.position;
        camRot = cam.transform.rotation;
        cameraSwitch = FindObjectOfType<CamSwitch>();
    }
    void Update()
    {
        foreach (Arm arm in arms)
        {
            if (arm.state == "moving")
            {
                workingArms++;
            }
        }
        if (workingArms == 0)
        {
            if(cameraSwitch.activeCamera == cam) {
                state = "editing";
            } else {
                state = "idle";
            }
        }
        else
        {
            if(cameraSwitch.activeCamera == cam) {
                state = "editing";
            } else {
                state = "working";
            }
            workingArms = 0;
        }
        switch (state)
        {
            case "unpowered":

                break;
            case "idle":

                foreach (Arm arm in arms)
                {
                    if (!heldObjs.Contains(arm.holdingObject)) heldObjs.Add(arm.holdingObject);
                }
                break;
            case "working":
                foreach (Arm arm in arms)
                {
                    if (!heldObjs.Contains(arm.holdingObject)) heldObjs.Add(arm.holdingObject);
                }
                break;
            case "editing":
                foreach(Arm arm in arms) {
                    if(arm.holdingObject != null) {
                    arm .holdPoint.rotation = arm.holdingObject.transform.rotation;
                    arm.holdPoint.position = arm.holdingObject.transform.position;
                    }
                }
                CheckForMovementClick();
                if (Input.GetKeyDown(KeyCode.Tab)) ExitCam();
                break;
            default:
                break;

        }
    }
        IEnumerator CameraCo(float duration)
        {
            state = "editing";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cam.transform.position = Camera.main.transform.position;
            cam.transform.rotation = Camera.main.transform.rotation;
            Vector3 startPos = cam.transform.position;
            Quaternion startRot = cam.transform.rotation;
            FindObjectOfType<CamSwitch>().Switch(cam);
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {

                float normalizedTime = t / duration;
                cam.transform.rotation = Quaternion.Lerp(startRot, camRot, normalizedTime);
                cam.transform.position = Vector3.Lerp(startPos, camPos, normalizedTime);
                yield return null;
            }
        }
        IEnumerator CameraExitCo(float duration)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Vector3 startPos = cam.transform.position;
            Quaternion startRot = cam.transform.rotation;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {

                float normalizedTime = t / duration;
                cam.transform.rotation = Quaternion.Lerp(startRot, PlayerCam.transform.rotation, normalizedTime);
                cam.transform.position = Vector3.Lerp(startPos, PlayerCam.transform.position, normalizedTime);
                yield return null;
            }
            FindObjectOfType<CamSwitch>().Switch(PlayerCam);
        }
    public virtual void SwitchCam()
    {
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(CameraCo(.5f));
    }
    public virtual void ExitCam()
    {
        if(move) Destroy(move);
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(CameraExitCo(.5f));
    }

    public void CheckForMovementClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                foreach(GameObject obj in heldObjs) {
                    print(obj);
                }
                if (hit.collider.tag == "factoryObj" && heldObjs.Contains(hit.collider.gameObject))
                {
                    print(FindObjectsOfType<MoveObject>().Length);
                    if(FindObjectsOfType<MoveObject>().Length == 0) {
                    move = Instantiate(Resources.Load<GameObject>("Move Prefab"), hit.collider.gameObject.transform);
                    print("w");
                    move.transform.parent = hit.collider.gameObject.GetComponent<ProductionObject>().holdingArm.targetPoint;
                    } else {
                        move.transform.parent = hit.collider.gameObject.GetComponent<ProductionObject>().holdingArm.targetPoint;
                    }
                }
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        var pickupObj = other.GetComponent<ProductionObject>();
        if (pickupObj)
        {
            print(pickupObj);
            state = "working";
            if (GetClosestArm(pickupObj.gameObject).holdingObject == null) GetClosestArm(pickupObj.gameObject).GrabObject(pickupObj);
        }
    }
    private Arm GetClosestArm(GameObject objectToTest)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = objectToTest.transform.position;
        foreach (Arm t in arms)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin.gameObject.GetComponent<Arm>();
    }
}
