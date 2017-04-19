using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydra : MonoBehaviour {

    public HydraHead[] heads;

    public float minTimeBetweenAttacks = 2;
    public float maxTimeBetweenAttacks = 8;

    private float timeToAttack;
    private HydraHead headToAttack;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (headToAttack == null) {
            bool allWaiting = true;
            foreach (HydraHead head in heads) {
                if (head.state != HydraHead.State.WAITING) {
                    allWaiting = false;
                }
            }
            if (allWaiting) {
                List<HydraHead> aliveHeads = new List<HydraHead>();
                foreach (HydraHead head in heads) {
                    if (head.status == HydraHead.Status.ALIVE) {
                        aliveHeads.Add(head);
                    }
                }
                if (aliveHeads.Count > 0) {
                    int i = Random.Range(0, aliveHeads.Count);
                    headToAttack = aliveHeads[i];
                    timeToAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
                }
                else {
                    print("all heads are dead");
                }
            }
        } else {
            timeToAttack -= Time.deltaTime;
            if (timeToAttack <= 0) {
                timeToAttack = 0;
                headToAttack.Attack();
                headToAttack = null;
            }
        }
    }
}
