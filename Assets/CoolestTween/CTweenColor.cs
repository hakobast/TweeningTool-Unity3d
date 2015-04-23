using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class CTweenColor : CBaseTweener {
	
	public Color toColor = Color.white;

	private Color fromColor;
	protected override void Awake ()
	{
		base.Awake ();
	}

	public override void initFromValue ()
	{
		if(renderer is SpriteRenderer)
			fromColor = (renderer as SpriteRenderer).color;
		else
			fromColor = renderer.sharedMaterial.color;
	}

	public override void playForward ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		if(renderer is SpriteRenderer)
			(renderer as SpriteRenderer).color = fromColor;
		else
			renderer.sharedMaterial.color = fromColor;
		
		tweenController = CoolestTween.colorTo(transform,toColor,duration,config);
	}
	
	public override void playReverse ()
	{
		initConfigs();
		if(tweenController == null)
			initFromValue();
		if(IsPlaying)
			stop();

		if(renderer is SpriteRenderer)
			(renderer as SpriteRenderer).color = toColor;
		else
			renderer.sharedMaterial.color = toColor;
		
		tweenController = CoolestTween.colorTo(transform,fromColor,duration,config);
	}
	
	public override void resetValues ()
	{
		if(tweenController != null)
		{
			if(renderer is SpriteRenderer)
				(renderer as SpriteRenderer).color = fromColor;
			else
				renderer.sharedMaterial.color = fromColor;
		}
	}
}
