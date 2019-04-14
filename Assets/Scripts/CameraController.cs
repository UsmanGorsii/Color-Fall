using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
    public PlayerController player;
    private Vector3 lastPlayerPosition;
    private float distanceToMove;
    public float sizeIncreasingSpeed;
    Camera self;
	
    // Use this for initialization
    void Start () {
        self = GetComponent<Camera>();
        lastPlayerPosition = player.transform.position;
    }
	
    // Update is called once per frame
    void Update () {
        // if(player.transform.position.y > 5 && self.orthographicSize < 8)
        // {
        //     self.orthographicSize += Time.deltaTime * sizeIncreasingSpeed;
        // }
        // else if(player.transform.position.y <= 5 && self.orthographicSize > 5)
        // {
        //     self.orthographicSize -= Time.deltaTime * sizeIncreasingSpeed;
        // }

        distanceToMove = player.transform.position.x - lastPlayerPosition.x;
        transform.position = new Vector3(transform.position.x + distanceToMove, transform.position.y, transform.position.z);
        lastPlayerPosition = player.transform.position;
    }
}