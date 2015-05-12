using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{
	public MeshRenderer footRenderer;
    public Transform cardboardHead;
    public LineRenderer laser;
    public PIDController controller;

    private float timeSinceLastStep = 5f;

    public float maxForwardVelocity = 10;
    public float minForwardVelocity = 1;

    public float curVel = 0;

    public Transform gazePointer;

	public bool movementEnabled = true;
	public bool useSteps = true;

	public bool movementToggled = false;
	public bool lookDownToggle = false;

	// Use this for initialization
	void Start ()
	{
	    Input.compass.enabled = true;
        
        StepDetector.OnStepDetected += OnStepDetected;

		if (!movementEnabled)
		{
			controller.enabled = false;
		}
		else
		{
			controller.enabled = true;
		}
	}

    void Update()
    {
        //Debug.Log(Input.compass.rawVector.magnitude);
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;

        /*
        if(!magnetCalibrated && Vector3.Dot(cardboardHead.transform.forward, Vector3.down) > .95)
        {
            magnetThreshold = Input.compass.rawVector.magnitude - 100;
        }
         */

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnStepDetected();
        }

	    if (useSteps)
	    {
		    footRenderer.enabled = false;
	    }
	    else
	    {
			footRenderer.enabled = true;
	    }
    }


    void FixedUpdate()
    {
	    if (useSteps)
	    {
		    timeSinceLastStep += Time.fixedDeltaTime;

		    if (timeSinceLastStep > 1)
		    {
			    //controller.targetVelocity.Set(0, 0, 0);
		    }

		    if (curVel > 0)
			    curVel *= .95f;
		    if (curVel < 0)
			    curVel = 0;
		    controller.targetVelocity.Set(0, 0, curVel);
	    }
	    else
	    {
		    if (Vector3.Dot(cardboardHead.transform.forward, Vector3.down) > .95)
		    {
			    if (!lookDownToggle)
			    {
				    lookDownToggle = true;
				    movementToggled = !movementToggled;
			    }
		    }
		    else
		    {
			    lookDownToggle = false;
		    }

		    if (movementToggled)
		    {
			    curVel = 5;
				controller.targetVelocity.Set(0, 0, curVel);
			    footRenderer.material.SetColor("_EmisColor", Color.green);
		    }
		    else
		    {
			    curVel *= .95f;
				controller.targetVelocity.Set(0, 0, curVel);
				footRenderer.material.SetColor("_EmisColor", Color.red);
		    }
	    }

    }

    void OnStepDetected()
    {
	    if (useSteps && movementEnabled)
	    {
		    //Debug.Log("Time since last step: " + timeSinceLastStep);
		    if (timeSinceLastStep > 1)
			    curVel = minForwardVelocity;
		    else if (timeSinceLastStep < .4)
			    curVel = maxForwardVelocity;
		    else
		    {
			    float slope = (minForwardVelocity - maxForwardVelocity)/.6f;
			    float yintercept = -slope + minForwardVelocity;
			    //Debug.Log("Slope: " + slope + " Yint: " + yintercept);
			    curVel = timeSinceLastStep*slope + yintercept;
		    }


		    controller.targetVelocity.Set(0, 0, curVel);

		    timeSinceLastStep = 0;
	    }
    }

	public void EnableMovement()
	{
		movementEnabled = true;
		controller.enabled = true;
	}
}
