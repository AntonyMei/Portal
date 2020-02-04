using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("The magnitude of jump force")]
    public float JumpForce = 5;
    [Tooltip("The maximum height when the player is judeged as grounded")]
    public float OverLapCapsuleOffset = 0.2f;
    [Tooltip("Minimal time between two jumps(Seconds)")]
    public float MinimalDeltaTime = 0.1f;

    [Header("Movement Settings")]
    [Tooltip("The camera attached to the player.(used for getting direction)")]
    public Camera PlayerCamera;
    [Tooltip("Speed of the player when LShift is not pressed")]
    public float NormalSpeed = 3f;
    [Tooltip("Speed boost when LShift is pressed")]
    public float Boost = 3f;

    //Those are variants related to ground detection
    private CapsuleCollider CapsuleCollider;
    private float LastJumpTime;

    void Awake() {
        CapsuleCollider = GetComponent<CapsuleCollider>();
        LastJumpTime = Time.time;
    }
    void Update() {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && OnGround() && IsJumpValid()) {
            Rigidbody rigidbody;
            if (rigidbody = GetComponent<Rigidbody>()) {
                if(rigidbody.velocity.y < 0)
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
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
    bool OnGround() {
        Vector3 bottom_center = transform.position 
            - (CapsuleCollider.height / 2 + CapsuleCollider.radius) * new Vector3(0, 1, 0);
        Vector3 top_center = transform.position 
            - (CapsuleCollider.height / 2 + CapsuleCollider.radius) * new Vector3(0, 1, 0);
        LayerMask ignore_mask = 1 << 9;
        var colliders = Physics.OverlapCapsule(bottom_center, top_center,
            CapsuleCollider.radius, ignore_mask);
        if (colliders.Length != 0) { return true; }
        else { return false; }
    }
    bool IsJumpValid() {
        if(Time.time - LastJumpTime > MinimalDeltaTime) {
            LastJumpTime = Time.time; return true;
        } else { return false; }
    }
}