using UnityEngine;
using System.Collections;

public class Tunnel : MonoBehaviour {
    public const int TUNNEL_PIECE_LENGTH = 10;

    public PlayerControl playerControl;
    public GameObject TunnelPiecePrefab;
    public int NumPieces;
    public int Velocity;
    public GameObject[] Obstacles;
    public float ObstacleSpawnProbability = 0.4f;
    public float timeToWin = 30;
    public float timeToIdle = 5;


    private GameObject[] TunnelPieces;
    private int curIdx = 0;
    private float startTime;

    private string gameState;

	// Use this for initialization
	void Start () {
        TunnelPieces = new GameObject[NumPieces];
        for (int i = 0; i < NumPieces; i++) {
            Vector3 pos = transform.position - Vector3.forward * TUNNEL_PIECE_LENGTH * i;
            TunnelPieces[i] = (GameObject) Instantiate(TunnelPiecePrefab, pos, Quaternion.identity);
        }
        startTime = Time.time;
        gameState = "RUNNING";
	}
	
	// Update is called once per frame
	void Update () {
        if (gameState.Equals("RUNNING")) {
            // Check for win
            if (Time.time > startTime + timeToWin) {
                gameState = "IDLE";
                startTime = Time.time;
                print("You won!");
            }
        } else if (gameState.Equals("IDLE")) {
            if (Time.time > startTime + timeToIdle) {
                gameState = "FINISHED";
                print("Leaving game");
                playerControl.Die();
            }
        }
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

        if (gameState.Equals("RUNNING") && Random.value < ObstacleSpawnProbability) {
            GameObject obsType = Obstacles[Random.Range(0, Obstacles.Length)];
            Instantiate(obsType, pos + obsType.transform.position, obsType.transform.rotation, TunnelPieces[idx].transform);
        }
    }
}
