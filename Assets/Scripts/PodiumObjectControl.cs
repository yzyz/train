using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PodiumObjectControl : MonoBehaviour {

    public GameObject player;
    public GameObject manager;
    public AudioClip teleportClip;

	public string sceneName;
	public Vector3 rot;
    public ushort hapticStrength = 400;
    public int fadeTime = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (rot);
	}

	void OnTriggerStay(Collider other) {
		if (!other.tag.Equals("GameController")) return;

		SteamVR_TrackedObject trackedObj = other.GetComponentInParent<SteamVR_TrackedObject>();
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);
        SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(hapticStrength);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            manager.GetComponent<AudioSource>().PlayOneShot(teleportClip);
            SteamVR_LoadLevel loader = player.GetComponent<SteamVR_LoadLevel>();
            loader.levelName = sceneName;
            loader.fadeOutTime = fadeTime;
            loader.fadeInTime = fadeTime;
            loader.Trigger();
		}
	}
}
