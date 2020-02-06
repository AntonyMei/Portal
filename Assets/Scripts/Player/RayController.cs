using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayController : MonoBehaviour {
    [Header("Cursor Settings")]
    [Tooltip("The normal texture of the cursor")]
    public Texture2D NormalTexture;
    [Tooltip("The texture of the cursor when its ray detect an anchor")]
    public Texture2D DetectedTexture;
    [Tooltip("The texture of the cursor when mouse0 is pressed")]
    public Texture2D PressedTexture;
    [Tooltip("The Image that presents the cursor")]
    public RawImage CursorImage;

    [Header("Ray Settings")]
    [Tooltip("The maximum length of the ray")]
    public float MaxRayLength = 300f;
    [Tooltip("The camera attached to the player, used as the origin of the ray")]
    public Camera PlayerCamera;
    [Tooltip("The gameobject that randers the ray")]
    public RayRenderer RayRenderer;
    [Tooltip("The end of player's ray")]
    public GameObject RayStartIndicator;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update() {
        // Cast a virtual ray to detect
        RaycastHit hit_info;
        Ray ray = new Ray(PlayerCamera.transform.position,
                          PlayerCamera.transform.forward);
        Physics.Raycast(ray, out hit_info);
        // Change cursor sprite and generate ray entity
        if (hit_info.distance < MaxRayLength && hit_info.distance != 0) {
            if (Input.GetKey(KeyCode.Mouse0)) {
                CursorImage.texture = PressedTexture;
                Vector3 start = RayStartIndicator.transform.position;
                Vector3 end = hit_info.point;
                RayRenderer.Refresh(start, end);
                RayRenderer.StartRendering();
            } else if (hit_info.transform.tag == "Anchor") {
                CursorImage.texture = DetectedTexture;
                RayRenderer.StopRendering();
            } else { 
                CursorImage.texture = NormalTexture;
                RayRenderer.StopRendering();
            }
        } else {
            if (Input.GetKey(KeyCode.Mouse0)) {
                CursorImage.texture = PressedTexture;
                Vector3 start = RayStartIndicator.transform.position;
                Vector3 end = PlayerCamera.transform.position +
                              PlayerCamera.transform.forward * MaxRayLength;
                RayRenderer.Refresh(start, end);
                RayRenderer.StartRendering();
            } else {
                CursorImage.texture = NormalTexture;
                RayRenderer.StopRendering();
            }
        }
    }
}