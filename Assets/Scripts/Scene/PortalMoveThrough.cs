using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMoveThrough : MonoBehaviour {
    public GameObject Player;
    public GameObject Destination;
    public PortalActivator PortalActivator;
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Player.transform.position = Destination.transform.position;
            // Disable the portal camera to reduce cost
            foreach(var obj in PortalActivator.ActivationList) {
                obj.SetActive(false);
            }
        }
    }
}
