using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

    public GameObject player;
	public string overworld = "Overworld";
    public int maxLives = 3;
    public float fadeTime = 1.5f;
    public float invulTime = 3f;

    public int lives;

    private float setTime;
    private bool invul = false;

	// Use this for initialization
	void Start () {
        lives = maxLives;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Fade to black and send the player back to the Overworld
	public void Die () {
        print("YOU DIED");
        SteamVR_LoadLevel loader = player.GetComponent<SteamVR_LoadLevel>();
        loader.levelName = overworld;
        loader.fadeOutTime = fadeTime;
        loader.fadeInTime = fadeTime;
        loader.Trigger();
	}

	// Flash red
	public void Injure () {
        if (!invul) {
            print("YOU GOT HIT");
            lives -= 1;
            if (lives == 0) Die();
            invul = true;
            setTime = Time.time;
        } else if (Time.time > setTime + invulTime) {
            invul = false;
        }
	}
}
