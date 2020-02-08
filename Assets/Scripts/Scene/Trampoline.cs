using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script for GameObject Trampoline
/// </summary>
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

    // The normal vector of the trampoline
    [HideInInspector]
    public Vector3 NormalVector = new Vector3();

    /// <summary>
    /// If the player enters the collider surrounding the trampoline, change its 
    /// physic material to full bounciness, so that the trampoline can totally reverse
    /// the player's velocity normal to the trampoline
    /// </summary>
    public void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Player") { collider.material = TrampolinePhysicMaterial; }
    }

    /// <summary>
    /// When the player leaves the trampoline, change its physic material back to normal.
    /// </summary>
    public void OnTriggerExit(Collider collider) {
        if (collider.tag == "Player") { collider.material = NormalPhysicMaterial; }
    }

    /// <summary>
    /// When the player actually hits the trampoline, add a force to the player depending on
    /// its velocity normal to the trampoline
    /// </summary>
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 NormalVelocity = Vector3.Project(rigidbody.velocity, NormalVector);
            if(NormalVelocity.magnitude >= MinimalNormalVelocity) {
                rigidbody.AddForce(NormalVector * Magnitude, ForceMode);
            }
        }
    }

    /// <summary>
    /// <para> Refreshes the normal vector of the trampoline. </para>
    /// <para> For non-static trampolines, this function will be called automatically when trampoline is
    /// created and in every frame. </para>
    /// <para> For static trampolines, this function must be called manually before it can work properly </para>
    /// </summary>
    public void  RefreshNormalVector() { 
        NormalVector = gameObject.transform.up;
        NormalVector.Normalize();
    }
    
    /// <summary>
    /// <para> If the trampoline is not static, refresh its normal vector </para>
    /// <para> Static trampolines should call RefreshNormalVector manually </para>
    /// </summary>
    void OnEnable() { if(!isStatic) RefreshNormalVector(); }

    /// <summary>
    /// If the trampoline is not static, refresh its normal vector
    /// Static trampolines should call RefreshNormalVector manually
    /// </summary>
    void Update() { if (!isStatic) RefreshNormalVector(); }
}
