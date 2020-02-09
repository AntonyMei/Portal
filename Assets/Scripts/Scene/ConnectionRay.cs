using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionRay : MonoBehaviour
{
    [Header("Render Settings")]
    [Tooltip("The material of active ray")]
    public Material ActiveMaterial;
    [Tooltip("The material of nonactive ray")]
    public Material NonactiveMaterial;

    [Tooltip("Is the ray active"), HideInInspector]
    public bool IsActive = false;
    [Tooltip("The list of mirror attached to the ray"), HideInInspector]
    public List<string> MirrorList = new List<string>();
    [Tooltip("The mesh renderer attached to the anchor"), HideInInspector]
    public MeshRenderer MeshRenderer;

    void OnEnable() {
        if (!TryGetComponent<MeshRenderer>(out MeshRenderer)) {
            MeshRenderer = new MeshRenderer();
        }
    }

    void Update() {
        if (MirrorList.Count == 0) IsActive = false;
        else IsActive = true;
    }

    /// <summary>
    /// Set the anchor's material to ActiveMaterial
    /// </summary>
    public void SetMaterial2Active() {
        MeshRenderer.material = ActiveMaterial;
    }

    /// <summary>
    /// Set the anchor's material to NonactiveMaterial
    /// </summary>
    public void SetMaterial2Nonactive() {
        MeshRenderer.material = NonactiveMaterial;
    }
}
