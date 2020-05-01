using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnController : MonoBehaviour
{
    public GameObject RespawnPoint;
    public GameObject Player;
    public Image BlackImage;
    public Text RespawnText;

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Respawn();
        }
    }
    public void Respawn() {
        Player.transform.position = RespawnPoint.transform.position;
        BlackImage.enabled = false;
        RespawnText.enabled = false;
    }
}
