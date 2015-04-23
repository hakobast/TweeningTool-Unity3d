using UnityEngine;
using System.Collections;

public class CTweenRotation : CBaseTweener {
	
	public Vector3 toRotation;

	private Vector3 fromRotation;
	protected override void Awake ()
	{
		base.Awake ();
	}

	public override void initFromValue ()
	{
		fromRotation = transform.eulerAngles;
	}

	public override void playForward ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.eulerAngles = fromRotation;
		tweenController = CoolestTween.rotateTo(transform,toRotation,duration,config);
	}
	
	public override void playReverse ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.eulerAngles = toRotation;
		tweenController = CoolestTween.rotateTo(transform,fromRotation,duration,config);
	}

	public override void resetValues ()
	{
		if(tweenController != null)
			transform.eulerAngles = fromRotation;
	}
}
