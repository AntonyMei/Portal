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
        // Calculate normal vector
        Vector3 dir1 = vertex2 - vertex1;
        Vector3 dir2 = vertex3 - vertex1;
        Vector3 normal = Vector3.Cross(dir1, dir2);
        normal.Normalize();
        // Set the vertices of the mesh
        Vector3[] vertices = new Vector3[6];
        vertices[0] = vertex1 + normal * 0.01f;
        vertices[1] = vertex1 - normal * 0.01f;
        vertices[2] = vertex2 + normal * 0.01f;
        vertices[3] = vertex2 - normal * 0.01f;
        vertices[4] = vertex3 + normal * 0.01f;
        vertices[5] = vertex3 - normal * 0.01f;
        frac.vertices = vertices;
        // Set the two triangles of the mesh
        int[] triangles = new int[24];
        triangles[0] = 0; triangles[1] = 2; triangles[2] = 4; // Up
        triangles[3] = 1; triangles[4] = 5; triangles[5] = 3; // Down
        triangles[6] = 0; triangles[7] = 1; triangles[8] = 3; // Side1
        triangles[9] = 0; triangles[10] = 2; triangles[11] = 3; // Side1
        triangles[12] = 2; triangles[13] = 3; triangles[14] = 5; // Side2
        triangles[15] = 2; triangles[16] = 4; triangles[17] = 5; // Side2
        triangles[18] = 0; triangles[19] = 1; triangles[20] = 5; // Side3
        triangles[21] = 0; triangles[22] = 4; triangles[23] = 5; // Side3
        frac.triangles = triangles;
        // Set UV of the mesh
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++) { uvs[i] = Vector2.zero; }
        frac.uv = uvs;
        return frac;
    }
}
