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

    [Header("Amplifier Settings")]
    [Tooltip("The maximum length of amplified rays (per anchor)")]
    public float AmplifierRayLengthPerAnchor = 300f;
    [Tooltip("The radius of amplified rays (per anchor)")]
    public float AmplifiedRayRadiusPerAnchor = 0.2f;
    [Tooltip("The maxium reflection count (per anchor)")]
    public int AmplifiedRayReflectionCountPerAnchor = 10;

    [Header("Dependencies")]
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;
    [Tooltip("The prefab of ray")]
    public GameObject RayPrefab;

    // The mirror that
    private Mirror last_mirror = null;
    private Vector3 last_hit_point = new Vector3();
    // The last amplifier ray
    private GameObject last_amplifier_ray = null;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (last_amplifier_ray) GameObject.Destroy(last_amplifier_ray);
        }
        // Check for interactions
        if (Input.GetKey(KeyCode.Mouse0)) {
            // Cast a virtual ray to detect
            RaycastHit hit_info = new RaycastHit();
            Ray ray = new Ray(PlayerCamera.transform.position,
                              PlayerCamera.transform.forward);
            Physics.Raycast(ray, out hit_info);
            // If the ray hits a mirror
            if (hit_info.distance != 0 && hit_info.transform.tag == "Mirror") {
                // If mirror changes, delete the last ray
                if (last_mirror && hit_info.transform != last_mirror.transform) {
                    // Destroys the ray in the last frame
                    if (last_mirror) {
                        last_mirror.DestroyOutputRay();
                        last_mirror = null;
                    }
                }
                // If the hit point changes, then refresh
                if (hit_info.point !=  last_hit_point) {
                    Mirror mirror_script = hit_info.transform.GetComponent<Mirror>();
                    mirror_script.GenerateOutputRay(ray.direction, hit_info.point,
                                            ReflectedRayLength, ReflectedRayRadius,
                                            hit_info.normal, MaxReflectionCount);
                    last_hit_point = hit_info.point;
                    last_mirror = mirror_script;
                }
            } else {
                // Destroys the ray in the last frame
                if (last_mirror) {
                    last_mirror.DestroyOutputRay();
                    last_mirror = null;
                }
            }
            // If the ray hit an anchor
            if (hit_info.distance != 0 && hit_info.transform.tag == "Anchor") {
                AnchorGroup anchor_group = hit_info.transform.parent.GetComponent<AnchorGroup>();
                Vector3 mass_center =
                    anchor_group.GetMassCenter(hit_info.transform.GetSiblingIndex(), out int anchor_count);
                if (anchor_count != 1) {
                    if (last_amplifier_ray) GameObject.Destroy(last_amplifier_ray);
                    GameObject amplifier_ray = Instantiate(RayPrefab);
                    amplifier_ray.name = "AmplifierRay";
                    RayRenderer ray_renderer = amplifier_ray.GetComponent<RayRenderer>();
                    // Calculate start and direction
                    Vector3 start = hit_info.transform.position;
                    Vector3 direction = mass_center - start;
                    direction.Normalize();
                    // Calculate end
                    RaycastHit hit_info_2 = new RaycastHit();
                    Ray detection_ray = new Ray(start, direction);
                    float max_length = AmplifierRayLengthPerAnchor * anchor_count;
                    Physics.Raycast(detection_ray, out hit_info_2, max_length, 1 << 11); // 1 << 11 layer 11
                    Vector3 end;
                    float actual_ray_length;
                    if (hit_info_2.distance == 0) {
                        actual_ray_length = max_length;
                        end = start + actual_ray_length * direction;
                    } else {
                        actual_ray_length = Mathf.Min(hit_info_2.distance, max_length);
                        end = start + actual_ray_length * direction;
                    }
                    float radius = AmplifiedRayRadiusPerAnchor * anchor_count;
                    // Rendering
                    ray_renderer.Refresh(start, end, radius);
                    ray_renderer.StartRendering();
                    // Add script
                    RayEntity ray_entity = amplifier_ray.AddComponent<RayEntity>();
                    ray_entity.MaxReflectionCount = AmplifiedRayReflectionCountPerAnchor * anchor_count;
                    ray_entity.Origin = start;
                    ray_entity.Direction = direction;
                    ray_entity.MaxLength = max_length;
                    ray_entity.ActualLength = actual_ray_length;
                    ray_entity.RayPrefab = RayPrefab;
                    ray_entity.Radius = radius;
                    // Save
                    last_amplifier_ray = amplifier_ray;
                } 
            }
            // Detect whether the ray points at a portal
            RaycastHit portal_detection_info = new RaycastHit();
            Ray portal_detection_ray = new Ray(PlayerCamera.transform.position,
                              PlayerCamera.transform.forward);
            Physics.Raycast(portal_detection_ray,
                out portal_detection_info, 100f, 1 << 12);
            if (portal_detection_info.distance != 0 &&
                portal_detection_info.transform.tag == "Portal")
            {
                portal_detection_info.transform.GetComponent<PortalActivator>().ActivatePortal(0.01);
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
