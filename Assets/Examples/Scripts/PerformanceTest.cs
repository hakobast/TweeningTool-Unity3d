using UnityEngine;
using System.Collections;

public class PerformanceTest : MonoBehaviour {

	public int objectsCount = 100;
	public float movingDuration = 2.0f;
	public Transform objPref;

	// Use this for initialization
	void Start () {
	
		for(int i = 0; i < objectsCount; i++)
		{
			Transform obj = Instantiate(objPref) as Transform;
			const float r = 10.0f;
			Vector3 randPos = Random.insideUnitSphere*r;
			CoolestTween.moveTo(obj,randPos,movingDuration,new CTweenConfig().setTweenType(CTweenType.PingPong));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
