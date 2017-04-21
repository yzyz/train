using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeStump : MonoBehaviour {

    public HydraHead head;
	public AudioClip slash;
    public AudioClip burn;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Fire")) {
            head.Burn();
        }
    }

	public void PlaySound(string id) {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (id.Equals ("SLASH")) {
			audioSource.PlayOneShot (slash);
		} else if (id.Equals("BURN")) {
			audioSource.PlayOneShot (burn);
		}
	}
}
