using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScene : MonoBehaviour {
    public Image BlackBackGround;
    public Text EndSceneText;
    void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Player") {
            BlackBackGround.enabled = true;
            EndSceneText.enabled = true;
        }
    }
}
