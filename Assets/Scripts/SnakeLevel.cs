using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeLevel : MonoBehaviour {

    public float minTimeBetweenAttacks = 2;
    public float maxTimeBetweenAttacks = 8;

    public float waitTime = 1;
    public float attackTime = 0.3f;
    public float pausedTime = 0.1f;
    public float retreatTime = 0.3f;
    public float regenWaitTime = 3f;
    public float regenSegmentTime = 0.1f;
    public float regenHeadTime = 1f;

}
