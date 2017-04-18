using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PodiumObjectControl : MonoBehaviour {

	public string sceneName;
	public Vector3 rot;

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

		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
			SceneManager.LoadScene(sceneName);
		}
	}
}
