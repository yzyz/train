using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGD : MonoBehaviour {

    public GameObject segmentPrefab;

    public GameObject targetObject;

    public float segmentLength = 0.05f;
    public int numSegments = 100;

    public float lambdaTan = 0.0001f;
    public float lambda = 0.0005f;

    private GameObject[] segments;
    private Vector3[] u, x, r, g;
    private Quaternion[] q;

	// Use this for initialization
	void Start () {
        segments = new GameObject[numSegments];
        for (int i = 0; i < numSegments; i++) {
            Transform parent = i > 0 ? segments[i - 1].transform : transform;
            segments[i] = Instantiate(segmentPrefab, parent.position + segmentLength * parent.forward,
                                      parent.rotation, parent);
        }

        u = new Vector3[numSegments + 1];
        x = new Vector3[numSegments + 1];
        r = new Vector3[numSegments];
        g = new Vector3[numSegments];
        q = new Quaternion[numSegments + 1];
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < numSegments; i++) segments[i].transform.Rotate(new Vector3(0.01f, 0, 0));
        Vector3 targetPos = transform.InverseTransformPoint(targetObject.transform.position);
        Quaternion targetRot = Quaternion.Inverse(transform.rotation) * targetObject.transform.rotation;
        GradientDescent(targetPos, targetRot);
        //GradientDescent(targetObject.transform.position - transform.position);
        for (int i = 0; i < numSegments; i++) {
            segments[i].transform.localEulerAngles = r[i];
        }
        //print("true distance:" + (targetObject.transform.position - segments[numSegments - 1].transform.position));
        CalculateForwardPass();
        print(Time.realtimeSinceStartup + " " + (transform.TransformPoint(x[numSegments])) + " " + segments[numSegments-1].transform.position);
        print((transform.rotation * q[numSegments]).eulerAngles + " " + segments[numSegments - 1].transform.eulerAngles);
    }

    void CalculateForwardPass() {
        u[0] = segmentLength * Vector3.forward;
        q[0] = Quaternion.identity;
        x[0] = Vector3.zero;
        for (int i = 0; i < numSegments; i++) {
            //u[i + 1] = Quaternion.Euler(r[i]) * u[i];
            q[i + 1] = q[i] * Quaternion.Euler(r[i]);
            u[i + 1] = q[i + 1] * u[0];
            x[i + 1] = x[i] + u[i + 1];
        }
    }
    
    void GradientDescent(Vector3 targetPos, Quaternion targetRot, int iterations=100, float eps=1e-2f) {
        for (int it = 0; it < iterations; it++) {
            CalculateForwardPass();
            CalculateGradientNumerically(targetPos, targetRot);
            for (int i = 0; i < numSegments; i++) {
                r[i] -= eps * g[i];
            }
        }
        //print(Time.realtimeSinceStartup + " gradient descent: " + (target - x[numSegments]));
    }

    // minimize |x[n] - target|^2 + lambdaTan * angle^2 + lambda * sum{|r|^2}
    void CalculateGradientNumerically(Vector3 targetPos, Quaternion targetRot, float eps=1e-3f) {
        int n = numSegments;

        float loss = Vector3.SqrMagnitude(targetPos - x[n]);
        float angle = Quaternion.Angle(targetRot, q[n]);
        loss += lambdaTan * Mathf.Pow(angle, 2);

        for (int i = 0; i < n; i++) {
            Vector3 v = x[n] - x[i];
            Quaternion rx = Quaternion.Inverse(q[i + 1]) * Quaternion.Euler(new Vector3(eps, 0, 0)) * q[i + 1];
            Quaternion ry = Quaternion.Inverse(q[i + 1]) * Quaternion.Euler(new Vector3(0, eps, 0)) * q[i + 1];
            Quaternion rz = Quaternion.Inverse(q[i + 1]) * Quaternion.Euler(new Vector3(0, 0, eps)) * q[i + 1];
            Vector3 xx = rx * v + x[i];
            Vector3 xy = ry * v + x[i];
            Vector3 xz = rz * v + x[i];
            float anglex = Quaternion.Angle(targetRot, q[n] * rx);
            float angley = Quaternion.Angle(targetRot, q[n] * ry);
            float anglez = Quaternion.Angle(targetRot, q[n] * rz);
            float lossx = Vector3.SqrMagnitude(targetPos - xx) + lambdaTan * Mathf.Pow(anglex, 2);
            float lossy = Vector3.SqrMagnitude(targetPos - xy) + lambdaTan * Mathf.Pow(angley, 2);
            float lossz = Vector3.SqrMagnitude(targetPos - xz) + lambdaTan * Mathf.Pow(anglez, 2);
            g[i].x = (lossx - loss) / eps + lambda * 2 * r[i].x;
            g[i].y = (lossy - loss) / eps + lambda * 2 * r[i].y;
            g[i].z = (lossz - loss) / eps + lambda * 2 * r[i].z;
        }
    }
}
