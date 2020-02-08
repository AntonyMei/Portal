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
    
    private Vector3 normal_vector = new Vector3();
    private bool has_output = false;
    private GameObject output_ray = null;

    public void RefreshNormalVector() {
        normal_vector = gameObject.transform.up;
    }
    /// <summary>
    /// Only works when the mirror is static
    /// </summary>
    public void SetNormalVector(Vector3 normal) {
        if (IsStatic) normal_vector = normal;
    }
    public Vector3 GetNormalVector() {
        return normal_vector;
    }
    public bool HasInput() {
        return has_output;
    }
    /// <returns>Output ray or null</returns>
    public GameObject GetOutputRay() {
        return output_ray;
    }
    public void GenerateOutputRay(Vector3 input, Vector3 collision_point, 
        float output_lengh, float output_radius) {
        // Generate gameobject
        GameObject ray_obj = Instantiate(RayPrefab);
        ray_obj.name = $"Mirror{gameObject.name}ray";
        ray_obj.transform.parent = gameObject.transform;
        ray_obj.tag = "Ray";
        // Calculate output direction
        Vector3 normal = Vector3.Project(input, normal_vector);
        Vector3 tangent = input - normal;
        Vector3 output_dir = tangent - normal;
        output_dir.Normalize();
        // Calculate output endpoint
        Vector3 end_point = collision_point + output_lengh * output_dir;
        // Rneder Ray
        RayRenderer renderer = ray_obj.GetComponent<RayRenderer>();
        renderer.Refresh(collision_point, end_point, output_radius);
        renderer.StartRendering();
        // Set output
        has_output = true;
        output_ray = ray_obj;
    }
    public void DestroyOutputRay() {
        if (output_ray) {
            GameObject.Destroy(output_ray);
            output_ray = null;
            has_output = false;
        }
    }

    private void OnEnable() {
        if (!IsStatic) RefreshNormalVector();
    }
    private void Update() {
        if (!IsStatic) RefreshNormalVector();
    }
}
