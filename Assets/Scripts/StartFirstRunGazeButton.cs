using UnityEngine;
using System.Collections;

public class StartFirstRunGazeButton : GazeButton {

	override public void ButtonEvent()
	{
		DataRecorder.StartFirstRun();
	}
}
