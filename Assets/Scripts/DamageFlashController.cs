using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashController : MonoBehaviour {

    public float MaxAlpha = 0.4f;
    public float HalfLife = 0.1f;
    
    private Image img;

	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        Color col = img.color;
        col.a *= Mathf.Pow(2f, -Time.deltaTime / HalfLife);
        img.color = col;
	}

    public void Flash() {
        Color col = img.color;
        col.a = MaxAlpha;
        img.color = col;
    }
}
