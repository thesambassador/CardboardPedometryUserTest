using UnityEngine;
using System.Collections;

public class emptyGizmo : MonoBehaviour {

	// Use this for initialization
	void OnDrawGizmos ()
	{
	    Gizmos.color = Color.green;
	    Gizmos.DrawSphere(transform.position, .5f);
	}
	

}
