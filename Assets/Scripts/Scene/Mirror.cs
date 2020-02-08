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
        Vector3 end_point = collision_point + output_lengh * output_dir;
        // Rneder Ray
        RayRenderer renderer = ray_obj.GetComponent<RayRenderer>();
        renderer.Refresh(collision_point, end_point, output_radius);
        renderer.StartRendering();
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
}
