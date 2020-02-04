using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [Tooltip("The minimal length of the chain")]
    public float MinLength = 20;
    [Tooltip("The line renderer")]
    public LineRenderer ChainRenderer;
    [Tooltip("The start point of the chain")]
    public GameObject ChainStartPoint;

    void Update() {
        ResetStartPoint();
        Vector3 ChainLength = ChainRenderer.GetPosition(0) 
                            - ChainRenderer.GetPosition(1);
        if(ChainLength.magnitude < MinLength) { ChainRenderer.enabled = false; }
    }
    public void ResetStartPoint() {
        ChainRenderer.SetPosition(0, ChainStartPoint.transform.position);
    }
}
