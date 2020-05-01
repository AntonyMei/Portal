using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalActivator : MonoBehaviour {
    /* Objects to activate */
    public GameObject[] ActivationList;
    public double ActivateThreshold;
    public GameObject LeftColumn;
    public GameObject RightColumn;

    /* Materials */
    public Material ActivePortalMaterial;
    public Material HalfActivePortalMaterial;
    public Material ColumnGlowMaterial;

    public void ActivatePortal(double input_strength) {
        if(input_strength < ActivateThreshold) {
            GetComponent<MeshRenderer>().material = HalfActivePortalMaterial;
        } else {
            GetComponent<MeshRenderer>().material = ActivePortalMaterial;
            foreach(GameObject obj in ActivationList) {
                obj.SetActive(true);
            }
            LeftColumn.GetComponent<MeshRenderer>().materials[1] = ColumnGlowMaterial;
            RightColumn.GetComponent<MeshRenderer>().materials[1] = ColumnGlowMaterial;
        }
    }
}
