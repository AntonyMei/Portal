using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEntity : MonoBehaviour
{
    [Tooltip("The maximum time of reflection"), HideInInspector]
    public int MaxReflectionCount = -1;
    [Tooltip("The origin of this ray"), HideInInspector]
    public Vector3 Origin;
    [Tooltip("The direction of this ray"), HideInInspector]
    public Vector3 Direction;

    private Mirror last_mirror = null;

    private void Update() {
        // Detect what object this ray hits
        RaycastHit hit_info = new RaycastHit();
        Ray detection_ray = new Ray(Origin, Direction);
        Physics.Raycast(detection_ray, out hit_info);
        if (hit_info.distance != 0 && hit_info.transform.tag == "Mirror") {
            Mirror mirror_script = hit_info.transform.GetComponent<Mirror>();
            //mirror_script.GenerateOutputRay(detection_ray.direction, hit_info.point, )
        }
    }
}
