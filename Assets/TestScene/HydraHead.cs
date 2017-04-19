﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraHead : MonoBehaviour {

    public enum State {
        WAITING, ATTACKING, RETREATING, REGENWAIT, REGENSEGMENTS, REGENHEAD
    }

    public enum Status {
        DEAD, CUT, ALIVE
    }

    public GameObject segmentPrefab;
    public GameObject headPrefab;
    public GameObject stumpPrefab;

    public GameObject targetObject;
    public GameObject waitTarget;
    public GameObject attackTarget;
    public GameObject player;

    public int id;

    public float segmentLength = 0.05f;
    public int numSegments = 100;
    public float tangentWeight = 5f;

    private GameObject[] segments;
    private GameObject head;
    private GameObject stump1, stump2;
    private SnakeSpline ss;
    private SnakeHead snakeHead;

    public State state;
    public Status status;

    int cut;

    float t;

	// Use this for initialization
	void Start () {
        segments = new GameObject[numSegments];
        for (int i = 0; i < numSegments; i++) {
            //Transform parent = i > 0 ? segments[i - 1].transform : transform;
            Transform parent = transform;
            segments[i] = Instantiate(segmentPrefab, parent);
            SnakeSegment seg = segments[i].GetComponent<SnakeSegment>();
            seg.id = i;
            seg.head = this;
        }
        head = Instantiate(headPrefab, segments[numSegments-1].transform);
        head.transform.localPosition = new Vector3(0, 0, segmentLength);
        head.transform.localRotation = Quaternion.identity;
        snakeHead = head.GetComponent<SnakeHead>();

        stump1 = Instantiate(stumpPrefab, transform);
        stump1.GetComponent<SnakeStump>().head = this;
        stump1.SetActive(false);
        stump2 = Instantiate(stumpPrefab, transform);
        stump2.GetComponent<SnakeStump>().head = this;
        stump2.GetComponent<BoxCollider>().enabled = false;
        stump2.SetActive(false);

        ss = new SnakeSpline(this);

        state = State.WAITING;
        status = Status.ALIVE;
        cut = numSegments;
        t = 0;
    }

    // Update is called once per frame
    void Update () {
        SetTargetObject();

        ss.Update();
        for (int i = 0; i < cut; i++) {
            ss.SetSegment(segments, i);
        }
	}

    public static float waitTime = 1;
    public static float attackTime = 0.3f;
    public static float retreatTime = 0.3f;
    public static float regenWaitTime = 3f;
    public static float regenSegmentTime = 0.1f;
    public static float regenHeadTime = 1f;

    void SetTargetObject() {
        t += Time.deltaTime;
        if (state == State.WAITING) {
            t = 0;
            targetObject.transform.position = waitTarget.transform.position;
            targetObject.transform.LookAt(player.transform);
            snakeHead.OpenMouth(0);
            /*
            if (t > waitTime) {
                t = 0;
                if (status == Status.ALIVE) {
                    state = State.ATTACKING;
                    attackTarget.transform.position = player.transform.position;
                }
            }*/
        }
        else if (state == State.ATTACKING) {
            targetObject.transform.position = Vector3.Lerp(waitTarget.transform.position, 
                                                           attackTarget.transform.position, 
                                                           t / attackTime);
            targetObject.transform.LookAt(attackTarget.transform);
            snakeHead.OpenMouth(t / attackTime);
            if (t > attackTime) {
                t = 0;
                state = State.RETREATING;
            }
        }
        else if (state == State.RETREATING) {
            targetObject.transform.position = Vector3.Lerp(attackTarget.transform.position,
                                                           waitTarget.transform.position,
                                                           t / retreatTime);
            Quaternion attackRot = Quaternion.LookRotation(attackTarget.transform.position - targetObject.transform.position);
            Quaternion playerRot = Quaternion.LookRotation(player.transform.position - targetObject.transform.position);
            targetObject.transform.rotation = Quaternion.Slerp(attackRot, playerRot, t / retreatTime);
            snakeHead.OpenMouth(1 - t / retreatTime);
            if (t > retreatTime) {
                t = 0;
                if (status == Status.CUT) {
                    state = State.REGENWAIT;
                } else {
                    state = State.WAITING;
                }
            }
        }
        else if (state == State.REGENWAIT || state == State.REGENSEGMENTS || state == State.REGENHEAD) {
            targetObject.transform.position = waitTarget.transform.position;
            targetObject.transform.LookAt(player.transform);
            if (state == State.REGENWAIT) {
                if (t > regenWaitTime) {
                    t = 0;
                    GameObject cutObj = segments[cut].transform.parent.gameObject;
                    for (int i = cut; i < numSegments; i++) {
                        segments[i].transform.parent = transform;
                        segments[i].SetActive(false);
                    }
                    head.SetActive(false);
                    Destroy(cutObj);
                    state = State.REGENSEGMENTS;
                }
            } else if (state == State.REGENSEGMENTS) {
                segments[cut].SetActive(true);
                ss.SetSegment(segments, cut);
                Vector3 targetPos = segments[cut].transform.position;
                Quaternion targetRot = segments[cut].transform.rotation;
                Transform prvSeg = cut > 0 ? segments[cut - 1].transform : transform;
                Vector3 prvSegPos = prvSeg.position;
                Quaternion prvSegRot = prvSeg.rotation;
                segments[cut].transform.position = Vector3.Lerp(prvSegPos, targetPos, t / regenSegmentTime);
                segments[cut].transform.rotation = Quaternion.Slerp(prvSegRot, targetRot, t / regenSegmentTime);
                SetStumps(segments[cut].transform, null);
                if (t > regenSegmentTime) {
                    t = 0;
                    cut++;
                    if (cut == numSegments) {
                        state = State.REGENHEAD;
                    }
                }
            } else if (state == State.REGENHEAD) {
                head.SetActive(true);
                float scale = Mathf.Lerp(0, 1, t / regenHeadTime);
                head.transform.localScale = new Vector3(scale, scale, scale);
                if (t > regenHeadTime) {
                    t = 0;
                    state = State.WAITING;
                    status = Status.ALIVE;
                    SetStumps(null, null);
                }
            }
        }
    }

    public void Attack() {
        if (status == Status.ALIVE) {
            t = 0;
            state = State.ATTACKING;
            attackTarget.transform.position = player.transform.position;
            snakeHead.Scream();
        }
    }

    public void Cut(int i) {
        if ((state != State.ATTACKING && state != State.RETREATING) ||
            status != Status.ALIVE) return;

        status = Status.CUT;
        cut = i;
        SetStumps(cut > 0 ? segments[cut-1].transform : transform, segments[cut].transform);

        GameObject cutObj = new GameObject("Cut Object");
        cutObj.transform.position = segments[i].transform.position;
        Rigidbody rb = cutObj.AddComponent<Rigidbody>();

        for (int j = cut; j < numSegments; j++) {
            segments[j].transform.parent = cutObj.transform;
        }

        rb.velocity = 10 * segments[i].transform.forward;
    }

    public void Burn() {
        if ((state != State.ATTACKING && state != State.RETREATING) ||
            status != Status.CUT) return;

        status = Status.DEAD;
    }

    void SetStumps(Transform par1, Transform par2) {
        if (par1 != null) {
            stump1.transform.parent = par1; //cut > 0 ? segments[cut - 1].transform : transform;
            stump1.transform.localPosition = segmentLength * Vector3.forward;
            stump1.transform.localRotation = Quaternion.identity;
            stump1.SetActive(true);
        }
        else stump1.SetActive(false);

        if (par2 != null) {
            stump2.transform.parent = par2; //segments[cut].transform;
            stump2.transform.localPosition = -segmentLength * Vector3.forward;
            stump2.transform.localRotation = Quaternion.Euler(0, 180, 0);
            stump2.SetActive(true);
        }
        else stump2.SetActive(false);
    }
}
