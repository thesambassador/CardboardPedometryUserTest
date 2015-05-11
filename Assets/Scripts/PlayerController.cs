using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{

    public Transform cardboardHead;
    public LineRenderer laser;
    public PIDController controller;

    private Rigidbody rigidbody;

    private float timeSinceLastStep = 5f;

    public float maxForwardVelocity = 8;
    public float minForwardVelocity = 1;

    private float curVel = 0;

    public Transform gazePointer;

    private float laserLength = 0;
    private float laserSpeed = 120f;

    private bool magnetCalibrated = false;

    private float magnetThreshold = 2400;

	// Use this for initialization
	void Start ()
	{
	    Input.compass.enabled = true;
        
        StepDetector.OnStepDetected += OnStepDetected;
	    rigidbody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        //Debug.Log(Input.compass.rawVector.magnitude);
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;

        if(!magnetCalibrated && Vector3.Dot(cardboardHead.transform.forward, Vector3.down) > .95)
        {
            magnetThreshold = Input.compass.rawVector.magnitude - 100;
        }
    }


    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnStepDetected();
        }
        timeSinceLastStep += Time.fixedDeltaTime;

        if (timeSinceLastStep > 1)
        {
            controller.targetVelocity.Set(0, 0, 0);
        }
        else
        {
            if(curVel > 0)
                curVel -= .1f;
            if (curVel < 0)
                curVel = 0;
            controller.targetVelocity.Set(0, 0, curVel);
        }

        

        
    }

    void OnStepDetected()
    {
        //Debug.Log("Time since last step: " + timeSinceLastStep);
        if (timeSinceLastStep > 1)
            curVel = minForwardVelocity;
        else if (timeSinceLastStep < .2)
            curVel = maxForwardVelocity;
        else
        {
            float slope = (minForwardVelocity - maxForwardVelocity)/.8f;
            float yintercept = -slope + minForwardVelocity;
            //Debug.Log("Slope: " + slope + " Yint: " + yintercept);
            curVel = timeSinceLastStep * slope + yintercept;
        }


        controller.targetVelocity.Set(0, 0, curVel);

        timeSinceLastStep = 0;
    }
}
