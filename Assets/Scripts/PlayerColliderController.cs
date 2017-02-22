using UnityEngine;
using System.Collections;

public class PlayerColliderController : MonoBehaviour {

    public Vector3 HeadDimensions = new Vector3(0.15f, 0.25f, 0.15f);
    public AudioClip ScreamClip;
    public AudioClip HitClip;
    public float volume = 0.5f;

    private Rigidbody rb;
    private BoxCollider col;
    private AudioSource scream;
    private AudioSource hit;
    private DamageFlashController flash;

    // Use this for initialization
    void Start () {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        col = gameObject.AddComponent<BoxCollider>();
        col.size = HeadDimensions;
        col.isTrigger = true;

        scream = gameObject.AddComponent<AudioSource>();
        scream.clip = ScreamClip;
        scream.volume = volume;
        hit = gameObject.AddComponent<AudioSource>();
        hit.clip = HitClip;
        hit.volume = volume;

        flash = GetComponent<DamageFlashController>();
	}
	
	void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            print("I've been hit!");
            scream.Play();
            hit.Play();
            flash.Flash();

            /*
            col.isTrigger = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(1000 * Vector3.back);

            other.isTrigger = false;
            if (other is BoxCollider) {
                BoxCollider o = (BoxCollider)other;
                Vector3 tmp = o.size;
                tmp.z *= 21;
                o.size = tmp;
                tmp = o.center;
                tmp.z += 10;
                o.center = tmp;
            }
            //other.attachedRigidbody.isKinematic = false;
            */
        }
    }
}
