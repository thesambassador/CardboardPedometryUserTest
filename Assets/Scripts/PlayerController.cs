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

    public float maxTimeBetweenSteps = .7f;

    public float curVel = 0;

    public Transform gazePointer;

	public bool movementEnabled = true;
	public bool useSteps = true;

	public bool movementToggled = false; //whether or not movement is currently toggled/happening
	public bool lookDownToggle = false; //stays true if looking down, otherwise is false

	public AudioSource footstepSound;
   
    private Rigidbody _rigidbody;

    private float _lookDownStepSoundTimer = 0;
    private float _lookDownStepSoundTime = .3f;

	// Use this for initialization
	void Start ()
	{
	    Input.compass.enabled = true;
        
        StepDetector.instance.OnStepDetected += OnStepDetected;

		if (!movementEnabled)
		{
			controller.enabled = false;
		}
		else
		{
			controller.enabled = true;
		}

        _rigidbody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        //Align player's left/right with the cardboard left/right
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;

        //enable the foot icon depending on whether useSteps is true or false
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
            WalkInPlaceFixedUpdate();
	    }
	    else
	    {
            LookDownFixedUpdate();
	    }
        if (movementEnabled)
        {
            Debug.Log(_rigidbody.velocity);
            Vector3 newVel = transform.forward * curVel;
            newVel.y = _rigidbody.velocity.y;
            _rigidbody.velocity = newVel;
        }
        else
        {
            Vector3 newVel = Vector3.zero;
            newVel.y = _rigidbody.velocity.y;
            _rigidbody.velocity = newVel;
        }

        
        

    }

    void LookDownFixedUpdate()
    {
        //if they're looking down, toggle the icon and movement
        if (Vector3.Dot(cardboardHead.transform.forward, Vector3.down) > .89)
        {
            if (!lookDownToggle)
            {
                lookDownToggle = true;
                movementToggled = !movementToggled;
                _lookDownStepSoundTimer = _lookDownStepSoundTime;
            }
        }
        else
        {
            lookDownToggle = false;
        }

        //move and set icon to green
        if (movementToggled)
        {
            curVel = maxForwardVelocity;
            //controller.targetVelocity.Set(0, 0, curVel);
            footRenderer.material.SetColor("_EmisColor", Color.green);
        }
        //dampen movement and set icon to red
        else
        {
            curVel *= .8f;
            //controller.targetVelocity.Set(0, 0, curVel);
            footRenderer.material.SetColor("_EmisColor", Color.red);
        }

        //play sound periodically for look down to move
        if (movementToggled)
        {
            _lookDownStepSoundTimer -= Time.fixedDeltaTime;
            if (_lookDownStepSoundTimer <= 0)
            {
                _lookDownStepSoundTimer = _lookDownStepSoundTime;
                footstepSound.Play();
            }
        }
    }

    void WalkInPlaceFixedUpdate()
    {
        timeSinceLastStep += Time.fixedDeltaTime;

        if (timeSinceLastStep > 1)
        {
            curVel = 0;
        }


        if (curVel > 0)
        {
            if (timeSinceLastStep < maxTimeBetweenSteps)
                curVel *= 1;
            else
                curVel *= .8f;
        }
        if (curVel < 0)
            curVel = 0;
        //controller.targetVelocity.Set(0, 0, curVel);
    }

    void OnStepDetected()
    {
		
	    if (useSteps && movementEnabled)
	    {
			footstepSound.Play();

		    if (timeSinceLastStep > .5)
			    curVel = minForwardVelocity;
		    else
                curVel = maxForwardVelocity;

		    /* for experiment
            else
		    {
			    float slope = (minForwardVelocity - maxForwardVelocity)/.6f;
			    float yintercept = -slope + minForwardVelocity;
			    //Debug.Log("Slope: " + slope + " Yint: " + yintercept);
			    curVel = timeSinceLastStep*slope + yintercept;
		    }
            */

		    //controller.targetVelocity.Set(0, 0, curVel);

		    timeSinceLastStep = 0;
			//Debug.Log("Step");
	    }
    }

	public void EnableMovement()
	{
		movementEnabled = true;
		controller.enabled = true;
	}

    public void OnCollisionEnter(Collision col)
    {
        //controller.targetVelocity = _rigidbody.velocity;
        if (col.collider.tag == "Obstacle")
        {
            Debug.Log("Ran into obstacle");
        }
    }
}
