using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [Header("Anchor Settings")]
    [Tooltip("The material when anchor is not active")]
    public Material NormalAnchorMaterial;
    [Tooltip("The material when anchor is active")]
    public Material ActiveAnchorMaterial;

    [Header("Ray Settings")]
    [Tooltip("The material attached to rays")]
    public Material RayMaterial;
    [Tooltip("The mesh of the ray")]
    public Mesh RayMesh;
    [Tooltip("The radius of the ray")]
    public float Radius = 0.1f;

    [Header("Cursor settings"), Range(0f, 100f)]
    [Tooltip("The sensitivity of aided aiming")]
    public float AidedAimingSensitivity = 20f;

    [Header("Dependencies")]
    [Tooltip("The ray controller attached to the player")]
    public RayController RayController;
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;

    private bool is_first_anchor_set = false;
    private GameObject first_anchor = null;

    private Stack<GameObject> ray_list = new Stack<GameObject>();
    private GameObject ray_root;

    void Start() {
        ray_root = new GameObject("RayRoot");
    }
    void Update() {
        // Cast a virtual ray to detect
        RaycastHit hit_info;
        Ray ray = new Ray(PlayerCamera.transform.position,
                          PlayerCamera.transform.forward);
        Physics.Raycast(ray, out hit_info);
        // React to clicks
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (!is_first_anchor_set) {
                if (hit_info.distance != 0 && hit_info.transform.tag == "Anchor") {
                    MeshRenderer renderer = hit_info.transform.GetComponent<MeshRenderer>();
                    renderer.material = ActiveAnchorMaterial;
                    is_first_anchor_set = true;
                    first_anchor = hit_info.transform.gameObject;
                }
            } else {
                if (hit_info.distance != 0 && hit_info.transform.tag == "Anchor") {
                    // Change color
                    MeshRenderer renderer = hit_info.transform.GetComponent<MeshRenderer>();
                    renderer.material = ActiveAnchorMaterial;
                    // Generate ray
                    GameObject ray_obj = new GameObject();
                    ray_obj.name = $"{first_anchor.name}to{hit_info.transform.name}";
                    ray_obj.transform.parent = ray_root.transform;
                    // Render ray
                    RayRenderer ray_renderer = ray_obj.AddComponent<RayRenderer>();
                    ray_renderer.Radius = Radius;
                    ray_renderer.SetRenderer(RayMaterial, RayMesh);
                    ray_renderer.RenderWithGameObjects(first_anchor, hit_info.transform.gameObject);
                    ray_renderer.StartRendering();
                    ray_list.Push(ray_obj);
                    is_first_anchor_set = false;
                    first_anchor = null;
                } else {
                    MeshRenderer renderer = first_anchor.GetComponent<MeshRenderer>();
                    renderer.material = NormalAnchorMaterial;
                    is_first_anchor_set = false;
                    first_anchor = null;
                }
            }
        }
    }
}
