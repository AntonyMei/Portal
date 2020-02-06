using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class RayRenderer : MonoBehaviour {
    [Header("Render Settings")]
    [Tooltip("The radius of the ray")]
    public float Radius = 0.001f;

    [HideInInspector]
    public Vector3 StartPoint = new Vector3();
    [HideInInspector]
    public Vector3 EndPoint = new Vector3();
    
    void OnEnable() { StopRendering(); }
    public void StartRendering() { 
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void StopRendering() {
        GetComponent<MeshRenderer>().enabled = false;
    }
    public bool isRayRendering() {
        return GetComponent<MeshRenderer>().enabled;
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
}