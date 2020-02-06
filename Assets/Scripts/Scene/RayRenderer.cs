using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayRenderer : MonoBehaviour {
    [Header("Render Settings")]
    [Tooltip("The radius of the ray")]
    public float Radius = 0.001f;
    [Tooltip("If active, the end point will be automatically refreshed")]
    public bool UseGameObjectAsEndpoint = false;
    [Tooltip("The start of the ray, only works when UseGameObjectAsEndpoint is active")]
    public GameObject Start = null;
    [Tooltip("The end of the ray, only works when UseGameObjectAsEndpoint is active")]
    public GameObject End = null;

    [HideInInspector]
    public Vector3 StartPoint = new Vector3();
    [HideInInspector]
    public Vector3 EndPoint = new Vector3();
    [HideInInspector]
    public MeshRenderer MeshRenderer = null;
    [HideInInspector]
    public MeshFilter MeshFilter = null;
    
    void OnEnable() { 
        if (!TryGetComponent<MeshRenderer>(out MeshRenderer)) {
            MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        if (!TryGetComponent<MeshFilter>(out MeshFilter)) {
            MeshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (isRayRendering()) StopRendering();
    }
    void Update() {
        if (UseGameObjectAsEndpoint) {
            Refresh(Start.transform.position, End.transform.position);
        }
    }
    public void StartRendering() { 
       MeshRenderer.enabled = true;
    }
    public void StopRendering() {
        MeshRenderer.enabled = false;
    }
    public bool isRayRendering() {
        return MeshRenderer.enabled;
    }
    public void SetRenderer(Material material, Mesh mesh) {
        MeshRenderer.material = material;
        MeshFilter.mesh = mesh;
    }
    /// <summary>
    /// Should be called after the start and end is set
    /// </summary>
    public void Refresh() {
        // Refresh position and scale
        transform.position = (EndPoint + StartPoint) / 2;
        transform.localScale =
            new Vector3(Radius, (EndPoint - StartPoint).magnitude / 2, Radius);
        // Refresh direction
        Vector3 dir = EndPoint - StartPoint;
        float angle = Vector3.Angle(new Vector3(0, 1, 0), dir);
        Vector3 axis = Vector3.Cross(dir, new Vector3(0, 1, 0));
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.Rotate(axis, -angle);
    }
    public void Refresh(Vector3 start, Vector3 end) {
        // Set start and end
        StartPoint = start; EndPoint = end;
        // Refresh position and scale
        transform.position = (EndPoint + StartPoint) / 2;
        transform.localScale =
            new Vector3(Radius, (EndPoint - StartPoint).magnitude / 2, Radius);
        // Refresh direction
        Vector3 dir = EndPoint - StartPoint;
        float angle = Vector3.Angle(new Vector3(0, 1, 0), dir);
        Vector3 axis = Vector3.Cross(dir, new Vector3(0, 1, 0));
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.Rotate(axis, -angle);
    }
    public void RenderWithGameObjects(GameObject start, GameObject end) {
        UseGameObjectAsEndpoint = true;
        Start = start; End = end;
    }
}