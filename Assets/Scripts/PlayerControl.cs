using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

	public string overworld = "Overworld";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Fade to black and send the player back to the Overworld
	void Die () {
		//TODO: fade to black

		SceneManager.LoadScene (overworld);
	}

	// Flash red
	void Injure () {

	}
}
