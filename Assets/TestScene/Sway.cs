using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {

    public Vector3 amplitude = new Vector3(1, 1, 1);
    public Vector3 period = new Vector3(1, 1, 1);

    private Vector3 t;

	// Use this for initialization
	void Start () {
        t.x = Random.Range(0, period.x);
        t.y = Random.Range(0, period.y);
        t.z = Random.Range(0, period.z);
	}
	
	// Update is called once per frame
	void Update () {
        t.x = (t.x + Time.deltaTime) % period.x;
        t.y = (t.y + Time.deltaTime) % period.y;
        t.z = (t.z + Time.deltaTime) % period.z;

        transform.localPosition = new Vector3(
            amplitude.x * Mathf.Cos(2 * Mathf.PI / period.x * t.x),
            amplitude.y * Mathf.Cos(2 * Mathf.PI / period.y * t.y),
            amplitude.z * Mathf.Cos(2 * Mathf.PI / period.z * t.z));
    }
}
