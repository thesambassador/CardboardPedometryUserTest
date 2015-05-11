using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StepCounter : MonoBehaviour
{

    private int steps = 0;
    public Text stepText;

	// Use this for initialization
	void Start ()
	{
	    StepDetector.OnStepDetected += OnStepDetected;
	}
	
	// Update is called once per frame
	void Update () {
	    if (stepText != null)
	    {
	        stepText.text = steps.ToString();
	    }
	}

    private void OnStepDetected()
    {
        steps ++;
    }
}
