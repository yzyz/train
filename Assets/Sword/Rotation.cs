using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
    
	void Update () {
        transform.Rotate(new Vector3(0f,1.5f,0f));
	}
}
