using UnityEngine;
using System.Collections;

public class TargetZone : MonoBehaviour
{

    public Material mat;

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
        if (Input.GetKeyDown(KeyCode.J))
        {
            Rect r = new Rect(6.25f, 13.75f, 7.5f, 7.5f);
            Vector2 v = new Vector2(10.60691f, 6.227509f);
            Debug.Log(DistancePointToRectangle(v, r));
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
        float minValid = -18;
        float maxValid = 18;

        

        float x = 0;
        float y = 0;

        bool intersectingRectangle = true;

        while(intersectingRectangle){

            x = Random.Range(minValid, maxValid);
            y = Random.Range(minValid, maxValid);
            Vector2 point = new Vector2(x, y);

            intersectingRectangle = !isValidPlacement(point);
        }

        this.transform.position = new Vector3(x, .01f, y);
        //TestRunner.ZoneCompleted();
    }

    bool isValidPlacement(Vector2 point){
        Rect[] rectsToAvoid = new Rect[4];
        rectsToAvoid[0] = new Rect(-13.75f, 13.75f, 7.5f, 7.5f);
        rectsToAvoid[1] = new Rect(6.25f, 13.75f, 7.5f, 7.5f);
        rectsToAvoid[2] = new Rect(-13.75f, -6.25f, 7.5f, 7.5f);
        rectsToAvoid[3] = new Rect(6.25f, -6.25f, 7.5f, 7.5f);

        foreach(Rect r in rectsToAvoid){
            if(DistancePointToRectangle(point, r) < 2) return false;
        }
        return true;
    }

    public static float DistancePointToRectangle(Vector2 p, Rect r)
    {
        var cx = Mathf.max(Mathf.min(p.x, r.left + r.width), r.left);
        var cy = Mathf.max(Mathf.min(p.y, r.top + r.height), r.top);
        return Math.sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
    }
}
