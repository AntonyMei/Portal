using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour
{
    [Header("Mirror Settings")]
    [Tooltip("The material for a non-camera mirror")]
    public Material NonCameraMirrorMaterial;
    [Tooltip("The camera prefab for a camera-based mirror")]
    public GameObject CameraPrefab;
    [Tooltip("The resolution of camera-based mirror "), Range(256, 1024)]
    public int XResolution = 1024;
    [Tooltip("The resolution of camera-based mirror "), Range(256, 1024)]
    public int YResolution = 1024;

    [Header("Ray Settings")]
    [Tooltip("The prefab of ray")]
    public GameObject RayPrefab;
    [Tooltip("The material of active ray")]
    public Material ActiveRayMaterial;
    [Tooltip("The material of non-active ray")]
    public Material NonactiveRayMaterial;

    [Header("Dependencies")]
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;
    
    [Tooltip("The mirror's root"), HideInInspector]
    public GameObject MirrorRoot;
    [Tooltip("The stack that stores all the generated mirrors"), HideInInspector]
    public Stack<GameObject> MirrorStack = new Stack<GameObject>();
    // The first and second edge
    private GameObject first_edge = null;
    private GameObject second_edge = null;

    void Start() {
        MirrorRoot = new GameObject("MirrorRoot");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit hit_info = new RaycastHit();
            Ray ray = new Ray(PlayerCamera.transform.position,
                              PlayerCamera.transform.forward);
            Physics.Raycast(ray, out hit_info);
            if (hit_info.distance != 0 && hit_info.transform.tag == "Ray") {
                if(!first_edge) {
                    // Set the first edge
                    first_edge = hit_info.transform.gameObject;
                    ConnectionRay connection_ray_script = 
                        first_edge.GetComponent<ConnectionRay>();
                    connection_ray_script.SetMaterial2Active();
                } else if (!second_edge) {
                    if (hit_info.transform.name != first_edge.name) {
                        // Set the second edge
                        second_edge = hit_info.transform.gameObject;
                        ConnectionRay connection_ray_script =
                            second_edge.GetComponent<ConnectionRay>();
                        connection_ray_script.SetMaterial2Active();
                    }
                } else {
                    if (hit_info.transform.name != first_edge.name 
                        && hit_info.transform.name != second_edge.name) {
                        Debug.Assert(first_edge && second_edge);
                        // Check if the three rays can form a triangle
                        RayRenderer ray_renderer_1 = first_edge.GetComponent<RayRenderer>();
                        RayRenderer ray_renderer_2 = second_edge.GetComponent<RayRenderer>();
                        RayRenderer ray_renderer_3 = hit_info.transform.GetComponent<RayRenderer>();
                        List<GameObject> anchor_list = new List<GameObject>();
                        anchor_list.Add(ray_renderer_1.Start);
                        anchor_list.Add(ray_renderer_1.End);
                        if (!anchor_list.Contains(ray_renderer_2.Start))
                            anchor_list.Add(ray_renderer_2.Start);
                        if (!anchor_list.Contains(ray_renderer_2.End))
                            anchor_list.Add(ray_renderer_2.End);
                        if (!anchor_list.Contains(ray_renderer_3.Start))
                            anchor_list.Add(ray_renderer_3.Start);
                        if (!anchor_list.Contains(ray_renderer_3.End))
                            anchor_list.Add(ray_renderer_3.End);
                        if (anchor_list.Count == 3) {
                            // Check if the mimrror has been generated
                            string name1 = $"Mirror{anchor_list[0].name}to{anchor_list[1].name}to{anchor_list[2].name}";
                            string name2 = $"Mirror{anchor_list[0].name}to{anchor_list[2].name}to{anchor_list[1].name}";
                            string name3 = $"Mirror{anchor_list[1].name}to{anchor_list[0].name}to{anchor_list[2].name}";
                            string name4 = $"Mirror{anchor_list[1].name}to{anchor_list[2].name}to{anchor_list[0].name}";
                            string name5 = $"Mirror{anchor_list[2].name}to{anchor_list[0].name}to{anchor_list[1].name}";
                            string name6 = $"Mirror{anchor_list[2].name}to{anchor_list[1].name}to{anchor_list[0].name}";
                            ConnectionRay ray_script_1 = first_edge.GetComponent<ConnectionRay>();
                            ConnectionRay ray_script_2 = second_edge.GetComponent<ConnectionRay>();
                            ConnectionRay ray_script_3 = hit_info.transform.GetComponent<ConnectionRay>();
                            if (ray_script_1.MirrorList.Contains(name1) || ray_script_1.MirrorList.Contains(name2)
                             || ray_script_1.MirrorList.Contains(name3) || ray_script_1.MirrorList.Contains(name4)
                             || ray_script_1.MirrorList.Contains(name5) || ray_script_1.MirrorList.Contains(name6)) {
                                // If first or second edge is selected, cancel the selection
                                if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                    first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                                }
                                first_edge = null;
                                if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                    second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();                                   
                                } 
                                second_edge = null;
                            } else {
                                // Generate mirror
                                // Set the last ray to active
                                ray_script_3.SetMaterial2Active();
                                // Generate mirror
                                GameObject mirror_obj = GenerateMirror(anchor_list[0], anchor_list[1],
                                                                       anchor_list[2], false);
                                mirror_obj.transform.parent = MirrorRoot.transform;
                                // Set MeshCollider to convex
                                //MeshCollider mesh_collider = mirror_obj.GetComponent<MeshCollider>();
                                //mesh_collider.convex = true;
                                // Add mirror to rays' mirror list
                                ray_script_1.MirrorList.Add(mirror_obj.name);
                                ray_script_2.MirrorList.Add(mirror_obj.name);
                                ray_script_3.MirrorList.Add(mirror_obj.name);
                                // Add rays to mirror's ray list
                                Mirror mirror_script = mirror_obj.GetComponent<Mirror>();
                                mirror_script.FirstEdge = first_edge;
                                mirror_script.SecondEdge = second_edge;
                                mirror_script.ThirdEdge = hit_info.transform.gameObject;
                                // Add the mirror to stack
                                MirrorStack.Push(mirror_obj);
                                // Reset to original
                                first_edge = null;
                                second_edge = null;
                            }
                        } else {
                            // If first or second edge is selected, cancel the selection
                            if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();                               
                            }
                            first_edge = null;
                            if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                            }
                            second_edge = null;
                        }
                    }
                }                
            } else {
                // If first or second edge is selected, cancel the selection
                if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                }
                first_edge = null;
                if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                }
                second_edge = null;
            }
        }
        // Q is the key to delete last mirror
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (!first_edge && !second_edge && MirrorStack.Count != 0) {
                // Get last added mirror
                GameObject last_mirror = MirrorStack.Pop();
                // Delete the mirror from its three edges
                Mirror mirror_script = last_mirror.GetComponent<Mirror>();
                ConnectionRay edge1_script = mirror_script.FirstEdge.GetComponent<ConnectionRay>();
                ConnectionRay edge2_script = mirror_script.SecondEdge.GetComponent<ConnectionRay>();
                ConnectionRay edge3_script = mirror_script.ThirdEdge.GetComponent<ConnectionRay>();
                edge1_script.MirrorList.Remove(last_mirror.name);
                edge2_script.MirrorList.Remove(last_mirror.name);
                edge3_script.MirrorList.Remove(last_mirror.name);
                // Reset renderings
                if (edge1_script.MirrorList.Count != 0) {
                    mirror_script.FirstEdge.GetComponent<MeshRenderer>().material = ActiveRayMaterial;
                } else {
                    mirror_script.FirstEdge.GetComponent<MeshRenderer>().material = NonactiveRayMaterial;
                }
                if (edge2_script.MirrorList.Count != 0) {
                    mirror_script.SecondEdge.GetComponent<MeshRenderer>().material = ActiveRayMaterial;
                } else {
                    mirror_script.SecondEdge.GetComponent<MeshRenderer>().material = NonactiveRayMaterial;
                }
                if (edge3_script.MirrorList.Count != 0) {
                    mirror_script.ThirdEdge.GetComponent<MeshRenderer>().material = ActiveRayMaterial;
                } else {
                    mirror_script.ThirdEdge.GetComponent<MeshRenderer>().material = NonactiveRayMaterial;
                }
                // Destroy mirror and its ray
                GameObject.Destroy(last_mirror);
            } else {
                // If first or second edge is selected, cancel the selection
                if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                }
                first_edge = null;
                if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                }
                second_edge = null;
            }
        }
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

    /// <summary>
    /// Generate a mirror that is rendered using a full-reflective material
    /// </summary>
    /// <param name="first_vertex"> The first vertex of the mirror </param>
    /// <param name="second_vertex"> The second vertex of the mirror </param>
    /// <param name="third_vertex"> The third vertex of the mirror </param>
    /// <param name="is_static"> 
    /// Is the mirror static, non-static mirrors will be attached MirrorUpdateMesh 
    /// that automatically refreshed its mesh every frame 
    /// </param>
    /// <returns></returns>
    public GameObject GenerateMirror(GameObject first_vertex, 
        GameObject second_vertex, GameObject third_vertex, bool is_static) {
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
        mesh_renderer.material = NonCameraMirrorMaterial;
        // Set collider
        mirror.AddComponent<MeshCollider>();
        // Add Script
        Mirror mirror_script = mirror.AddComponent<Mirror>();
        mirror_script.IsStatic = is_static;
        mirror_script.RayPrefab = RayPrefab;
        mirror_script.Vertex1 = first_vertex;
        mirror_script.Vertex2 = second_vertex;
        mirror_script.Vertex3 = third_vertex;
        // Add auto update if necessary
        if (!is_static) {
            MirrorUpdateMesh mesh_updater =  mirror.AddComponent<MirrorUpdateMesh>();
            mesh_updater.Vertex1 = first_vertex;
            mesh_updater.Vertex2 = second_vertex;
            mesh_updater.Vertex3 = third_vertex;
        }
        return mirror;
    }

    
    /// <summary>
    /// Generate a mirror that is rendered using a full-reflective material
    /// </summary>
    /// <param name="first_vertex"> The first vertex of the mirror </param>
    /// <param name="second_vertex"> The second vertex of the mirror </param>
    /// <param name="third_vertex"> The third vertex of the mirror </param>
    /// <returns></returns>
    public GameObject GenerateCameraBasedMirror(GameObject first_vertex,
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
        // Add camera scripts
        GameObject camera_obj = Instantiate(CameraPrefab);
        camera_obj.transform.parent = mirror.transform;
        MirrorRenderer mirror_renderer = camera_obj.GetComponent<MirrorRenderer>();
        mirror_renderer.mirrorPlane = mirror;
        MirrorManager mirror_manager = camera_obj.GetComponent<MirrorManager>();
        mirror_manager.mirrorPlane = mirror;
        mirror_manager.mainCamera = PlayerCamera;
        // Set render texture
        RenderTextureDescriptor descriptor = 
            new RenderTextureDescriptor(XResolution, YResolution);
        RenderTexture render_texture = new RenderTexture(descriptor);
        camera_obj.GetComponent<Camera>().targetTexture = render_texture;
        // Creat material
        Material camera_based_mirror_material = 
            new Material(Shader.Find("Unlit/Texture"));
        camera_based_mirror_material.mainTexture = render_texture;
        camera_based_mirror_material.mainTextureOffset = new Vector2(1, 0);
        camera_based_mirror_material.mainTextureScale = new Vector2(-1, 1);
        // Set renderer
        MeshFilter mesh_filter = mirror.AddComponent<MeshFilter>();
        mesh_filter.mesh = CreateFractureMesh(first_vertex_position_relative,
             second_vertex_position_relative, third_vertex_position_relative);
        MeshRenderer mesh_renderer = mirror.AddComponent<MeshRenderer>();
        mesh_renderer.material = camera_based_mirror_material;
        // Set collider
        mirror.AddComponent<MeshCollider>();
        // Add mirror script
        Mirror mirror_script = mirror.AddComponent<Mirror>();
        mirror_script.IsStatic = false;
        mirror_script.RayPrefab = RayPrefab;
        return mirror;
    }

}
