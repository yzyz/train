using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpline {

    public GameObject targetObject;

    public float segmentLength = 0.05f;
    public int numSegments = 100;

    public float tangentWeight = 5f;

    private HydraHead hh;
    private float[] t;
    private Vector3[] x;
    private Quaternion[] r;
    
    public SnakeSpline(HydraHead h) {
        hh = h;
        targetObject = hh.targetObject;
        segmentLength = hh.segmentLength;
        numSegments = hh.numSegments;
        tangentWeight = hh.tangentWeight;
        t = new float[numSegments + 1];
        x = new Vector3[numSegments + 1];
        r = new Quaternion[numSegments];
    }
    
    public void Update() {
        t[numSegments] = 1.0f;
        x[numSegments] = targetObject.transform.position;

        for (int i = numSegments-1; i >= 0; i--) {
            // find t[i] such that Dist(x[i], x[i+1]) = segmentLength
            float lo = 0, hi = t[i + 1];
            for (int it = 0; it < 20; it++) {
                t[i] = (lo + hi) / 2;
                x[i] = GetHermitePoint(t[i]);
                if (Vector3.Distance(x[i], x[i+1]) > segmentLength) lo = t[i];
                else hi = t[i];
            }
            t[i] = (lo + hi) / 2;
            x[i] = GetHermitePoint(t[i]);
        }
        
        // calculate rotations
        for (int i = 0; i < numSegments; i++) {
            if (x[i + 1] == x[i]) {
                r[i] = hh.transform.rotation;
            }
            else {
                Vector3 up = hh.transform.up;
                if (i > 0) {
                    up = r[i - 1] * up;
                }
                r[i] = Quaternion.LookRotation(x[i + 1] - x[i], up);
            }
        }
        // now to fix final z-rotation...
        // first we calculate how much we need to rotate the head around its local z-axis so it is upright
        Vector3 curRight = r[numSegments - 1] * Vector3.right;
        Vector3 curForward = r[numSegments - 1] * Vector3.forward;
        Vector3 desRight = Vector3.Cross(Vector3.up, curForward); // left-hand rule
        float angle;
        Vector3 axis;
        Quaternion.FromToRotation(curRight, desRight).ToAngleAxis(out angle, out axis);
        if (Vector3.Dot(curForward, axis) < 0) {
            angle = -angle;
        }
        //MonoBehaviour.print(angle);
        // now each segment rotates a fraction of the angle more until the final head is correctly rotated
        for (int i = 0; i < numSegments; i++) {
            r[i] *= Quaternion.AngleAxis(angle * (i + 1) / numSegments, Vector3.forward);
        }
    }

    public void SetSegment(GameObject[] segments, int i) {
        segments[i].transform.position = (x[i] + x[i + 1]) / 2;
        segments[i].transform.rotation = r[i];
    }

    Vector3 GetHermitePoint(float t) {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 p0 = hh.transform.position;
        Vector3 m0 = tangentWeight * hh.transform.forward;
        Vector3 p1 = targetObject.transform.position;
        Vector3 m1 = tangentWeight * targetObject.transform.forward;

        float h00 = 2 * t3 - 3 * t2 + 1;
        float h10 = t3 - 2 * t2 + t;
        float h01 = -2 * t3 + 3 * t2;
        float h11 = t3 - t2;

        return h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
    }
}
