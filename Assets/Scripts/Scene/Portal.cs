using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Portal : MonoBehaviour
{
    public GameObject Other;
    public Material UpperMaterial;
    public Material LowerMaterial;
    public bool isFixed = true;
    void Update() {
        if (!isFixed) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if(transform.position.y < Other.transform.position.y) {
                renderer.material = LowerMaterial;
            } else {
                renderer.material = UpperMaterial;
            }
        }
    }
    public  void RefreshColor() {
        if (Other) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (transform.position.y < Other.transform.position.y) {
                renderer.material = LowerMaterial;
            } else {
                renderer.material = UpperMaterial;
            }
        }
    }
}
