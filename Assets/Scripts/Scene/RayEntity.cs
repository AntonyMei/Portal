using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEntity : MonoBehaviour
{
    [Tooltip("The maximum time of reflection"), HideInInspector]
    public int MaxReflectionCount = -1;
    [Tooltip("The origin of this ray"), HideInInspector]
    public Vector3 Origin;
    [Tooltip("The direction of this ray"), HideInInspector]
    public Vector3 Direction;
    [Tooltip("The maxium length of this ray"), HideInInspector]
    public float MaxLength;
    [Tooltip("The actual length of this ray"), HideInInspector]
    public float ActualLength;
    [Tooltip("The radius of this ray"), HideInInspector]
    public float Radius;

    [HideInInspector]
    public GameObject RayPrefab;
    private Vector3 last_hit_point = new Vector3();
    private GameObject next = null;

    private void Update() {
        // Detect what object this ray hits
        RaycastHit hit_info = new RaycastHit();
        Ray detection_ray = new Ray(Origin, Direction);
        Physics.Raycast(detection_ray, out hit_info, MaxLength, 1 << 11); // 1 << 11 layer 11
        if (hit_info.distance != 0 && hit_info.transform.tag == "Mirror") {
            // If the hit point changes, then refresh
            if (hit_info.point != last_hit_point) {
                GameObject.Destroy(next);
                next = GenerateNextRay(detection_ray.direction, hit_info.point, MaxLength - ActualLength,
                    Radius, hit_info.normal, MaxReflectionCount - 1);
                last_hit_point = hit_info.point;
            }
        } else {
            GameObject.Destroy(next);
            last_hit_point = new Vector3();
        }
    }

    private void OnDestroy() {
        GameObject.Destroy(next);
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// <para> Generate the output ray </para>
    /// <para> The ray is stored in output_ray </para>
    /// <para> For fracture based mirrors, must call this function to ensure a correct normal vector </para>
    /// <para> This function add RayEntity to generated rays </para>
    /// </summary>
    /// <param name="input"> The input ray's direction </param>
    /// <param name="collision_point"> The point where the input ray hits the mirror </param>
    /// <param name="max_output_length"> The maxium length of the output ray </param>
    /// <param name="output_radius"> The radius of output ray </param>
    /// <param name="normal_vector"> The normal vector of the mirror </param>
    /// <param name="maxium_reflection_count"> The maxium reflection count of the ray </param>
    public GameObject GenerateNextRay(Vector3 input, Vector3 collision_point,
        float max_output_length, float output_radius, Vector3 normal_vector,
        int maxium_reflection_count) {
        // Generate gameobject
        GameObject ray_obj = Instantiate(RayPrefab);
        ray_obj.name = $"{gameObject.name}N";
        ray_obj.tag = "Ray";
        // Calculate output direction
        Vector3 normal = Vector3.Project(input, normal_vector);
        Vector3 tangent = input - normal;
        Vector3 output_dir = tangent - normal;
        output_dir.Normalize();
        // Calculate output endpoint
        RaycastHit hit_info = new RaycastHit();
        Ray detection_ray = new Ray(collision_point, output_dir);
        Physics.Raycast(detection_ray, out hit_info);
        Vector3 end_point = new Vector3();
        float ray_length;
        if (hit_info.distance == 0) {
            ray_length = max_output_length;
            end_point = collision_point + output_dir * ray_length;
        } else {
            ray_length = Mathf.Min(max_output_length, hit_info.distance);
            end_point = collision_point + output_dir * ray_length;
        }
        // Rneder Ray
        RayRenderer renderer = ray_obj.GetComponent<RayRenderer>();
        renderer.Refresh(collision_point, end_point, output_radius);
        renderer.StartRendering();
        // Add ray entity
        RayEntity ray_entity = ray_obj.AddComponent<RayEntity>();
        ray_entity.MaxReflectionCount = maxium_reflection_count;
        ray_entity.Origin = collision_point + 0.1f * output_dir;
        ray_entity.Direction = output_dir;
        ray_entity.MaxLength = max_output_length;
        ray_entity.ActualLength = ray_length;
        ray_entity.Radius = output_radius;
        ray_entity.RayPrefab = RayPrefab;
        // Returns the ray
        return ray_obj;
    }
}
