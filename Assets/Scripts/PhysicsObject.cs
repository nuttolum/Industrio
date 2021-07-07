using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public void MoveObject(Vector3 direction, float speed) {
        GetComponent<Rigidbody>().velocity = direction * speed;
    }
}
