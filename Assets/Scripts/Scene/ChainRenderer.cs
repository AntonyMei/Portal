using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainRenderer : MonoBehaviour
{
    [Tooltip("The minimal length of the chain")]
    public float MinLength = 10f;
    [Tooltip("The radius of the chain")]
    public float Radius = 0.5f;
    [Tooltip("The start point of the chain")]
    public GameObject ChainStartPoint;

    private Vector3 EndPoint = new Vector3();

    void Update() {
        Refresh();
        Vector3 ChainLength = EndPoint - ChainStartPoint.transform.position;
        if(ChainLength.magnitude < MinLength) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public void ResetEnd(Vector3 end_point) {
        GetComponent<MeshRenderer>().enabled = true;
        EndPoint = end_point; 
    }
    public void Refresh() {
        // Refresh position and scale
        Vector3 StartPoint = ChainStartPoint.transform.position;
        transform.position = (EndPoint + StartPoint) / 2;
        transform.localScale = 
            new Vector3(Radius,(EndPoint - StartPoint).magnitude,Radius);
        // Refresh direction
        Vector3 dir = EndPoint - StartPoint;
        //float angle_1 = Vector3.Angle(new Vector3(0, 1, 0), dir);
        //float angle_2 = Vector3.Angle(new Vector3(), dir);
        //transform.eulerAngles = new Vector3();
        //transform.Rotate(transform.right, angle_1);
        float angle = Vector3.Angle(new Vector3(0, 1, 0), dir);
        Vector3 axis = Vector3.Cross(dir, new Vector3(0, 1, 0));
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.Rotate(axis, -angle);
    }
}