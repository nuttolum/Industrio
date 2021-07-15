using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    public Camera[] cameras;
    public Camera activeCamera;
    void Start()
    {
        cameras = FindObjectsOfType<Camera>();
        foreach(Camera camera in cameras) {
            if(camera == Camera.main) {
                activeCamera = camera;
                camera.gameObject.SetActive(true);
            } else {
                camera.gameObject.SetActive(false);
            }
        }
    }
    public void Switch(Camera cameraToSwitch) {
        foreach(Camera camera in cameras) {
            if(camera == cameraToSwitch) {
                activeCamera = camera;
                camera.gameObject.SetActive(true);
            } else {
                camera.gameObject.SetActive(false);
            }
        }
    }
}
