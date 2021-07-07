using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Conveyor : MonoBehaviour
{
    public float speed = 1f;
    Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate() {
        Vector3 pos = rb.position;
        rb.position -= transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }
}
