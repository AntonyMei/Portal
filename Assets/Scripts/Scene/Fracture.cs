using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    [Tooltip("Duration of the fracture(Second)")]
    public float Duration;
    
    private float start_time;
    void OnEnable() {
        start_time = Time.time;
    }
    void Update() {
        if (Time.time - start_time >= Duration) GameObject.Destroy(gameObject); 
    }
}
