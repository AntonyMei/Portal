using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script should be added to GameObjects AnchorGroup
/// </summary>
public class AnchorHighlightController : MonoBehaviour
{
    [Header("Render Settings")]
    [Tooltip("If anchor highlight is enabled")]
    public bool EnableHighlight = true;
    [Tooltip("Only anchors within this range can be highlighted")]
    public float HighlightRange = 30f;
    [Tooltip("Anchors nearest to the cursor and within this range will be highlighted (Pixel)")]
    public float HighlightTriggerRange = 200f;
    [Tooltip("The gameobjects anchor group")]
    public List<GameObject> AnchorGroup;

    [Header("Dependencies")]
    [Tooltip("The script ConnectionController")]
    public ConnectionController ConnectionController;
    [Tooltip("The raw image used as cursor")]
    public RawImage CursorImage;
    [Tooltip("The camera attached to the player")]
    public Camera PlayerCamera;

    [HideInInspector]
    public List<GameObject> AnchorList;
    [HideInInspector]
    public GameObject HighLightedAnchor;

    void OnEnable() {
        foreach (GameObject anchor_group in AnchorGroup) {
            for (int i = 0; i < anchor_group.transform.childCount; i++) {
                AnchorList.Add(anchor_group.transform.GetChild(i).gameObject);
            }
        }
    }
    void Update() {
        // Remove highlight
        if (HighLightedAnchor) {
            Anchor anchor_script = HighLightedAnchor.GetComponent<Anchor>();
            if (anchor_script.RayList.Count == 0) anchor_script.SetMaterial2Nonactive();
            else anchor_script.SetMaterial2Active();
        }
        // Calculate highlight
        if(EnableHighlight && ConnectionController.IsFirstAnchorSet()) {
            GameObject nearest_anchor = null;
            float nearest_anchor_distance = HighlightTriggerRange;
            foreach(GameObject anchor in AnchorList) {
                float Anchor2CamDis = Vector3.Distance(anchor.transform.position, 
                    PlayerCamera.transform.position);
                float Anchor2CursorDis = Vector3.Distance(CursorImage.rectTransform.position, 
                    PlayerCamera.WorldToScreenPoint(anchor.transform.position));
                if (Anchor2CamDis <= HighlightRange && Anchor2CursorDis <= nearest_anchor_distance) {
                    nearest_anchor = anchor;
                    nearest_anchor_distance = Anchor2CursorDis;
                } 
            }
            if (nearest_anchor && nearest_anchor.GetComponent<Anchor>().RayList.Count == 0
                && nearest_anchor != ConnectionController.GetFirstAnchor()) {
                nearest_anchor.GetComponent<Anchor>().SetMaterial2Highlighted();
                HighLightedAnchor = nearest_anchor;
            } else { HighLightedAnchor = null; }
        } else { HighLightedAnchor = null; }
    }
}
