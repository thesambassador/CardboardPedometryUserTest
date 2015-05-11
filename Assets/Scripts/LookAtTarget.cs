using UnityEngine;
using System.Collections;

public class LookAtTarget : MonoBehaviour
{

    public Transform target;
    public Transform pupilRotationObject;

    public float angle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	    if (target != null && pupilRotationObject != null)
	    {
	        Vector3 towards = target.position - transform.position;

	        //towards.y = 0;

	        towards.Normalize();

	        angle = Vector3.Dot(towards, transform.forward);

	        if (angle <= 1 && angle >= .88)
	        {
	            pupilRotationObject.LookAt(target);
	        }
	        if (angle <= .98)
	        {
	            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(towards, Vector3.up), .1f);
	        }
	    }

	}
}
