using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour {

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

	/* Objects and Components */
	private Camera cam;
	private GlobeState globeState;
	private GlobeControl spinScript;

	/* Logic / Function Variables */
	private Quaternion originalRotation;
	private Quaternion movementRotation;
	private float movementDeltaTime;
	private Vector3 startVec;
	private LinkedList<Quaternion> rots;
	private LinkedList<float> times;
	private float angleScale = 1;
	private float storedTime;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		globeState = GlobeState.IDLE;
		spinScript.globeState = (GlobeControl.GlobeState) GlobeState.IDLE;
		spinScript = globe.GetComponent<GlobeControl>();

		rots = new LinkedList<Quaternion> ();
		times = new LinkedList<float> ();
	}

	// Update is called once per frame
	void FixedUpdate () {

		/* INPUT (replace with VR version when necessary) */
		// Set the current position from the mouse
		Vector3 currPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCam));

		// Handle input
		if (Input.GetMouseButtonDown (0)) {

			// Disable idling rotation
			//spinScript.StopSpin ();

			// Reinitialize values
			originalRotation = globe.transform.rotation;
			startVec = (currPos - globe.transform.position);
			rots.Clear ();
			times.Clear ();
		}

		if (Input.GetMouseButton (0)) {
			// Update Rotation
			Vector3 endVec = (currPos - globe.transform.position);
			Quaternion rot = Quaternion.FromToRotation (startVec, endVec);
			globe.transform.rotation = rot * originalRotation;

			// Update lists
			rots.AddLast(globe.transform.rotation);
			times.AddLast (Time.time);
			while (times.First.Value < Time.time - velocityDeltaTime) {
				rots.RemoveFirst ();
				times.RemoveFirst ();
			}

		} else if (Input.GetMouseButtonUp (0)) {
			// Set / Reinitialize values
			movementRotation = Quaternion.Inverse(rots.First.Value) * globe.transform.rotation;
			movementDeltaTime = Time.time - times.First.Value;
			angleScale = 1;
			globeState = GlobeState.MOVING;
			spinScript.globeState = (GlobeControl.GlobeState) GlobeState.MOVING;
		}

		// Handle movement from previous momentum
		if (globeState == GlobeState.MOVING) {

			// Check if we've stopped
			if (angleScale <= tolerance) {
				globeState = GlobeState.STOPPED;
				spinScript.globeState = (GlobeControl.GlobeState) GlobeState.STOPPED;
				storedTime = Time.time;
			}

			// Update globe rotation
			globe.transform.rotation *= Quaternion.Slerp (Quaternion.identity, movementRotation, Time.fixedDeltaTime / movementDeltaTime * angleScale);
			angleScale *= angleDecay;

		} else if (globeState == GlobeState.STOPPED) {
			if (Time.time - storedTime > waitTime) {
				globeState = GlobeState.IDLE;
				spinScript.globeState = (GlobeControl.GlobeState) GlobeState.IDLE;
				//spinScript.StartSpin ();
			}
		}
	}
}
