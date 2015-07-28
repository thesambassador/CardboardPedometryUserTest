using UnityEngine;
using System.Collections;

public class StartExperimentGazeButton : GazeButton
{

	override public void ButtonEvent()
	{
		this.GetComponentInParent<Canvas>().enabled = false;
		TestRunner.StartExperiment();
	}
}
