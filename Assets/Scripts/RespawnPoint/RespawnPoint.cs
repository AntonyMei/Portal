using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour {
    public RespawnController RespawnController;
    public GameObject RespawnPointIndicator;
    public GameObject FlagStand;
    public GameObject Flag;
    public Material ActiveFlagMaterial;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            RespawnController.RespawnPoint = RespawnPointIndicator;
            Flag.GetComponent<MeshRenderer>().material = ActiveFlagMaterial;
            FlagStand.GetComponent<MeshRenderer>().material = ActiveFlagMaterial;
        }
    }
}
