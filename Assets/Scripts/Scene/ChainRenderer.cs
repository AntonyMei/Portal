﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainRenderer : MonoBehaviour
{
    [Tooltip("The minimal length of the chain")]
    public float MinLength = 3f;
    [Tooltip("The radius of the chain")]
    public float Radius = 0.5f;
    [Tooltip("The start point of the chain")]
    public GameObject ChainStartPoint;

    private Vector3 EndPoint = new Vector3();

    void Update() {
        Refresh();
        Vector3 ChainLength = EndPoint - ChainStartPoint.transform.position;
        if(ChainLength.magnitude < MinLength || Input.GetKeyUp(KeyCode.Mouse0)) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public void ResetEnd(Vector3 end_point) {
        EndPoint = end_point; Refresh();
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void Refresh() {
        // Refresh position and scale
        Vector3 StartPoint = ChainStartPoint.transform.position;
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