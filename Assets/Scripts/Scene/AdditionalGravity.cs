using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalGravity : MonoBehaviour {

    [Header("Gravity Settings")]
    [Tooltip("Add additional gravity to the player")]
    public Rigidbody Player;
    [Tooltip("The magnitude of additional gravity")]
    public float Magnitude = 100f;

    void Update() {
        Player.AddForce(new Vector3(0, -Magnitude, 0));
    }
}