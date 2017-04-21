using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeStump : MonoBehaviour {

    public HydraHead head;
	public AudioClip slash;
	public AudioClip burn;

	private AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Fire")) {
            head.Burn();
        }
    }

	public void PlaySound(string id) {
		if (id.Equals ("SLASH")) {
			audioSource.PlayOneShot (slash);
		} else if (id.Equals("BURN")) {
			audioSource.PlayOneShot (burn);
		}
	}
}
