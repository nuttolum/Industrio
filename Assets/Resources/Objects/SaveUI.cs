using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class SaveUI : MonoBehaviour
{

    public Canvas saveCanvas;
    public Canvas loadCanvas;
    GameObject objToSave;
    CamSwitch camSwitch;
    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Objects"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Objects");
        }
        camSwitch = FindObjectOfType<CamSwitch>();
        saveCanvas.enabled = false;
        loadCanvas.enabled = false;
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G) && !loadCanvas.enabled)
        {
            RaycastHit hit;
            LayerMask Layermask = LayerMask.GetMask("factoryObj");
            if (Physics.Raycast(camSwitch.activeCamera.transform.position, camSwitch.activeCamera.transform.forward, out hit, 15f, Layermask))
            {
                if(hit.transform.parent == null) {
                    GameObject parent = new GameObject();
                    hit.transform.parent = parent.transform;
                }
                saveCanvas.enabled = true;
                objToSave = hit.transform.parent.gameObject;
                SaveObject.CenterParent(objToSave);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                camSwitch.focused = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.L) && !saveCanvas.enabled)
        {
            loadCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            camSwitch.focused = false;
        }
    }
    public void saveButton()
    {
        if (objToSave != null)
        {
            camSwitch.focused = true;
            SaveObject.Save(objToSave, saveCanvas.GetComponentInChildren<InputField>().text);
            objToSave = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            saveCanvas.enabled = false;
        }
    }
    public void loadButton()
    {
        camSwitch.focused = true;
        SaveObject.Load(loadCanvas.GetComponentInChildren<InputField>().text, camSwitch.activeCamera.transform.position + camSwitch.activeCamera.transform.parent.transform.forward * 10f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        loadCanvas.enabled = false;
    }
}
