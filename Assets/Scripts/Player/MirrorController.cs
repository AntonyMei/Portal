using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{
    [Header("Mirror Settings")]
    [Tooltip("The material for a non-rendering mirror")]
    public Material NonrenderingMirrorMaterial;

    // Test examples
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    /// <summary>
    /// <para> Creates a triangle that is rendered on both sides </para>
    /// </summary>
    /// <returns> The mesh of this  </returns>
    public static Mesh CreateFractureMesh(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
        Mesh frac = new Mesh();
        // Set the vertices of the mesh
        Vector3[] vertices = new Vector3[3];
        vertices[0] = vertex1;
        vertices[1] = vertex2;
        vertices[2] = vertex3;
        frac.vertices = vertices;
        // Set the two triangles of the mesh
        int[] triangles = new int[6];
        triangles[0] = 0; triangles[1] = 1;
        triangles[2] = 2; triangles[3] = 0;
        triangles[4] = 2; triangles[5] = 1;
        frac.triangles = triangles;
        // Set UV of the mesh
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++) { uvs[i] = Vector2.zero; }
        frac.uv = uvs;
        return frac;
    }

    public GameObject GenerateMirror(GameObject first_vertex, 
        GameObject second_vertex, GameObject third_vertex) {
        GameObject mirror = new GameObject();
        // Calculate vertex position
        Vector3 first_vertex_position = first_vertex.transform.position;
        Vector3 second_vertex_position = second_vertex.transform.position;
        Vector3 third_vertex_position = third_vertex.transform.position;
        Vector3 mirror_position = (first_vertex_position + second_vertex_position + third_vertex_position) / 3;
        Vector3 first_vertex_position_relative = first_vertex_position - mirror_position;
        Vector3 second_vertex_position_relative = second_vertex_position - mirror_position;
        Vector3 third_vertex_position_relative = third_vertex_position - mirror_position;
        // Set transform
        mirror.name = $"Mirror{first_vertex.name}to{second_vertex.name}to{third_vertex.name}";
        mirror.transform.position = mirror_position; 
        mirror.transform.tag = "Mirror";
        mirror.layer = 9;
        // Set renderer
        MeshFilter mesh_filter = mirror.AddComponent<MeshFilter>();
        mesh_filter.mesh = CreateFractureMesh(first_vertex_position_relative, 
             second_vertex_position_relative, third_vertex_position_relative);
        MeshRenderer mesh_renderer = mirror.AddComponent<MeshRenderer>();
        mesh_renderer.material = NonrenderingMirrorMaterial;
        // Set collider
        mirror.AddComponent<MeshCollider>();
        // Add Script
        //
        //
        //
        return mirror;
    }
    
    void Start() {
        GenerateMirror(obj1, obj2, obj3);
    }
}
