using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [Header("Anchor Settings")]
    [Tooltip("The material when anchor is not active")]
    public Material NonactiveAnchorMaterial;
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

    public bool IsFirstAnchorSet() {
        return is_first_anchor_set;
    }
    /// <returns> The first anchor or null </returns>
    public GameObject GetFirstAnchor() {
        return first_anchor;
    }

    void Start() {
        ray_root = new GameObject("RayRoot");
    }
    void Update() {
        // Cast a virtual ray to detect
        RaycastHit hit_info;
        Ray ray = new Ray(PlayerCamera.transform.position,
                          PlayerCamera.transform.forward);
        Physics.Raycast(ray, out hit_info);
        // React to clicks, it adds a ray
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (!is_first_anchor_set) {
                if (hit_info.distance != 0 && hit_info.transform.tag == "Anchor") {
                    hit_info.transform.GetComponent<Anchor>().SetMaterial2Active();
                    is_first_anchor_set = true;
                    first_anchor = hit_info.transform.gameObject;
                }
            } else {
                if (hit_info.distance != 0 && hit_info.transform.tag == "Anchor" 
                    && hit_info.transform != first_anchor.transform) {
                    Anchor second_anchor_script = hit_info.transform.GetComponent<Anchor>();
                    Anchor first_anchor_script = first_anchor.GetComponent<Anchor>();
                    string ray_id_1 = $"{first_anchor.name}to{hit_info.transform.name}";
                    string ray_id_2 = $"{hit_info.transform.name}to{first_anchor.name}";
                    if (!second_anchor_script.RayList.Contains(ray_id_1)
                        && !first_anchor_script.RayList.Contains(ray_id_1)
                        && !second_anchor_script.RayList.Contains(ray_id_2)
                        && !first_anchor_script.RayList.Contains(ray_id_2)){
                        // Change color
                        hit_info.transform.GetComponent<Anchor>().SetMaterial2Active();
                        first_anchor.transform.GetComponent<Anchor>().SetMaterial2Active();
                        // Generate ray
                        GameObject ray_obj = new GameObject();
                        ray_obj.name = ray_id_1; ray_obj.tag = "Ray";
                        ray_obj.transform.parent = ray_root.transform;
                        // Render ray
                        RayRenderer ray_renderer = ray_obj.AddComponent<RayRenderer>();
                        ray_renderer.Radius = Radius;
                        ray_renderer.SetRenderer(RayMaterial, RayMesh);
                        ray_renderer.SetRendererWithGameObjects(first_anchor, hit_info.transform.gameObject);
                        ray_renderer.StartRendering();
                        // Add ray to lists
                        ray_list.Push(ray_obj);
                        first_anchor_script.RayList.Add(ray_id_1);
                        second_anchor_script.RayList.Add(ray_id_1);
                        is_first_anchor_set = false;
                        first_anchor = null;
                    }
                } else {
                    Anchor anchor = first_anchor.GetComponent<Anchor>();
                    if (anchor.RayList.Count == 0) anchor.SetMaterial2Nonactive(); //
                    is_first_anchor_set = false;
                    first_anchor = null;
                }
            }
        }
        // React to E, it removes the last ray
        if (Input.GetKeyDown(KeyCode.E) && ray_list.Count != 0) {
            if (!is_first_anchor_set) {
                // Remove ray from lists
                GameObject ray_obj = ray_list.Pop();
                RayRenderer ray_renderer = ray_obj.GetComponent<RayRenderer>();
                Anchor first_anchor_script = ray_renderer.Start.GetComponent<Anchor>();
                Anchor second_anchor_script = ray_renderer.End.GetComponent<Anchor>();
                string ray_id_1 = $"{ray_renderer.Start.name}to{ray_renderer.End.name}";
                string ray_id_2 = $"{ray_renderer.End.name}to{ray_renderer.Start.name}";
                if (first_anchor_script.RayList.Contains(ray_id_1)) {
                    first_anchor_script.RayList.Remove(ray_id_1);
                }
                if (first_anchor_script.RayList.Contains(ray_id_2)) {
                    first_anchor_script.RayList.Remove(ray_id_2);
                }
                if (second_anchor_script.RayList.Contains(ray_id_1)) {
                    second_anchor_script.RayList.Remove(ray_id_1);
                }
                if (second_anchor_script.RayList.Contains(ray_id_2)) {
                    second_anchor_script.RayList.Remove(ray_id_2);
                }
                // Reset renderings
                if (first_anchor_script.RayList.Count == 0) first_anchor_script.SetMaterial2Nonactive();
                if (second_anchor_script.RayList.Count == 0) second_anchor_script.SetMaterial2Nonactive();
                GameObject.Destroy(ray_obj);
            } else {
                Anchor anchor = first_anchor.GetComponent<Anchor>();
                if (anchor.RayList.Count == 0) anchor.SetMaterial2Nonactive();
                is_first_anchor_set = false;
                first_anchor = null;
            }
        }
    }
}
