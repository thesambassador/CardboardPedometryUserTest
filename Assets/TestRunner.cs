﻿using UnityEngine;
using System.Collections.Generic;

public class TestRunner : MonoBehaviour
{
    public Transform ZoneTransform;

    public static TestRunner instance;

    public int NumTestZones = 15;

    private int curZoneCount = 0;

    public int xMinLimit = -16;
    public int xMaxLimit = 16;
    public int zMinLimit = -16;
    public int zMaxLimit = 16;

    public float totalTime = 0f;
    public float timeSinceLastCollection = 0f;

	public float totalDistanceMoved = 0f;
	public float currentWaypointDistanceMoved = 0f;

	public bool isPractice;
	public bool hasStarted = false;
	public int totalSteps = 0;

	public Transform PlayerTransform;
	public Vector3 lastFramePlayerPos;

	private RunData data;

	// Use this for initialization
	void Start ()
	{
	    instance = this;

	    Random.seed = 50;

		data.times = new List<float>();
		data.waypoints = new List<Vector2>();
		data.distancesMoved = new List<float>();
		data.distancesWaypoint = new List<float>();

		data.waypoints.Add(new Vector2(0,0)); //first waypoint is where the player starts, at 0,0

		StepDetector.OnStepDetected += OnStepDetected;
	}

	private void OnStepDetected()
	{
		totalSteps ++;
	}


	// Update is called once per frame
	void Update ()
	{
		if (isPractice) NumTestZones = 3;

	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        ZoneCompleted();
	    }
        UpdateTimers();


		//update our player position and distance tracking
		float distanceMoved = Vector3.Distance(PlayerTransform.position, lastFramePlayerPos);
		totalDistanceMoved += distanceMoved;
		currentWaypointDistanceMoved += distanceMoved;
		lastFramePlayerPos = PlayerTransform.position;

	}

    void UpdateTimers()
    {
	    if (hasStarted)
	    {
		    totalTime += Time.deltaTime;
		    timeSinceLastCollection += Time.deltaTime;
	    }
    }

    protected void InternalZoneCompleted()
    {
		curZoneCount += 1;

	    Vector2 lastWaypoint = data.waypoints[data.waypoints.Count - 1];
	    Vector2 currentWaypoint = new Vector2(ZoneTransform.position.x, ZoneTransform.position.z);

	    float dist = Vector2.Distance(lastWaypoint, currentWaypoint);

		//record our data:
		data.times.Add(timeSinceLastCollection);
		data.waypoints.Add(currentWaypoint);
	    data.distancesWaypoint.Add(dist);
		data.distancesMoved.Add(currentWaypointDistanceMoved);

		//reset variables
	    currentWaypointDistanceMoved = 0;
	    timeSinceLastCollection = 0f;

		//if we're done, add the final data and finish
	    if (curZoneCount >= NumTestZones)
	    {
			StepDetector.OnStepDetected -= OnStepDetected;
		    if (isPractice)
			    DataRecorder.FinishPractice();
		    else
		    {
			    data.totalDistanceTraveled = totalDistanceMoved;
			    data.totalTime = totalTime;
			    data.totalSteps = totalSteps;
			    DataRecorder.FinishRun(data);
		    }
	    }
	    else
	    {

		    Vector3 oldPos = ZoneTransform.position;
		    Vector3 newPos = GetRandomGridPosition();

		    while (Vector3.Distance(oldPos, newPos) < 10)
		    {
			    newPos = GetRandomGridPosition();
		    }

		    ZoneTransform.transform.position = newPos;
	    }
    }

    public Vector3 GetRandomGridPosition()
    {
        Vector3 oldPos = ZoneTransform.position;
        int newX = Random.Range(xMinLimit / 2, xMaxLimit / 2) * 2;
        int newZ = Random.Range(xMinLimit / 2, xMaxLimit / 2) * 2;
        Vector3 newPos = oldPos;
        newPos.x = newX;
        newPos.z = newZ;
        return newPos;
    }

    public static void ZoneCompleted()
    {
        instance.InternalZoneCompleted();
    }

	public static void StartExperiment()
	{
		instance.hasStarted = true;
		PlayerController controller = instance.PlayerTransform.GetComponent<PlayerController>();
		controller.EnableMovement();
	}



 


}
