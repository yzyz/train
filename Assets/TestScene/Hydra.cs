using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydra : MonoBehaviour {

    public GameObject scriptManager;

    public HydraHead[] heads;
    public float[] attackTimes;

    public float minTimeBetweenAttacks = 2;
    public float maxTimeBetweenAttacks = 8;
    public float victoryTime = 8;

    private float timeToAttack;
    private HydraHead headToAttack;
    private float setTime;
    private int headsLeft;

    // Use this for initialization
    void Start () {
        headsLeft = heads.Length;
        LoadLevel(heads.Length - headsLeft + 1);
	}

    // Remove a head
    public void LoseHead() {
        headsLeft -= 1;
        LoadLevel(heads.Length - headsLeft + 1);
        if (headsLeft == 0) {
            setTime = Time.time;
        }
    }

    // Loads a difficulty level
    void LoadLevel(int levelIndex) {
        print("Loading difficulty: " + levelIndex);
        SnakeLevel level = scriptManager.transform.FindChild("Level" + levelIndex).GetComponent<SnakeLevel>();
        minTimeBetweenAttacks = level.minTimeBetweenAttacks;
        maxTimeBetweenAttacks = level.maxTimeBetweenAttacks;
        HydraHead.attackTime = level.attackTime;
        HydraHead.waitTime = level.waitTime;
        HydraHead.pausedTime = level.pausedTime;
        HydraHead.retreatTime = level.retreatTime;
        HydraHead.regenWaitTime = level.regenWaitTime;
        HydraHead.regenSegmentTime = level.regenSegmentTime;
        HydraHead.regenHeadTime = level.regenHeadTime;
    }

    // Update is called once per frame
    void Update() {
        if (headsLeft > 0) {
            if (headToAttack == null) {
                bool allWaiting = true;
                foreach (HydraHead head in heads) {
                    if (head.state == HydraHead.State.ATTACKING
                        || head.state == HydraHead.State.PAUSED
                        || head.state == HydraHead.State.RETREATING) {
                        allWaiting = false;
                    }
                }
                if (allWaiting) {
                    List<HydraHead> aliveHeads = new List<HydraHead>();
                    foreach (HydraHead head in heads) {
                        if (head.status == HydraHead.Status.ALIVE && head.state == HydraHead.State.WAITING) {
                            aliveHeads.Add(head);
                        }
                    }
                    if (aliveHeads.Count > 0) {
                        int i = Random.Range(0, aliveHeads.Count);
                        headToAttack = aliveHeads[i];
                        timeToAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
                        //HydraHead.attackTime = HydraHead.retreatTime = attackTimes[heads.Length - aliveHeads.Count];
                    }
                    else {
                        //print("all heads are not ready to attack");
                    }
                }
            }
            else {
                timeToAttack -= Time.deltaTime;
                if (timeToAttack <= 0) {
                    timeToAttack = 0;
                    headToAttack.Attack();
                    headToAttack = null;
                }
            }
        } else {
            if (Time.time > setTime + victoryTime) {
                scriptManager.GetComponent<PlayerControl>().Die();
            }
        }
    }
}
