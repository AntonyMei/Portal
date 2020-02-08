using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Render Settings")]
    [Tooltip("The maxium of reflected ray")]
    public float ReflectedRayLength = 300f;
    [Tooltip("The radius of reflected ray")]
    public float ReflectedRayRadius = 1f;


    [Header("Dependencies")]
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;

    private Mirror last_mirror = null;

    void Update() {
        if(last_mirror) last_mirror.DestroyOutputRay();
        if (Input.GetKey(KeyCode.Mouse0)) {
            // Cast a virtual ray to detect
            RaycastHit hit_info = new RaycastHit();
            Ray ray = new Ray(PlayerCamera.transform.position,
                              PlayerCamera.transform.forward);
            Physics.Raycast(ray, out hit_info);
            // If the ray hits a mirror
            if(hit_info.distance != 0 && hit_info.transform.tag == "Mirror") {
                Mirror mirror_script = hit_info.transform.GetComponent<Mirror>();
                mirror_script.GenerateOutputRay(ray.direction, hit_info.point, 
                    ReflectedRayLength, ReflectedRayRadius);
                last_mirror = mirror_script;
            }
        }
    }
}
