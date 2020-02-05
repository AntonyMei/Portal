using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayRenderer : MonoBehaviour
{
    [Header("Render Settings")]
    [Tooltip("The minimal length of the ray")]
    public float MinLength = 3f;
    [Tooltip("The radius of the ray")]
    public float Radius = 0.5f;
    [Tooltip("The start point of the ray")]
    public GameObject RayStartPoint;

    [Header("Force Settings")]
    [Tooltip("The player that receives the force")]
    public Rigidbody Player;
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;
    [Tooltip("The controller of the camera")]
    public CameraController CameraController;
    [Tooltip("The force of the ray")]
    public float PullStrength = 10.0f;
    [Tooltip("Type of the force")]
    public ForceMode ForceMode = ForceMode.Force;

    private Vector3 EndPoint = new Vector3();

    void Update() {
        Refresh();
        float RayLength = (EndPoint - RayStartPoint.transform.position).magnitude;
        if(RayLength < MinLength || Input.GetKeyUp(KeyCode.Mouse0)) {
            GetComponent<MeshRenderer>().enabled = false;
        }
        if(isRayEnabled()) {
            Vector3 dir = EndPoint - RayStartPoint.transform.position;
            dir.Normalize();
            Player.AddForce(dir * PullStrength, ForceMode);
            PlayerCamera.transform.LookAt(EndPoint);
            CameraController.RefershCameraRotation();
        }
    }
    public void ResetEnd(Vector3 end_point) {
        EndPoint = end_point; Refresh();
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void Refresh() {
        // Refresh position and scale
        Vector3 StartPoint = RayStartPoint.transform.position;
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
    public bool isRayEnabled() {
        return GetComponent<MeshRenderer>().enabled;
    }
}