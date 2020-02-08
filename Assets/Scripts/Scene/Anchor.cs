using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [Header("Render Settings")]
    [Tooltip("Has the anchor been connected")]
    public bool IsActive = false;
    [Tooltip("The material set to active anchor")]
    public Material ActiveAnchorMaterial;
    [Tooltip("The material set to non-active anchor")]
    public Material NonactiveAnchorMaterial;
    [Tooltip("The material for highlighted anchors")]
    public Material HighlightMaterial;

    [Header("Connections")]
    [Tooltip("The list that stores all the rays connected to the anchor")]
    public List<string> RayList = new List<string>();
    [Tooltip("The mesh renderer attached to the anchor"), HideInInspector]
    public MeshRenderer MeshRenderer;

    void OnEnable() {
        // Get the MeshRenderer attached
        if (!TryGetComponent<MeshRenderer>(out MeshRenderer)) {
            MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
    }

    void Update() {
        // Change active state
        if (RayList.Count != 0) IsActive = true;
        else IsActive = false;
    }

    /// <summary>
    /// Set the anchor's material to ActiveMaterial
    /// </summary>
    public void SetMaterial2Active() {
        MeshRenderer.material = ActiveAnchorMaterial;
    }

    /// <summary>
    /// Set the anchor's material to NonactiveMaterial
    /// </summary>
    public void SetMaterial2Nonactive() {
        MeshRenderer.material = NonactiveAnchorMaterial;
    }

    /// <summary>
    /// Set the anchor's material to HighlightMaterial
    /// </summary>
    public void SetMaterial2Highlighted() {
        MeshRenderer.material = HighlightMaterial;
    }
}
