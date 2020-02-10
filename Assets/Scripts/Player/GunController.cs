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
    [Tooltip("The maximum time of reflection"), HideInInspector]
    public int MaxReflectionCount = 10;

    [Header("Dependencies")]
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;

    // The mirror that
    private Mirror last_mirror = null;
    private Vector3 LastHitPoint = new Vector3();

    void Update() {
        // Check for interactions
        if (Input.GetKey(KeyCode.Mouse0)) {
            // Cast a virtual ray to detect
            RaycastHit hit_info = new RaycastHit();
            Ray ray = new Ray(PlayerCamera.transform.position,
                              PlayerCamera.transform.forward);
            Physics.Raycast(ray, out hit_info);
            // If the ray hits a mirror
            if(hit_info.distance != 0 && hit_info.transform.tag == "Mirror") {
                // If the hit point changes, then refresh
                if (hit_info.point !=  LastHitPoint) {
                    Mirror mirror_script = hit_info.transform.GetComponent<Mirror>();
                    mirror_script.GenerateOutputRay(ray.direction, hit_info.point,
                                            ReflectedRayLength, ReflectedRayRadius,
                                            hit_info.normal, MaxReflectionCount);
                    LastHitPoint = hit_info.point;
                    last_mirror = mirror_script;
                }
            } else {
                // Destroys the ray in the last frame
                if (last_mirror) {
                    last_mirror.DestroyOutputRay();
                    last_mirror = null;
                }
            }
        } else {
            // Destroys the ray in the last frame
            if (last_mirror) { 
                last_mirror.DestroyOutputRay();
                last_mirror = null;
            }
        }
    }
}
