using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public Canvas canvas;
    string state = "nan";
    public GameObject obj;
    public CamSwitch camSwitch;
    void Start()
    {
        camSwitch = FindObjectOfType<CamSwitch>();
    }
    public void Spawn(string objName)
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("factoryObj");
        mask = ~mask;
        if (Physics.Raycast(camSwitch.activeCamera.transform.position, camSwitch.activeCamera.transform.forward, out hit, Mathf.Infinity, mask))
        {
            state = "placing";
            canvas.enabled = !canvas.enabled;
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            Cursor.visible = !Cursor.visible;
            camSwitch.focused = !Cursor.visible;
            obj = Instantiate(Resources.Load<GameObject>("Machines/" + objName), hit.point, Quaternion.identity);
        }
    }
    void Update()
    {
        switch (state)
        {
            case "nan":
                if (Input.GetKeyDown(KeyCode.E))
                {
                    canvas.enabled = !canvas.enabled;
                    if (Cursor.lockState == CursorLockMode.None)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                    Cursor.visible = !Cursor.visible;
                    camSwitch.focused = !Cursor.visible;
                }
                break;
            case "placing":
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("Machine");
                mask = ~mask;
                if (Physics.Raycast(camSwitch.activeCamera.transform.position, camSwitch.activeCamera.transform.forward, out hit, Mathf.Infinity, mask))
                {
                    obj.transform.position = hit.point;
                }
                obj.transform.eulerAngles += Quaternion.AngleAxis(Input.mouseScrollDelta.y, Vector3.up).eulerAngles;
                if (Input.GetMouseButtonDown(0))
                {
                    obj = null;
                    state = "nan";
                }
                break;
            default:
                break;
        }
    }
}
