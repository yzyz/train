using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashController : MonoBehaviour {

    public Color color = new Color(1, 0, 0, 0);
    public float MaxAlpha = 0.4f;
    public float HalfLife = 0.1f;

    Material mat;
    int matColorID = -1;

    // Use this for initialization
    void Start () {
        mat = new Material(Shader.Find("Custom/SteamVR_Fade"));
        matColorID = Shader.PropertyToID("fadeColor");
	}

    public void Flash() {
        color.a = MaxAlpha;
    }
    
    void OnPostRender() {
        color.a *= Mathf.Pow(2f, -Time.deltaTime / HalfLife);
        mat.SetColor(matColorID, color);
        mat.SetPass(0);
        GL.Begin(GL.QUADS);

        GL.Vertex3(-1, -1, 0);
        GL.Vertex3(1, -1, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(-1, 1, 0);
        GL.End();
    }
}
