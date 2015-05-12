using UnityEngine;
using System.Collections;

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

	// Use this for initialization
	void Start ()
	{
	    instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        ZoneCompleted();
	    }
        UpdateTimers();
	}

    void UpdateTimers()
    {
        totalTime += Time.deltaTime;
        timeSinceLastCollection += Time.deltaTime;
    }

    protected void InternalZoneCompleted()
    {
        timeSinceLastCollection = 0f;
        curZoneCount += 1;

        Vector3 oldPos = ZoneTransform.position;
        Vector3 newPos = GetRandomGridPosition();

        while (Vector3.Distance(oldPos, newPos) < 10)
        {
            newPos = GetRandomGridPosition();
        }

        ZoneTransform.transform.position = newPos;
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
}
