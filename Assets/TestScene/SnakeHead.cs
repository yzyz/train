using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour {

    public GameObject upperJaw, lowerJaw;
    public float upperCloseAngle, upperOpenAngle;
    public float lowerCloseAngle, lowerOpenAngle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenMouth(float t) {
        float upperAngle = Mathf.Lerp(upperCloseAngle, upperOpenAngle, t);
        float lowerAngle = Mathf.Lerp(lowerCloseAngle, lowerOpenAngle, t);

        upperJaw.transform.localRotation = Quaternion.Euler(-180, upperAngle, 0);
        lowerJaw.transform.localRotation = Quaternion.Euler(-180, lowerAngle, 0);
    }
}
