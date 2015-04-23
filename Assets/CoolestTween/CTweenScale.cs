using UnityEngine;
using System.Collections;

public class CTweenScale : CBaseTweener {

	public Vector3 toScale = Vector3.one;

	private Vector3 fromScale;
	protected override void Awake ()
	{
		base.Awake ();
	}

	public override void initFromValue ()
	{
		fromScale = transform.localScale;
	}

	public override void playForward ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.localScale = fromScale;
		tweenController = CoolestTween.scaleTo(transform,toScale,duration,config);
	}
	
	public override void playReverse ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		transform.localScale = toScale;
		tweenController = CoolestTween.scaleTo(transform,fromScale,duration,config);
	}

	public override void resetValues ()
	{
		if(tweenController != null)
			transform.localScale = fromScale;
	}
}
