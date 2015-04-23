using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierUtils
{
	public static Vector3[] CubicCurvePoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4,int segments)
	{
		Vector3[] points = new Vector3[segments+1];

		for(int i = 0; i < segments; i++)
		{
			points[i] = CubicCurve(p1,p2,p3,p4,i/(float)segments);
		}
		points[points.Length-1] = p4;

		return points;
	}

	public static Vector3 CubicCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		float a = (1-t);
		float aa = a*a;
		float aaa = aa*a;
		float tt = t*t;
		float ttt = tt*t;

		return aaa*p1 + 3*aa*t*p2 + 3*a*tt*p3 + ttt*p4;
	}

	public static Vector3 QuadraticCurve(Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float a = (1-t);
		float aa = a*a;
		float tt = t*t;

		return aa*p1 + 2*t*a*p2 + tt*p3;
	}
}