using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class ForceRespawn : MonoBehaviour {
    public GameObject GameManager;
    public Image BlackImage;
    public Text RespawnText;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            BlackImage.enabled = true;
            RespawnText.enabled = true;
        }
    }
}
