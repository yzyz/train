using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraHead : MonoBehaviour {

    public enum State {
        WAITING, ATTACKING, PAUSED, RETREATING, REGENWAIT, REGENSEGMENTS, REGENHEAD
    }

    public enum Status {
        DEAD, CUT, ALIVE
    }

	public GameObject swordController;
	public GameObject torchController;

    public GameObject segmentPrefab;
    public GameObject headPrefab;
    public GameObject stumpPrefab;

    public GameObject targetObject;
    public GameObject waitTarget;
    public GameObject attackTarget;
    public GameObject player;
    public Hydra hydra;

    public int id;

    public float segmentLength = 0.05f;
    public int numSegments = 100;
    public float tangentWeight = 5f;
    public float hapticLength = .2f;

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
    public static float pausedTime = 0.1f;
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
            float param = EaseAttack(t / attackTime);
            targetObject.transform.position = Vector3.LerpUnclamped(waitTarget.transform.position, 
                                                                    attackTarget.transform.position, 
                                                                    param);
            Quaternion attackRot = Quaternion.LookRotation(attackTarget.transform.position - waitTarget.transform.position);
            targetObject.transform.rotation = attackRot;
            snakeHead.OpenMouth(t / attackTime);
            if (t > attackTime) {
                t = 0;
                state = State.PAUSED;
            }
        }
        else if (state == State.PAUSED) {
            if (t > pausedTime) {
                t = 0;
                state = State.RETREATING;
            }
        }
        else if (state == State.RETREATING) {
            float param = EaseAttack(1 - t / retreatTime);
            targetObject.transform.position = Vector3.LerpUnclamped(waitTarget.transform.position,
                                                                    attackTarget.transform.position,
                                                                    param);
            Quaternion attackRot = Quaternion.LookRotation(attackTarget.transform.position - waitTarget.transform.position);
            Quaternion playerRot = Quaternion.LookRotation(player.transform.position - targetObject.transform.position);
            targetObject.transform.rotation = Quaternion.Slerp(playerRot, attackRot, param);
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

    float EaseAttack(float t) {
        if (t < 0) t = 0;
        if (t > 1) t = 1;
        float t2 = t * t;
        float t3 = t2 * t;
        float t4 = t3 * t;
        float t5 = t4 * t;
        float t6 = t5 * t;
        float t7 = t6 * t;
        float t8 = t7 * t;
        return -4.8750518473098398e-001f * t
               + 4.5264070188173147e+000f * t2
               - 4.0821755908042114e+001f * t3
               + 1.8034722422285620e+002f * t4
               - 4.2097506070128566e+002f * t5
               + 5.1175138840736878e+002f * t6
               - 2.9101315616237736e+002f * t7
               + 5.7872458307064427e+001f * t8;
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

		stump1.GetComponent<SnakeStump>().PlaySound("SLASH");
		int index = (int) swordController.GetComponent<SteamVR_TrackedObject> ().index;
        StartCoroutine(RunHaptics(index, Time.time));
    }

    public void Burn() {
        if ((state != State.ATTACKING && state != State.PAUSED && state != State.RETREATING) ||
            status != Status.CUT) return;
		
		int index = (int) torchController.GetComponent<SteamVR_TrackedObject> ().index;
        StartCoroutine(RunHaptics(index, Time.time));
		stump1.GetComponent<SnakeStump>().PlaySound("BURN");
        stump1.transform.FindChild("BloodSprayEffect").gameObject.SetActive(false);
        stump1.transform.FindChild("BloodStreamEffect").gameObject.SetActive(false);
        stump1.transform.FindChild("FlamesParticleEffect").gameObject.SetActive(true);

        status = Status.DEAD;
        hydra.LoseHead();
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

    IEnumerator RunHaptics(int index, float startTime) {
        while (Time.time < startTime + hapticLength) {
            SteamVR_Controller.Input(index).TriggerHapticPulse(400);
            yield return null;
        }
    }
}
