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

    // The start of ray in world space
    [HideInInspector]
    public Vector3 StartPoint = new Vector3();
    // The end of ray in world space
    [HideInInspector]
    public Vector3 EndPoint = new Vector3();
    // The MeshRenderer that renders the ray
    [HideInInspector]
    public MeshRenderer MeshRenderer = null;
    // The MeshFilter attached to the ray
    [HideInInspector]
    public MeshFilter MeshFilter = null;
    
    void OnEnable() { 
        // Initializes ray renderer
        if (!TryGetComponent<MeshRenderer>(out MeshRenderer)) {
            MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        if (!TryGetComponent<MeshFilter>(out MeshFilter)) {
            MeshFilter = gameObject.AddComponent<MeshFilter>();
        }
        // Rendering is disabled when ray is created
        // End points must first be set(in Vector3 or in GameObject) before rendering can start
        if (IsRayRendering()) StopRendering();
    }

    void Update() {
        // If the ray uses gameobjects as end points,
        // the end points will be refreshed automatically every frame
        if (UseGameObjectAsEndpoint) {
            Refresh(Start.transform.position, End.transform.position);
        }
    }

    /// <summary>
    /// <para> Start rendering the ray </para>
    /// <para> Must be called after end points are set </para>
    /// </summary>
    public void StartRendering() { 
       MeshRenderer.enabled = true;
    }

    /// <summary>
    /// <para> Stop rendering the ray </para>
    /// </summary>
    public void StopRendering() {
        MeshRenderer.enabled = false;
    }

    /// <summary>
    /// Returns whether the ray is rendering
    /// </summary>
    /// <returns> Rendering state of the ray </returns>
    public bool IsRayRendering() {
        return MeshRenderer.enabled;
    }

    /// <summary>
    /// <para> Set the ray's material and mesh </para>
    /// </summary>
    /// <param name="material"> The material used to render the ray </param>
    /// <param name="mesh"> The mesh of the ray </param>
    public void SetRenderer(Material material, Mesh mesh) {
        MeshRenderer.material = material;
        MeshFilter.mesh = mesh;
    }

    /// <summary>
    /// <para> Should be called after the start and end are set </para>
    /// <para> 
    /// Notice that the parent of the ray must be world (or have eulerAngles (0, 0, 0))
    /// , otherwise transform may go wrong
    /// </para>
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

    /// <summary>
    /// <para> Should be called after the start and end are set </para>
    /// <para> 
    /// Notice that the parent of the ray must be world (or have eulerAngles (0, 0, 0))
    /// , otherwise transform may go wrong
    /// </para>
    /// </summary>
    /// <param name="start"> The start point of the ray </param>
    /// <param name="end"> The end point of the ray </param>
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

    /// <summary>
    /// <para> Should be called after the start and end are set </para>
    /// <para> 
    /// Notice that the parent of the ray must be world (or have eulerAngles (0, 0, 0))
    /// , otherwise transform may go wrong
    /// </para>
    /// </summary>
    /// <param name="start"> The start point of the ray </param>
    /// <param name="end"> The end point of the ray </param>
    /// <param name="radius"> The radius of the ray </param>
    public void Refresh(Vector3 start, Vector3 end, float radius) {
        // Set start, end and radius
        StartPoint = start; EndPoint = end; Radius = radius;
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

    /// <summary>
    /// <para> Set the end points of the ray with gameobjects </para>
    /// <para> This fuction will also turn on UseGameObjectAsEndpoint automatically </para> 
    /// </summary>
    /// <param name="start"> The game object used as start point </param>
    /// <param name="end"> The game object used as end point </param>
    public void SetRendererWithGameObjects(GameObject start, GameObject end) {
        UseGameObjectAsEndpoint = true;
        Start = start; End = end;
    }
}