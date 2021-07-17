using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheeldropper : MonoBehaviour
{
    public GameObject wheel;
    void Update() {
        if(Input.GetKeyDown(KeyCode.T)) {
            var instance = Instantiate(wheel, transform.position, Quaternion.identity);
            print(instance.transform.position);
            instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y + 3f, instance.transform.position.z);
        }
    }
}
