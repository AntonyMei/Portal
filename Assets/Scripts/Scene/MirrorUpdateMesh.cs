using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorUpdateMesh : MonoBehaviour
{
    // The three vertices of the mirror
    public GameObject Vertex1;
    public GameObject Vertex2;
    public GameObject Vertex3;

    void Update() {
        Vector3 first_vertex_position = Vertex1.transform.position;
        Vector3 second_vertex_position = Vertex2.transform.position;
        Vector3 third_vertex_position = Vertex3.transform.position;
        Vector3 mirror_position = (first_vertex_position + second_vertex_position + third_vertex_position) / 3;
        Vector3 first_vertex_position_relative = first_vertex_position - mirror_position;
        Vector3 second_vertex_position_relative = second_vertex_position - mirror_position;
        Vector3 third_vertex_position_relative = third_vertex_position - mirror_position;
        // Change mesh
        GetComponent<MeshFilter>().mesh = CreateFractureMesh(first_vertex_position_relative,
        second_vertex_position_relative, third_vertex_position_relative);
        // Change collider's shared mesh
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
        // Change position
        transform.position = mirror_position;
    }

    /// <summary>
    /// <para> Creates a triangle that is rendered on both sides </para>
    /// </summary>
    /// <returns> The mesh of this fracture </returns>
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
}
