using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeStump : MonoBehaviour {

    public HydraHead head;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Fire")) {
            head.Burn();
        }
    }
}
