using UnityEngine;
using System.Collections;

public class Tunnel : MonoBehaviour {
    public const int TUNNEL_PIECE_LENGTH = 10;

    public GameObject TunnelPiecePrefab;
    public int NumPieces;
    public int Velocity;
    public GameObject[] Obstacles;
    public float ObstacleSpawnProbability = 0.4f;

    private GameObject[] TunnelPieces;
    private int curIdx = 0;

	// Use this for initialization
	void Start () {
        TunnelPieces = new GameObject[NumPieces];
        for (int i = 0; i < NumPieces; i++) {
            Vector3 pos = transform.position - Vector3.forward * TUNNEL_PIECE_LENGTH * i;
            TunnelPieces[i] = (GameObject) Instantiate(TunnelPiecePrefab, pos, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < NumPieces; i++) {
            TunnelPieces[i].transform.position -= Velocity * Time.deltaTime * Vector3.forward;
        }
        Vector3 curPos = TunnelPieces[curIdx].transform.position;
        if (Vector3.Distance(curPos, transform.position) > TUNNEL_PIECE_LENGTH) {
            int nxtIdx = curIdx > 0 ? curIdx - 1 : NumPieces - 1;
            Respawn(nxtIdx, curPos + Vector3.forward * TUNNEL_PIECE_LENGTH);
            curIdx = nxtIdx;
        }
	}

    void Respawn(int idx, Vector3 pos) {
        TunnelPieces[idx].transform.position = pos;
        foreach (Transform child in TunnelPieces[idx].transform) {
            if (child.gameObject.CompareTag("Obstacle")) {
                Destroy(child.gameObject);
            }
        }

        if (Random.value < ObstacleSpawnProbability) {
            GameObject obsType = Obstacles[Random.Range(0, Obstacles.Length)];
            Instantiate(obsType, pos + obsType.transform.position, obsType.transform.rotation, TunnelPieces[idx].transform);
        }
    }
}
