using UnityEngine;
using System.Collections;

public class TargetZone : MonoBehaviour
{

    public Material mat;

    public Transform obstacle;

    private Renderer groundRenderer;
    private Renderer wallRenderer;

    public Color startColor;

    public float zoneDuration = 2f;

    private float curZoneDuration = 0f;

	// Use this for initialization
	void Start ()
	{
	    groundRenderer = GetComponent<MeshRenderer>();
	    wallRenderer = transform.FindChild("targetZoneWall").GetComponent<MeshRenderer>();

	    startColor = groundRenderer.material.GetColor("_EmisColor");
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.K))
        {
            ZoneCompleted();
        }
        SetColor(Color.Lerp(startColor, Color.green, curZoneDuration / zoneDuration));
	}

    void SetColor(Color col)
    {
        groundRenderer.material.SetColor("_EmisColor", col);
        wallRenderer.material.SetColor("_EmisColor", col);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("TRIGGER ENTERED");
        curZoneDuration += Time.deltaTime;

        if (curZoneDuration > zoneDuration)
        {
            ZoneCompleted();
        }

    }

    void OnTriggerExit(Collider other)
    {
        curZoneDuration = 0;
    }

    void ZoneCompleted()
    {
        curZoneDuration = 0;


        //MoveZoneObstacle();

        TestRunner.ZoneCompleted();
    }

    void MoveZoneNoObstacle()
    {
        obstacle.position = new Vector3(-50, -50, -50);

        transform.position = GetNewZonePosition(transform.position, 8);

    }

    void MoveZoneObstacle()
    {
        Vector3 currentPos = transform.position;

        transform.position = GetNewZonePosition(transform.position, 8);

        //get the midpoint

        Vector3 offset = transform.position - currentPos;
        float halfMag = offset.magnitude / 2;

        Vector3 newObsPos = currentPos + offset.normalized * halfMag;
        obstacle.position = newObsPos;

        //now orientation
        obstacle.LookAt(transform.position);
       



    }

    Vector3 GetNewZonePosition(Vector3 curPos, float minDist)
    {
        Vector3 result;
        result.y = curPos.y;

        Vector2 current2d = new Vector2(curPos.x, curPos.z);

        float minValid = -18;
        float maxValid = 18;

        Vector2 newPoint = new Vector2();
        do
        {
            float x = Random.Range(minValid, maxValid);
            float y = Random.Range(minValid, maxValid);

            newPoint = new Vector2(x, y);
        }
        while (Vector2.Distance(current2d, newPoint) < minDist);
        
        result.x = newPoint.x;
        result.z = newPoint.y;

        return result;
    }





}
