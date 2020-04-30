using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeCameraDirection : MonoBehaviour
{
    public GameObject Target;
    void Start() {
        transform.LookAt(Target.transform);
        GetComponent<CameraController>().enabled = true;
    } 
}
