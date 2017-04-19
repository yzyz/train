using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnakeCollision : MonoBehaviour {

    public PlayerControl playerControl;

	void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            playerControl.Die();
        }
    }
}
