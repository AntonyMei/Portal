using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("The magnitude of jump force")]
    public float JumpForce = 5;
    [Tooltip("The maximum height when the player is judeged as grounded")]
    public float GroundError = 0.2f;

    [Header("Movement Settings")]
    [Tooltip("The camera attached to the player.(used for getting direction)")]
    public Camera PlayerCamera;
    [Tooltip("Speed of the player when LShift is not pressed")]
    public float NormalSpeed = 3f;
    [Tooltip("Speed boost when LShift is pressed")]
    public float Boost = 3f;

    void Update() {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space)) {
            Rigidbody rigidbody;
            if (rigidbody = GetComponent<Rigidbody>()) {
                rigidbody.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
            }
        }
        // Motion control
        Vector3 forward = new Vector3(PlayerCamera.transform.forward.x,
                                   0, PlayerCamera.transform.forward.z);
        Vector3 delta_pos = new Vector3();
        forward.Normalize();
        if(Input.GetKey(KeyCode.W)) { delta_pos += forward * NormalSpeed; }
        if(Input.GetKey(KeyCode.S)) { delta_pos -= forward * NormalSpeed; }
        if(Input.GetKey(KeyCode.A)) {
            var dir = Vector3.Cross(forward, new Vector3(0, 1, 0));
            dir.Normalize(); delta_pos += dir * NormalSpeed;
        }
        if(Input.GetKey(KeyCode.D)) {
            var dir = Vector3.Cross(forward, new Vector3(0, 1, 0));
            dir.Normalize(); delta_pos -= dir * NormalSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift)) { delta_pos *= Boost; } 
        delta_pos *= Time.deltaTime;
        transform.position += delta_pos;
    }
    public bool isGrounded() {
        return Physics.Raycast(transform.position, -transform.up, GroundError);
    }
}