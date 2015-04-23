using UnityEngine;
using System.Collections;

public class CTweenPosition : CBaseTweener {
	
	public Vector3 toPosition;

	private Vector3 fromPosition;
	protected override void Awake ()
	{
		base.Awake ();
	}

	public override void initFromValue ()
	{
		fromPosition = transform.position;
	}

	public override void playForward ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.position = fromPosition;
		tweenController = CoolestTween.moveTo(transform,toPosition,duration,config);
	}
	
	public override void playReverse ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.position = toPosition;
		tweenController = CoolestTween.moveTo(transform,fromPosition,duration,config);
	}

	public override void resetValues ()
	{
		if(tweenController != null)
			transform.position = fromPosition;
	}
}
