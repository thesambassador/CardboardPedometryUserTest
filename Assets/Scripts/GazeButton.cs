using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GazeButton : MonoBehaviour
{

	public float gazeTime = 1.5f;
	public float curGazeTime = 0f;

	private bool gaze = false;

	public RectTransform scaleTransform;

	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update ()
	{
		if (gaze)
			curGazeTime += Time.deltaTime;
		else
		{
			if(curGazeTime > 0)
				curGazeTime -= 2*Time.deltaTime;
		}

		float scale = curGazeTime/gazeTime * 125;
		if (scale > 125) scale = 125;
		scaleTransform.sizeDelta = new Vector2(scale, scale);


		if (curGazeTime >= gazeTime)
		{
			ButtonEvent();
		}

	}

	public virtual void ButtonEvent()
	{
		Debug.Log("CLICKED");
	}

	public void OnGazeEnter()
	{
		gaze = true;
	}
	
	public void OnGazeExit()
	{
		gaze = false;
		//curGazeTime = 0;
	}
}
