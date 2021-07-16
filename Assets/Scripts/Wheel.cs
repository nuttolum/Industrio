using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public GameObject wheel;
    public float torque = 200f;
    public float direction;
    public WheelCollider wc;
    

    void Update()
    {
        float thrustTorque = Mathf.Clamp(direction, -1, 1) * torque;
        wc.motorTorque = thrustTorque;
    }
}
