using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour {
    public int id;
    public HydraHead head;

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Sword")) {
            head.Cut(id);
        }
    }
}
