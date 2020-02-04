using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChainController : MonoBehaviour
{
    [Header("Cursor Settings")]
    [Tooltip("The normal texture of the cursor")]
    public Texture2D NormalTexture;
    [Tooltip("The texture of the cursor when its ray detect a target")]
    public Texture2D DetectedTexture;
    [Tooltip("The texture of the cursor when its ray hit a target")]
    public Texture2D HitTexture;
    [Tooltip("The Image that presents the cursor")]
    public RawImage CursorImage;

    [Header("Chain Settings")]
    [Tooltip("The maximum length of the chain")]
    public float MaxChainLength = 300f;
    [Tooltip("The camera attached to the player, used as the origin of the ray")]
    public Camera PlayerCamera;
    [Tooltip("The gameobject that randers the chain")]
    public ChainRenderer ChainRenderer;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update() {
        // Change cursor and ray
        RaycastHit hit_info;
        Ray ray = new Ray(PlayerCamera.transform.position, 
                          PlayerCamera.transform.forward);
        Physics.Raycast(ray, out hit_info);
        if(hit_info.distance < MaxChainLength && hit_info.distance != 0) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                CursorImage.texture = HitTexture;
                ChainRenderer.ResetEnd(hit_info.point);
            } else { CursorImage.texture = DetectedTexture; }
        } else { CursorImage.texture = NormalTexture; }
    }
}