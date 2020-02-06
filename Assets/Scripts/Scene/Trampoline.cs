using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Trampoline Settings")]
    [Tooltip("Is the trampoline static. Static trampolines will not " +
             "refresh its normal vector automatically")]
    public bool isStatic = true;
    [Tooltip("The physic material of full bounciness")]
    public PhysicMaterial TrampolinePhysicMaterial;
    [Tooltip("Normal physic material")]
    public PhysicMaterial NormalPhysicMaterial;

    [Header("Force settings")]
    [Tooltip("The magnitude of the force added by the trampoline")]
    public float Magnitude = 100f;
    [Tooltip("The type of force added by the trampoline")]
    public ForceMode ForceMode = ForceMode.Impulse;
    [Tooltip("The minimum speed normal to the trampoline " +
        "to trigger an additional force")]
    public float MinimalNormalVelocity  = 5f;

    [HideInInspector]
    public Vector3 NormalVector = new Vector3();

    void OnEnable() { if(!isStatic) RefreshNormalVector(); }

    void Update() { if (!isStatic) RefreshNormalVector(); }
    public void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Player") { collider.material = TrampolinePhysicMaterial; }
    }
    public void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player") { collider.material = NormalPhysicMaterial; }
    }
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 NormalVelocity = Vector3.Project(rigidbody.velocity, NormalVector);
            if(NormalVelocity.magnitude >= MinimalNormalVelocity) {
                rigidbody.AddForce(NormalVector * Magnitude, ForceMode);
            }
        }
    }
    public void  RefreshNormalVector() { 
        NormalVector = gameObject.transform.up;
        NormalVector.Normalize();
    }
}
