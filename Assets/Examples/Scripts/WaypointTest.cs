using UnityEngine;
using System.Collections;

public class WaypointTest : MonoBehaviour {

	public Transform target;
	public Transform[] trPoints;

	public float duration;
	public float delay;
	public CTweenType type;
	public CTEaseType easeType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3[] points = new Vector3[trPoints.Length];
		for(int i = 0; i < points.Length; i++)
			points[i] = trPoints[i].position;

		if(Input.GetKeyDown(KeyCode.M))
		{
			CTweenConfig conf = new CTweenConfig();
			conf.setDelay(delay);
			conf.setEaseType(easeType);
			conf.setTweenType(type);
			CoolestTween.moveToWaypoints(target,points,duration,conf);
		}

		CTWaypoints waypoints = new CTWaypoints(points);
		points = waypoints.getAll();
		for(int k = 0; k < points.Length-1; k++)
			Debug.DrawLine(points[k],points[k+1],Color.green);
	}
}
