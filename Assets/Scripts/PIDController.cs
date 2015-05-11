using UnityEngine;
using System.Collections;

public class PIDController : MonoBehaviour {
	
	private Vector3 integral;
	private Vector3 prev_error;
	
	public float cP = 30;
	public float cI = .5f;
	public float cD = 0;
	
	public Vector3 axisEnabled;
	
	public float runForce = 40;
	
	public Vector3 minForce;
    public Vector3 maxForce;
	
	public Vector3 targetVelocity;
	
	public bool on = false;
	public bool local = false;

    private Rigidbody _rigidbody;
	
	// Use this for initialization
	void Start ()
	{
	    _rigidbody = GetComponent<Rigidbody>();
		integral = new Vector3();
		prev_error = new Vector3();
	}

	
	void FixedUpdate() {
		if(on){
			applyController();
		}
	}
	
	public void setTargetVelocity(Vector3 target, float maxF, bool shouldSlow){
		targetVelocity = target;
		
		minForce = new Vector3();
		maxForce = new Vector3();
		
		minForce.x = -maxF;
		minForce.z = -maxF;
		maxForce.x = maxF;
		maxForce.z = maxF;
		
		if(!shouldSlow){
			if(target.x > 0){
				minForce.x = 0;	
			}
			else{
				maxForce.x = 0;	
			}
			
			if(target.z > 0){
				minForce.z = 0;	
			}
			else{
				maxForce.z = 0;	
			}
		}
		
	}
	
	public void applyController(){
		Vector3 speed;

	    if (local)
	    {
            speed = transform.InverseTransformDirection(_rigidbody.velocity);
	    }
	    else
	    {
            speed = this.GetComponent<Rigidbody>().velocity;
	    }
		Vector3 error = targetVelocity - speed;
		this.integral += error;
		Vector3 derivative = error - this.prev_error;
		this.prev_error = error;
		
		Vector3 action = error * this.cP + this.integral * this.cI + derivative * this.cD;
		
		for(int i=0; i<3; i++){
			if(action[i] < this.minForce[i])
				action[i] = this.minForce[i];
			if(action[i] > this.maxForce[i])
				action[i] = this.maxForce[i];
		}
		
		if(local)
			_rigidbody.AddRelativeForce(action);
		else
            _rigidbody.AddForce(action);
		
		//Debug.Log(action);
		//Debug.Log(speed);
		
		
	}
	
}
