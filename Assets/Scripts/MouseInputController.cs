using UnityEngine;
using System.Collections;

public class MouseInputController : MonoBehaviour {

    public float sens = 0.1f;
	
	// Update is called once per frame
	void Update () {
        float x = sens * Input.GetAxis("Mouse X");
        float y = sens * Input.GetAxis("Mouse Y");
        transform.Translate(x, y, 0);
	}
}
