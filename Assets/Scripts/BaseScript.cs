using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.UI;
public class BaseScript : MonoBehaviour
{
    public string state = "idle";
    public Camera cam;
    Camera PlayerCam;
    public Canvas controlPanel;
    public Canvas nameEdit;
    public string machineName;
    public string type;
    CamSwitch cameraSwitch;
    Vector3 camPos;
    Quaternion camRot;
    Coroutine currentCo;
    Arm[] arms;
    private int workingArms;
    public TMPro.TextMeshPro text;
    GameObject move;

    List<GameObject> heldObjs = new List<GameObject>();

    void Start()
    {
        Setup();
    }
    public void Setup()
    {

        nameEdit.enabled = false;
        controlPanel.enabled = false;
        arms = GetComponentsInChildren<Arm>();
        PlayerCam = Camera.main;
        camPos = cam.transform.position;
        camRot = cam.transform.rotation;
        if (!Directory.Exists(Application.persistentDataPath + "/Machines/" + type))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Machines/" + type);
        }
        if(machineName != null) {
            LoadData(machineName);
        }
        cameraSwitch = FindObjectOfType<CamSwitch>();
    }
    public void OpenControlPanel()
    {
        if (state != "working")
        {
            controlPanel.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraSwitch.focused = false;
        }
    }
    public void closeControlPanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controlPanel.enabled = false;
        cameraSwitch.focused = true;
    }
    void Update()
    {
        text.text = state;
        foreach (Arm arm in arms)
        {
            if (arm.state == "moving")
            {
                workingArms++;
            }
        }
        if (workingArms == 0 && state == "working")
        {
            state = "idle";
        }
        if (workingArms != 0 && state == "idle")
        {
            state = "working";
        }
        workingArms = 0;
        switch (state)
        {
            case "unpowered":

                break;
            case "paused":
                foreach (Arm arm in arms)
                {
                    if (!heldObjs.Contains(arm.holdingObject)) heldObjs.Add(arm.holdingObject);
                }
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
                foreach (Arm arm in arms)
                {
                    if (arm.holdingObject != null)
                    {
                        arm.holdPoint.rotation = arm.holdingObject.transform.rotation;
                        arm.holdPoint.position = arm.holdingObject.transform.position;
                    }
                }
                machineName = nameEdit.GetComponentInChildren<InputField>().text;
                CheckForMovementClick();
                if (Input.GetKeyDown(KeyCode.Tab)) ExitCam();
                break;
            default:
                break;

        }
    }
    IEnumerator CameraCo(float duration)
    {
        nameEdit.enabled = true;
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
        nameEdit.enabled = false;
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
        if (state == "paused")
        {
            closeControlPanel();
            state = "editing";
            if (currentCo != null) StopCoroutine(currentCo);
            currentCo = StartCoroutine(CameraCo(.5f));
        }
    }
    public void TogglePause()
    {
        if (state == "paused")
        {
            state = "idle";
        }
        else
        {
            state = "paused";
        }
        closeControlPanel();
    }
    public void SaveData()
    {
        List<ArmData> armDatas = new List<ArmData>();
        foreach (Arm arm in arms)
        {
            ArmData armData = new ArmData();
            armData.holdPos = arm.targetPoint.localPosition;
            armData.holdRot = arm.targetPoint.rotation;
            armData.name = arm.name;
            armDatas.Add(armData);
        }
        MachineData data = new MachineData();
        data.arms = armDatas.ToArray();
        data.name = machineName;
        data.type = type;
        string json = JsonUtility.ToJson(data);
        string saveFile = Application.persistentDataPath + "/Machines/" + type + "/" + machineName + ".data";
        File.WriteAllText(saveFile, json);
    }
    public void LoadData()
    {
        string saveFile = Application.persistentDataPath + "/Machines/" + type + "/" + machineName + ".data";
        string json = File.ReadAllText(saveFile);
        MachineData data = JsonUtility.FromJson<MachineData>(json);
        foreach (Arm child in arms)
        {
            foreach (ArmData arm in data.arms)
            {
                if (child.name == arm.name)
                {
                    child.holdPoint.localPosition = arm.holdPos;
                    child.holdPoint.rotation = arm.holdRot;
                    child.targetPoint.localPosition = arm.holdPos;
                    child.targetPoint.rotation = arm.holdRot;
                }
            }
        }
    }
        public void LoadData(string loadName)
    {
        string saveFile = Application.persistentDataPath + "/Machines/" + type + "/" + loadName + ".data";
        string json = File.ReadAllText(saveFile);
        MachineData data = JsonUtility.FromJson<MachineData>(json);
        foreach (Arm child in arms)
        {
            foreach (ArmData arm in data.arms)
            {
                if (child.name == arm.name)
                {
                    child.holdPoint.localPosition = arm.holdPos;
                    child.holdPoint.rotation = arm.holdRot;
                    child.targetPoint.localPosition = arm.holdPos;
                    child.targetPoint.rotation = arm.holdRot;
                }
            }
        }
    }
    public virtual void ExitCam()
    {

        state = "paused";
        if (move) Destroy(move);
        if (currentCo != null) StopCoroutine(currentCo);
        currentCo = StartCoroutine(CameraExitCo(.5f));
    }

    public void CheckForMovementClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("factoryObj");
            mask = ~mask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                print(hit.collider.gameObject);
                var hitArm = hit.collider.gameObject.GetComponent<Arm>();
                if(hitArm)
                if (arms.ToList().Contains(hitArm))
                {
                    print(FindObjectsOfType<MoveObject>().Length);
                    if (FindObjectsOfType<MoveObject>().Length == 0)
                    {
                        move = Instantiate(Resources.Load<GameObject>("Move Prefab"), hitArm.targetPoint);
                        print("w");
                        move.transform.parent = hitArm.targetPoint;
                    }
                    else
                    {
                        move.transform.position = hitArm.targetPoint.position;
                        move.transform.rotation = hitArm.targetPoint.rotation;
                        move.transform.parent = hitArm.targetPoint;
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
            if (pickupObj.connectedObjects.Count > 0)
            {
                foreach (ProductionObject connectedObj in pickupObj.connectedObjects)
                {
                    if (connectedObj.holdingArm != null)
                    {
                        return;
                    }
                }
            }
            print(pickupObj);
            if (state == "idle") state = "working";
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

public class MachineData
{
    public ArmData[] arms;
    public string name;
    public string type;
}