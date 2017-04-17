using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Split : MonoBehaviour {

    public GameObject segment;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetKeyDown("space")) {
            //Vector3 pos = segment.transform.position;
            print(segment.transform.position);
            print(segment.transform.rotation.eulerAngles);
            segment.transform.parent = null;
            //float playbackTime = animator.playbackTime;
            animator.Rebind();
            //animator.playbackTime = playbackTime;
            //segment.transform.position = pos;
        }
	}
}
