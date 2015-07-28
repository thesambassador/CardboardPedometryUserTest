using UnityEngine;
using System.Collections;

public class StayInView : MonoBehaviour {

    public Transform player;

    public float initialDist;

	// Use this for initialization
	void Start () {
        initialDist = (transform.position - player.position).magnitude;
	}
	
	// Update is called once per frame
	void LateUpdate () {



        Vector3 newPos = player.forward * initialDist;
        newPos.y = transform.position.y;

        transform.position = newPos;

        Vector3 sameHeight = player.position;
        sameHeight.y = transform.position.y;

        sameHeight = sameHeight + (transform.position - sameHeight) * 2;

        transform.LookAt(sameHeight);
        
	}
}
