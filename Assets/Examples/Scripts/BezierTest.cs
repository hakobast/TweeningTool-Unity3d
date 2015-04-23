using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierTest : MonoBehaviour {

	public Transform target;

	[Range(0f,1f)]
	public float t;
	public float duration = 1;
	public float delay;
	public CTEaseType easeType;
	public CTweenType tweenType;

	public List<Transform> trPoints;

	CTBezierPath path;
	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.A))
			appendCurve();

		Vector3[] controls = new Vector3[trPoints.Count];
		for(int i = 0; i < trPoints.Count; i++)
			controls[i] = trPoints[i].position;

		if(Input.GetKeyDown(KeyCode.M))
		{
			CTweenConfig conf = new CTweenConfig();
			conf.delay = delay;
			conf.ease = easeType;
			conf.type = tweenType;
			CoolestTween.moveToBezier(target,controls,duration,conf);
		}

		path = new CTBezierPath(controls);
		
		CTBezierCurve[] curves = path.curves;
		for(int i = 0; i < curves.Length; i++)
		{
			Vector3[] points = curves[i].getAll();
			for(int k = 0; k < points.Length-1; k++)
				Debug.DrawLine(points[k],points[k+1],Color.green);
		}

		//target.position = path.getPoint(t);
	}

	public void appendCurve()
	{
		Transform start = trPoints[trPoints.Count-1];

		Transform c1 = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		c1.name = "c" + trPoints.Count/4 + "-1";
		Transform c2 = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		c2.name = "c" + trPoints.Count/4 + "-2";
		Transform end = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		end.name = "end" + trPoints.Count/4 + "-1";

		trPoints.Add(start);
		trPoints.Add(c1);
		trPoints.Add(c2);
		trPoints.Add(end);
	}
}
