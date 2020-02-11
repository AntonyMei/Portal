using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [Header("Mirror Settings")]
    [Tooltip("Is this mirror static. Static mirror will not refresh its normal vector automatically")]
    public bool IsStatic = false;
    [Tooltip("The prefab for ray")]
    public GameObject RayPrefab;

    // The three edges of the mirror
    [HideInInspector]
    public GameObject FirstEdge;
    [HideInInspector]
    public GameObject SecondEdge;
    [HideInInspector]
    public GameObject ThirdEdge;

    // The three vertices of the mirror
    [HideInInspector]
    public GameObject Vertex1;
    [HideInInspector]
    public GameObject Vertex2;
    [HideInInspector]
    public GameObject Vertex3;
    
    // The normal vector of the mirror
    private Vector3 normal_vector = new Vector3();
    // If the mirror has output
    private bool has_output = false;
    // The output ray of the mirror
    [SerializeField]
    private GameObject output_ray = null;

    /// <summary>
    /// <para> Refreshes the normal vector of the mirror </para>
    /// <para> For non-static mirrors, this function will be called automatically every frame </para>
    /// </summary>
    public void RefreshNormalVector() {
        normal_vector = gameObject.transform.up;
    }

    /// <summary>
    /// Only works when the mirror is static
    /// </summary>
    public void SetNormalVector(Vector3 normal) {
        if (IsStatic) normal_vector = normal;
    }

    /// <summary>
    /// Get the normal vector of the mirror
    /// </summary>
    /// <returns> The normal vector of the mirror </returns>
    public Vector3 GetNormalVector() {
        return normal_vector;
    }

    /// <summary>
    /// Returns whether the mirror has a output ray
    /// </summary>
    /// <returns></returns>
    public bool HasOutput() {
        return has_output;
    }

    /// <summary>
    /// Get the output ray
    /// </summary>
    /// <returns></returns>
    public GameObject GetOutputRay() {
        return output_ray;
    }

    /// <summary>
    /// <para> Generate the output ray </para>
    /// <para> The ray is stored in output_ray </para>
    /// </summary>
    /// <param name="input"> The input ray's direction </param>
    /// <param name="collision_point"> The point where the input ray hits the mirror </param>
    /// <param name="output_lengh"> The maxium length of the output ray </param>
    /// <param name="output_radius"> The radius of output ray </param>
    public void GenerateOutputRay(Vector3 input, Vector3 collision_point, 
        float output_lengh, float output_radius) {
        // Generate gameobject
        GameObject ray_obj = Instantiate(RayPrefab);
        ray_obj.name = $"Mirror{gameObject.name}ray";
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
        if (hit_info.distance == 0) {
            end_point = collision_point + output_dir * output_lengh;
        } else {
            end_point = collision_point + output_dir * Mathf.Min(output_lengh, hit_info.distance);
        }
        // Rneder Ray
        RayRenderer renderer = ray_obj.GetComponent<RayRenderer>();
        renderer.Refresh(collision_point, end_point, output_radius);
        renderer.StartRendering();
        // Destroy the last ray
        DestroyOutputRay();
        // Set output
        has_output = true;
        output_ray = ray_obj;
    }

    /// <summary>
    /// <para> Generate the output ray </para>
    /// <para> The ray is stored in output_ray </para>
    /// <para> For fracture based mirrors, must call this function to ensur a correct normal vector </para>
    /// </summary>
    /// <param name="input"> The input ray's direction </param>
    /// <param name="collision_point"> The point where the input ray hits the mirror </param>
    /// <param name="output_lengh"> The maxium length of the output ray </param>
    /// <param name="output_radius"> The radius of output ray </param>
    /// <param name="normal_vector"> The normal vector of the mirror </param>
    public void GenerateOutputRay(Vector3 input, Vector3 collision_point,
        float output_lengh, float output_radius, Vector3 normal_vector) {
        // Generate gameobject
        GameObject ray_obj = Instantiate(RayPrefab);
        ray_obj.name = $"Mirror{gameObject.name}ray";
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
        if (hit_info.distance == 0) {
            end_point = collision_point + output_dir * output_lengh;
        } else {
            end_point = collision_point + output_dir * Mathf.Min(output_lengh, hit_info.distance);
        }
        // Rneder Ray
        RayRenderer renderer = ray_obj.GetComponent<RayRenderer>();
        renderer.Refresh(collision_point, end_point, output_radius);
        renderer.StartRendering();
        // Destroy the last ray
        DestroyOutputRay();
        // Set output
        has_output = true;
        output_ray = ray_obj;
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
    public void GenerateOutputRay(Vector3 input, Vector3 collision_point,
        float max_output_length, float output_radius, Vector3 normal_vector,
        int maxium_reflection_count) {
        // Generate gameobject
        GameObject ray_obj = Instantiate(RayPrefab);
        ray_obj.name = $"Mirror{gameObject.name}ray";
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
        ray_entity.Origin = collision_point;
        ray_entity.Direction = output_dir;
        ray_entity.MaxLength = max_output_length;
        ray_entity.ActualLength = ray_length;
        ray_entity.Radius = output_radius;
        ray_entity.RayPrefab = RayPrefab;
        // Destroy the last ray
        DestroyOutputRay();
        // Set output
        has_output = true;
        output_ray = ray_obj;
    }

    /// <summary>
    /// Destroys the output ray
    /// </summary>
    public void DestroyOutputRay() {
        if (output_ray) {
            GameObject.Destroy(output_ray);
            output_ray = null;
            has_output = false;
        }
    }

    private void OnEnable() {
        // If the mirror is non-static, the normal vector will be refreshed when mirror enabled and in every frame
        // For static mirrors, normal vectors must be set automatically through calling SetNormalVector(Vector3)
        if (!IsStatic) RefreshNormalVector();
    }

    private void Update() {
        // If the mirror is non-static, the normal vector will be refreshed when mirror enabled and in every frame
        // For static mirrors, normal vectors must be set automatically through calling SetNormalVector(Vector3)
        if (!IsStatic) RefreshNormalVector();
    }

    private void OnDestroy() {
        // Destroys the output ray
        DestroyOutputRay();
    }
}
