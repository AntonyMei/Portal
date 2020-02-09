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
                        // Check if the mimrror has been generated
                        //
                        //
                        //
                        if (anchor_list.Count == 3) {
                            // Generate mirror
                            // Get script
                            ConnectionRay ray_script_1 = first_edge.GetComponent<ConnectionRay>();
                            ConnectionRay ray_script_2 = second_edge.GetComponent<ConnectionRay>();
                            ConnectionRay ray_script_3 = hit_info.transform.GetComponent<ConnectionRay>();
                            ray_script_3.SetMaterial2Active();
                            // Generate mirror
                            GameObject mirror_obj = GenerateMirror(anchor_list[0], anchor_list[1],
                                                                   anchor_list[2], false);
                            mirror_obj.transform.parent = MirrorRoot.transform;
                            // Add mirror to rays' mirror list
                            ray_script_1.MirrorList.Add(mirror_obj.name);
                            ray_script_2.MirrorList.Add(mirror_obj.name);
                            ray_script_3.MirrorList.Add(mirror_obj.name);
                            // Reset to original
                            first_edge = null;
                            second_edge = null;
                        } else {
                            if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                                first_edge = null;
                            }
                            if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                                second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                                second_edge = null;
                            }
                        }
                    }
                }                
            } else {
                if (first_edge && first_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    first_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                    first_edge = null;
                }
                if (second_edge && second_edge.GetComponent<ConnectionRay>().MirrorList.Count == 0) {
                    second_edge.GetComponent<ConnectionRay>().SetMaterial2Nonactive();
                    second_edge = null;
                }
            }
        } 
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
