using UnityEngine;
using System.Collections;

public class TweenTest : MonoBehaviour {

	public float duration;
	public float delay;
	public int count = 1;
	public CTweenAction action;
	public CTweenType tweenType;
	public CTEaseType easeType;
	public Transform myObj;
	public Transform otherObj;
	public Transform dest;

	public int repeats;
	public Color tweenColor = Color.blue;

	// Use this for initialization
	void Start () {


	}

	void spawn()
	{
		CTweenConfig config = new CTweenConfig();
		config.setDelay(delay);
		config.setTweenType(tweenType);
		config.setEaseType(easeType);
		config.setRepeatCount(repeats);

		GameObject primitive = null;
		for(int i = 0; i < count; i++)
		{
			primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
			primitive.transform.position = Random.insideUnitSphere*20;

			if(action == CTweenAction.Move)
				CoolestTween.moveTo(primitive.transform,dest.position,duration,config);
			else if(action == CTweenAction.Rotate)
				CoolestTween.rotateTo(primitive.transform,new Vector3(0,0,180),duration,config);
			else if(action == CTweenAction.Scale)
				CoolestTween.scaleTo(primitive.transform,new Vector3(6,6,6),duration,config);
			else if(action == CTweenAction.Alpha)
				CoolestTween.alphaTo(primitive.transform,0.1f,duration,config);
		}
	}

	private CTweenController last;

	public CTweenPosition pos;

	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.V))
			pos.playForward();

		if(Input.GetKeyDown(KeyCode.X))
			CoolestTween.stopTweens(myObj);

		if(Input.GetKeyDown(KeyCode.Z))
			CoolestTween.stopAll();

		if(Input.GetKeyDown(KeyCode.N))
			CoolestTween.pauseTweens(myObj);

		if(Input.GetKeyDown(KeyCode.M))
			CoolestTween.resumeTweens(myObj);

		if(Input.GetKeyDown(KeyCode.R))
			last.resume();
		
		if(Input.GetKeyDown(KeyCode.P))
			last.pause();

		if(Input.GetKeyDown(KeyCode.L))
			Debug.Log("COUNT " + CoolestTween.TweenCount);

		if(Input.GetKeyDown(KeyCode.S))
			spawn();

		if(Input.GetKeyDown(KeyCode.T))
		{
			CTweenConfig config = new CTweenConfig();
			config.setDelay(delay);
			config.setTweenType(tweenType);
			config.setEaseType(easeType);

			for(int i = 0; i < count; i++)
			{
				if(action == CTweenAction.Move)
					last = CoolestTween.moveTo(myObj,dest.position,duration,config);
				else if(action == CTweenAction.Rotate)
					last = CoolestTween.rotateZTo(myObj,180,duration,config);
				else if(action == CTweenAction.Scale)
					last = CoolestTween.scaleTo(myObj,new Vector3(6,6,6),duration,config);
				else if(action == CTweenAction.Alpha)
					last = CoolestTween.alphaTo(myObj,0.1f,duration,config);
				else if(action == CTweenAction.Color)
					last = CoolestTween.colorTo(myObj,tweenColor,duration,config);
			}

			/*for(int i = 0; i < count; i++)
				LeanTween.move(otherObj.gameObject,dest.position,duration,args);*/
				
		}
	}

	void OnComplete(object k)
	{
		Debug.Log("COMPLETE " + k);
	}
}
