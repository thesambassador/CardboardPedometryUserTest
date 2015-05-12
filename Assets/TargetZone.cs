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
        TestRunner.ZoneCompleted();
    }
}
