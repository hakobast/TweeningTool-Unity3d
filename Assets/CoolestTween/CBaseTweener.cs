using UnityEngine;
using System.Collections;
using System;

public abstract class CBaseTweener : MonoBehaviour {
	
	public float duration = 1;
	public float delay;
	public int repeatCount;
	public CTweenType type;
	public CTEaseType easeType;
	
	public bool playOnAwake = true;

	public bool IsPlaying{
		get{
			if(tweenController == null)
				return false;

			return tweenController.IsStarted && !tweenController.IsStopped && !tweenController.IsPaused;
		}
	}

	public CTweenController tweenController;
	protected CTweenConfig config;

	public string completeAction;
	public MonoBehaviour completeHandler;
	public bool destroyWhenFinish;

	public abstract void playForward();
	public abstract void playReverse();
	public abstract void initFromValue();
	public abstract void resetValues();

	protected virtual void Awake()
	{
		if(playOnAwake && enabled)
			playForward();
	}

	protected virtual void Start(){}
	
	protected void initConfigs()
	{
		config = new CTweenConfig();
		config.setDelay(delay);
		config.setEaseType(easeType);
		config.setTweenType(type);
		config.setRepeatCount(repeatCount);
		config.setCompleteCallback(OnFinish);
	}

	private void OnFinish()
	{
		if(!string.IsNullOrEmpty(completeAction) && completeHandler != null)
		{
			Action callback = Action.CreateDelegate(typeof(Action),completeHandler,completeAction,false,false) as Action;
			callback();
		}

		if(destroyWhenFinish && this != null && gameObject != null)
			Destroy(gameObject);
	}

	public void pause()
	{
		if(tweenController != null)
			tweenController.pause();
	}

	public void resume()
	{
		if(tweenController != null)
			tweenController.resume();
	}

	public void stop()
	{
		if(tweenController != null)
			tweenController.stop();
	}

	public void stopAndDestroy()
	{
		if(tweenController != null)
			tweenController.stop();

		Destroy(gameObject);
	}
}