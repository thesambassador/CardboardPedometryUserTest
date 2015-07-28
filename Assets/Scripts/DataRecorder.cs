using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Xml.Schema;

public static class MyExtensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random rng = new System.Random(3);
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}

public class DataRecorder : MonoBehaviour
{

	private const string generalFilename = "participantNum.txt";
	private const string dataFilename = "_data.txt";

	public static DataRecorder instance;

	private int _participantNumber;

	public bool ranOne = false;
	public bool isCurrentlyPractice = true;

	public string currentMethod;

	private StreamWriter dataWriter;

	public Text participantNumText;

	// Use this for initialization
    void Start(){
		GameObject.DontDestroyOnLoad(this.gameObject);
	    instance = this;

	    _participantNumber = GetNextParticipantNumber();
	    participantNumText.text = _participantNumber.ToString();

		//create new file, overwrite if exists, with player number on top
		dataWriter = new StreamWriter(Application.persistentDataPath + "/" + _participantNumber.ToString() + dataFilename, false);

		dataWriter.WriteLine(_participantNumber.ToString());

		dataWriter.Close();
    }

	public static int GetNextParticipantNumber()
	{
		string filePath = Application.persistentDataPath + "/" + generalFilename;
		Debug.Log(filePath);
		//First time running, we return 1 and then write the next value into the file
		if (!File.Exists(filePath))
		{
			Debug.Log("File didn't exist, returning 1 and creating file");
			File.WriteAllText(filePath, "2");
			return 1;
		}
		else
		{
			Debug.Log("File did exist, trying to read");
			string val = File.ReadAllText(filePath);
			int participantNum;
			if (int.TryParse(val, out participantNum))
			{
				Debug.Log("Read participant number successfully, it is: " + participantNum.ToString());

				File.WriteAllText(filePath, (participantNum + 1).ToString());

				return participantNum;
			}
			else
			{
				Debug.Log("Something went wrong");
				return -1;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnLevelWasLoaded(int level)
	{
		if (level == 1)
		{
			GameObject player = GameObject.Find("Player");
			GameObject testRunner = GameObject.Find("TestRunner");

			//set current movement method
			PlayerController controller = player.GetComponent<PlayerController>();
			if (currentMethod == "step")
				controller.useSteps = true;
			else
				controller.useSteps = false;

			TestRunner runner = testRunner.GetComponent<TestRunner>();
			runner.isPractice = isCurrentlyPractice;

			//set the welcome message stuff
			Text movementMethod = GameObject.Find("text_movementMethod").GetComponent<Text>();
			Text instructions = GameObject.Find("text_instructions").GetComponent<Text>();
			Text isReal = GameObject.Find("text_isreal").GetComponent<Text>();

			if (isCurrentlyPractice) movementMethod.text = "Practice - ";
			else movementMethod.text = "";

			if (currentMethod == "step")
			{
				movementMethod.text += "Walk in Place to Move";
				instructions.text =
					"With this movement method, you move around the environment by walking in place.";
			}
			else if (currentMethod == "look")
			{
				movementMethod.text += "Look down to Move";
				instructions.text = "With this movement method, you can enable forward movement by looking down at the icon at your feet.  Look up and back down at the icon to stop moving.";
			}

			if (isCurrentlyPractice)
			{
				isReal.text = "This is practice.  Spend a bit of time getting used to moving around the environment.";
			}
			else
			{
				isReal.text = "This is now the real experiment, try to go as fast as you can.";
			}

		}
	}

	void WriteData(RunData data)
	{
		dataWriter = new StreamWriter(Application.persistentDataPath + "/" + _participantNumber.ToString() + dataFilename, true);
		dataWriter.WriteLine();
		dataWriter.WriteLine("Current method: " + currentMethod);

		dataWriter.WriteLine("Total Time:");
		dataWriter.WriteLine(data.totalTime.ToString());

		dataWriter.WriteLine("Total Distance:");
		dataWriter.WriteLine(data.totalDistanceTraveled.ToString());

		dataWriter.WriteLine("Total Steps:");
		dataWriter.WriteLine(data.totalSteps.ToString());

		dataWriter.WriteLine("Waypoint Coordinates:");
		foreach (Vector3 point in data.waypoints)
		{
			dataWriter.WriteLine(point.x.ToString() + " " + point.z.ToString());
		}

		dataWriter.WriteLine("Waypoint Distances");
		foreach (float dist in data.distancesWaypoint)
		{
			dataWriter.WriteLine(dist.ToString());
		}

		dataWriter.WriteLine("Actual Distances Moved");
		foreach (float dist in data.distancesMoved)
		{
			dataWriter.WriteLine(dist.ToString());
		}

		dataWriter.WriteLine("Times for each waypoint");
		foreach (float time in data.times)
		{
			dataWriter.WriteLine(time.ToString());
		}

        dataWriter.WriteLine("Obstacle order");
        foreach (int obstacle in data.obstacle)
        {
            dataWriter.WriteLine(obstacle.ToString());
        }

		dataWriter.Close();
	}

	public static void StartFirstRun()
	{
		//just going to assume 10 participants, this code just makes sure we get both "starting" methods randomly assigned in an equal fashion
		List<int> methods = new List<int> { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 };
		methods.Shuffle();

		int startingMethod;
		if (instance._participantNumber < methods.Count)
			startingMethod = methods[instance._participantNumber];
		else
			startingMethod = Random.Range(0, 2);

		//0 is the "look" method
		if (startingMethod == 0)
		{
			instance.currentMethod = "look";
			StartRun("look", true);
		}

		//1 is the "step" method
		else
		{
			instance.currentMethod = "step";
			StartRun("step", true);
		}
	}

	public static void StartRun(string method, bool isPractice)
	{
		instance.currentMethod = method;
		instance.isCurrentlyPractice = isPractice;
		
		Application.LoadLevel(1);

	}

	public static void FinishRun(RunData data)
	{
		if (instance.ranOne)
		{
			instance.WriteData(data);
			Application.LoadLevel(2);
		}
		else
		{
			instance.WriteData(data);
			instance.ranOne = true;

			if (instance.currentMethod == "step")
				StartRun("look", true);
			else
				StartRun("step", true);

			
		}
	}

	public static void FinishPractice()
	{
		if(instance.currentMethod == "step")
			StartRun("step", false);
		else if (instance.currentMethod == "look")
			StartRun("look", false);
	}

	


}



public struct RunData
{
	public string method;
	public List<Vector2> waypoints;
	public List<float> distancesWaypoint;
	public List<float> distancesMoved;
	public List<float> times;
    public List<int> obstacle;
	public float totalTime;
	public float totalDistanceTraveled;
	public int totalSteps;
}