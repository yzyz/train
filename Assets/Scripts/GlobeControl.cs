using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeControl : MonoBehaviour {

	public enum GlobeState {
		IDLE, MOVING, DRAGGING, STOPPED
	}

	/* Objects */
	public GameObject globe;

	/* Constants */
	public float distFromCam = 1;
	public float waitTime = 2f;
	public float velocityDeltaTime = .1f;
	public float angleDecay = .8f;
	public float tolerance = .1f;
	public float maxSpeed = 1;
	public float acceleration = .05f;

	/* Objects and Components */
	public GlobeState globeState;
	private GlobeControl spinScript;
	private GameObject activeController;

	/* Logic / Function Variables */
	private Quaternion originalRotation;
	private Quaternion movementRotation;
	private float movementDeltaTime;
	private Vector3 startVec;
	private LinkedList<Quaternion> rots;
	private LinkedList<float> times;
	private float angleScale = 1;
	private float storedTime;
	private float currSpeed;

	// Use this for initialization
	void Start () {
		currSpeed = maxSpeed;

		globeState = GlobeState.IDLE;

		rots = new LinkedList<Quaternion> ();
		times = new LinkedList<float> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	// FixedUpdate is called once every fixed time frame
	void FixedUpdate () {
		print (globeState);
		if (globeState == GlobeState.DRAGGING) {
			SteamVR_TrackedObject trackedObj = activeController.GetComponent<SteamVR_TrackedObject> ();
			SteamVR_Controller.Device device = SteamVR_Controller.Input ((int)trackedObj.index);
			Vector3 currPos = trackedObj.transform.position;

			if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
				// Update Rotation
				Vector3 endVec = (currPos - globe.transform.position);
				Quaternion rot = Quaternion.FromToRotation (startVec, endVec);
				globe.transform.rotation = rot * originalRotation;

				// Update lists
				rots.AddLast (globe.transform.rotation);
				times.AddLast (Time.time);
				while (times.First.Value < Time.time - velocityDeltaTime) {
					rots.RemoveFirst ();
					times.RemoveFirst ();
				}
			} else if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				// Set / Reinitialize values
				movementRotation = Quaternion.Inverse (rots.First.Value) * globe.transform.rotation;
				movementDeltaTime = Time.time - times.First.Value;
				angleScale = 1;
				globeState = GlobeState.MOVING;
			}
		} else if (globeState == GlobeState.MOVING) {
			// Check if we've stopped
			if (angleScale <= tolerance) {
				globeState = GlobeState.STOPPED;
				storedTime = Time.time;
			} else { // Otherwise update globe rotation
				globe.transform.rotation *= Quaternion.Slerp (Quaternion.identity, movementRotation, Time.fixedDeltaTime / movementDeltaTime * angleScale);
				angleScale *= angleDecay;
			}
		} else if (globeState == GlobeState.STOPPED) {
			if (Time.time - storedTime > waitTime) {
				globeState = GlobeState.IDLE;
			}
		} else if (globeState == GlobeState.IDLE) {
			// Rotate by velocity
			transform.Rotate (new Vector3 (0, currSpeed, 0));

			// Accelerate speed to a cap
			currSpeed += acceleration;
			if (currSpeed > maxSpeed) {
				currSpeed = maxSpeed;
			}
		}
			
	}

	void OnTriggerStay(Collider other) {
		// TODO: put in check that it's a controller that triggered it
        if (!other.tag.Equals("GameController")) {
            return;
        }

		SteamVR_TrackedObject trackedObj = other.GetComponentInParent<SteamVR_TrackedObject> ();
		SteamVR_Controller.Device device = SteamVR_Controller.Input ((int)trackedObj.index);

		// We're in range and we've hit the trigger
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
			// Grab our position in world space
			Vector3 currPos = other.transform.position;

			// Reinitialize values
			originalRotation = globe.transform.rotation;
			startVec = (currPos - globe.transform.position);
			rots.Clear ();
			times.Clear ();

			// Set variables
			activeController = other.transform.parent.gameObject;
			globeState = GlobeState.DRAGGING;
            currSpeed = 0;

		}
	}
}
