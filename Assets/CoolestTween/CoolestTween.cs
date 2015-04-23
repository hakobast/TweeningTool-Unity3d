/*
 * Version 1.4
 * added Bezier curve movement
 * added Waypoint movement
 * code improvements
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoolestTween : MonoBehaviour
{
	public static readonly int INITIAL_CAPACITY = 100;

	private static GameObject tweenGameObject;
	private static CoolestTween tweener;

	protected List<CTweenDesc> tweens;
	protected LinkedList<CTweenDesc> completedTweens;

	public static int TweenCount
	{
		get
		{
			if(tweener == null)
				return 0;

			return tweener.tweens.Count - tweener.completedTweens.Count;
		}
	}

	void Awake()
	{
		if(tweenGameObject == null)
		{
			tweenGameObject = gameObject;
			tweener = this;
		}
			
	}

	void Start()
	{

	}

	System.Diagnostics.Stopwatch watch;
	void Update()
	{
		/*watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		*/
		updateTweens();
		checkDelay();
			
		/*watch.Stop();
		Debug.Log("TIME " + watch.ElapsedMilliseconds);*/
	}

	private void updateTweens()
	{
		CTweenDesc tweenInfo = null;
		for(int i = 0,len = tweens.Count; i < len; i++)
		{
			tweenInfo = tweens[i];

			if(tweenInfo.isStarted && !tweenInfo.isPaused && !tweenInfo.isStopped)
			{
				if(tweenInfo.target == null)
				{
					tweenInfo.isStopped = true;
					completedTweens.AddLast(tweenInfo);
					continue;
				}

				tweenInfo.update(Time.time);
				
				if(tweenInfo.isComplete)
					complete(tweenInfo);
			}
		}
	}

	private void checkDelay()
	{
		CTweenDesc tweenInfo = null;
		for(int i = 0,len = tweens.Count; i < len; i++)
		{
			tweenInfo = tweens[i];
			if(tweenInfo.isStopped == false && tweenInfo.isStarted == false && tweenInfo.isPaused == false && Time.time > tweenInfo.startTime+tweenInfo.delay+tweenInfo.pauseDuration)
			{
				if(tweenInfo.tweenAction == CTweenAction.Move)
				{
					tweenInfo.startValue = tweenInfo.target.position;
				}
				else if(tweenInfo.tweenAction == CTweenAction.Rotate)
				{
					tweenInfo.startValue = tweenInfo.target.eulerAngles;
				}
				else if(tweenInfo.tweenAction == CTweenAction.Scale)
				{
					tweenInfo.startValue = tweenInfo.target.localScale;
				}
				else if(tweenInfo.tweenAction == CTweenAction.Alpha)
				{
					if(tweenInfo._renderer is SpriteRenderer)
						tweenInfo.startValue = new Vector3((tweenInfo._renderer as SpriteRenderer).color.a,0,0);
					else 
						tweenInfo.startValue = new Vector3(tweenInfo._renderer.sharedMaterial.color.a,0,0);
				}
				else if(tweenInfo.tweenAction == CTweenAction.Color)
				{
					Color color;
					if(tweenInfo._renderer is SpriteRenderer)
						color = (tweenInfo._renderer as SpriteRenderer).color;
					else
						color = tweenInfo._renderer.sharedMaterial.color;
					tweenInfo.startValue4 = new Vector4(color.r,color.g,color.b,color.a);
				}

				tweenInfo.isStarted = true;
			}
		}
	}
			
	private void complete(CTweenDesc tweenInfo)
	{
		tweenInfo.isStopped = true;

		if(tweenInfo.duration > 0 && tweenInfo.tweenType != CTweenType.Once)
		{
			tweenInfo.repeats++;
			if(tweenInfo.tweenType == CTweenType.PingPong)
			{
				tweenInfo.direction = -tweenInfo.direction;
				if(tweenInfo.tweenAction == CTweenAction.Color)
				{
					Vector4 temp = tweenInfo.startValue4;
					tweenInfo.startValue4 = tweenInfo.endValue4;
					tweenInfo.endValue4 = temp;
				}
				else
				{
					Vector3 temp = tweenInfo.startValue;
					tweenInfo.startValue = tweenInfo.endValue;
					tweenInfo.endValue = temp;
				}
			}

			if(tweenInfo.repeatCount == 0 || tweenInfo.RepeatsLeft > 0)
			{
				tweenInfo.startTime = Time.time - tweenInfo.delay;
				tweenInfo.pauseDuration = 0;
				tweenInfo.isStopped = false;
				tweenInfo.isComplete = false;
			}
			else
			{
				completedTweens.AddLast(tweenInfo);
				sendCompleteMessage(tweenInfo);
			}
		} 
		else //once tweentype
		{
			completedTweens.AddLast(tweenInfo);
			sendCompleteMessage(tweenInfo);
		}
	}

	private void sendCompleteMessage(CTweenDesc tweenInfo)
	{
		if(tweenInfo.completeAction != null)
		{
			tweenInfo.completeAction();
		}
		else if(tweenInfo.completeActionWithArg != null)
		{
			tweenInfo.completeActionWithArg(tweenInfo.completeParams);
		}
	}

	public static void init()
	{
		if(tweenGameObject == null || tweener.tweens == null)
		{
			if(tweenGameObject == null)
			{
				tweenGameObject = new GameObject("Tweener d(-_-)b");
				tweener = tweenGameObject.AddComponent<CoolestTween>();
			}
			tweener.tweens = new List<CTweenDesc>(INITIAL_CAPACITY);
			tweener.completedTweens = new LinkedList<CTweenDesc>();

			CTweenDesc desc = null;
			for(int i = 0; i < INITIAL_CAPACITY; i++)
			{
				desc = new CTweenDesc();
				desc.isStopped = true;
				tweener.tweens.Add(desc);
			}
				
			for(int i = 0; i < tweener.tweens.Count; i++)
				tweener.completedTweens.AddLast(tweener.tweens[i]);
		}
	}

	public static CTweenDesc getTween()
	{
		init();

		CTweenDesc tween = null;
		if(tweener.completedTweens.Count > 0)
		{
			tween = tweener.completedTweens.First.Value;
			tweener.completedTweens.RemoveFirst();

			tween.delay = 0;
			tween.repeats = 0;
			tween.repeatCount = 0;
			tween.pauseTime = 0;
			tween.direction = 1;
			tween.isStopped = false;
			tween.isComplete = false;
			tween.completeAction = null;
			tween.completeActionWithArg = null;
			tween.completeParams = null;
		}

		if(tween == null)
		{
			tween = new CTweenDesc();
			tweener.tweens.Add(tween);
		}

		return tween;
	}

	public static CTweenController moveTo(Transform gameObject, Vector3 to, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc.endValue = to;
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.Move;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);

		desc.startValue = gameObject.position;
		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController moveToBezier(Transform gameObject, Vector3[] points, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}

		CTBezierPath path = new CTBezierPath(points);

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc.bezierPath = path;
		desc.startValue = path.getPoint(0f);
		desc.endValue = path.getPoint(1f);
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.MoveBezier;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);

		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController moveToWaypoints(Transform gameObject, Vector3[] points, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}
		
		CTWaypoints path = new CTWaypoints(points);
		
		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc.waypointPath = path;
		desc.startValue = path.getPoint(0f);
		desc.endValue = path.getPoint(1f);
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.MoveWaypoint;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);

		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController rotateTo(Transform gameObject, Vector3 to, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc.endValue = to;
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.Rotate;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);
		
		desc.startValue = gameObject.eulerAngles;
		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController scaleTo(Transform gameObject, Vector3 to, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc.endValue = to;
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.Scale;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);
		
		desc.startValue = gameObject.localScale;
		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController alphaTo(Transform gameObject, float to, float duration,CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}

		if(gameObject.renderer == null)
		{
			Debug.LogError("Alpha tweening object must have a renderer");
			return null;
		}

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc._renderer = gameObject.renderer;
		desc.endValue = new Vector3(to,0,0);
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.Alpha;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);

		if(gameObject.renderer is SpriteRenderer)
			desc.startValue = new Vector3((gameObject.renderer as SpriteRenderer).color.a,0,0);
		else 
			desc.startValue = new Vector3(gameObject.renderer.sharedMaterial.color.a,0,0);

		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;
		
		return new CTweenController(desc);
	}

	public static CTweenController colorTo(Transform gameObject, Color to, float duration, CTweenConfig descriptor = null)
	{
		if(gameObject == null)
		{
			Debug.LogError("Tweening object is null");
			return null;
		}
		
		if(gameObject.renderer == null)
		{
			Debug.LogError("Color tweening object must have a renderer");
			return null;
		}

		CTweenDesc desc = getTween();
		desc.target = gameObject;
		desc._renderer = gameObject.renderer;
		desc.endValue4 = new Vector4(to.r,to.g,to.b,to.a);
		desc.duration = duration;
		desc.easeType = CTEaseType.Linear;
		desc.tweenAction = CTweenAction.Color;
		desc.tweenType = CTweenType.Once;
		setConfigs(desc,descriptor);

		Color color;
		if(gameObject.renderer is SpriteRenderer)
			color = (gameObject.renderer as SpriteRenderer).color;
		else
			color = gameObject.renderer.sharedMaterial.color;

		desc.startValue4 = new Vector4(color.r,color.g,color.b,color.a);
		desc.startTime = Time.time;
		desc.isStarted = desc.delay <= 0;

		return new CTweenController(desc);
	}

	public static CTweenController rotateXTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.eulerAngles;
		dest.x = to;
		
		return rotateTo(gameObject,dest,duration,config);
	}
	
	public static CTweenController rotateYTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.eulerAngles;
		dest.y = to;
		
		return rotateTo(gameObject,dest,duration,config);
	}
	
	public static CTweenController rotateZTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.eulerAngles;
		dest.z = to;
		
		return rotateTo(gameObject,dest,duration,config);
	}

	public static CTweenController scaleXTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.localScale;
		dest.x = to;
		
		return scaleTo(gameObject,dest,duration,config);
	}
	
	public static CTweenController scaleYTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.localScale;
		dest.y = to;
		
		return scaleTo(gameObject,dest,duration,config);
	}
	
	public static CTweenController scaleZTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.localScale;
		dest.z = to;
		
		return scaleTo(gameObject,dest,duration,config);
	}

	public static CTweenController moveXTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;

		Vector3 dest = gameObject.position;
		dest.x = to;

		return moveTo(gameObject,dest,duration,config);
	}

	public static CTweenController moveYTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.position;
		dest.y = to;
		
		return moveTo(gameObject,dest,duration,config);
	}

	public static CTweenController moveZTo(Transform gameObject, float to, float duration,CTweenConfig config = null)
	{
		if(gameObject == null)
			return null;
		
		Vector3 dest = gameObject.position;
		dest.z = to;
		
		return moveTo(gameObject,dest,duration,config);
	}

	private static void setConfigs(CTweenDesc desc, CTweenConfig config)
	{
		if(config == null)
			return;

		desc.completeAction = config.completeAction;
		desc.completeActionWithArg = config.completeActionWithArg;
		desc.completeParams = config.completeParams;
		desc.easeType = config.ease;
		desc.tweenType = config.type;
		desc.repeatCount = config.repeatCount;
		desc.delay = config.delay;
	}

	public static void pauseTween(CTweenDesc tween)
	{
		if(tweener == null || tween == null)
		{
			return;
		}

		int index = tweener.tweens.IndexOf(tween);
		if(index != -1)
		{
			if(tween.isPaused == false)
			{
				tween.isPaused = true;
				tween.pauseTime = Time.time;
			}
		}
	}

	public static void pauseTweens(Transform obj)
	{
		if(tweener == null || obj == null)
		{
			return;
		}

		CTweenDesc tween = null;
		for(int i = 0; i < tweener.tweens.Count; i++)
		{
			tween = tweener.tweens[i];
			if(tween.target == obj && tween.isPaused == false)
			{
				tween.isPaused = true;
				tween.pauseTime = Time.time;
			}
		}
	}

	public static void resumeTweens(Transform obj)
	{
		if(tweener == null || obj == null)
		{
			return;
		}

		CTweenDesc tween = null;
		for(int i = 0; i < tweener.tweens.Count; i++)
		{
			tween = tweener.tweens[i];
			if(tween.target == obj && tween.isPaused == true)
			{
				tween.isPaused = false;
				tween.pauseDuration += Time.time-tween.pauseTime;
			}
		}
	}

	public static void resumeTween(CTweenDesc tween)
	{
		if(tweener == null || tween == null)
		{
			return;
		}
		
		int index = tweener.tweens.IndexOf(tween);
		if(index != -1)
		{
			if(tween.isPaused == true)
			{
				tween.isPaused = false;
				tween.pauseDuration += Time.time-tween.pauseTime;
			}
		}
	}

	public static void stopTweens(Transform obj)
	{
		if(tweener == null || obj == null)
		{
			return;
		}

		CTweenDesc tween = null;
		for(int i = 0; i < tweener.tweens.Count; i++)
		{
			tween = tweener.tweens[i];
			if(tween.target == obj && tween.isStopped == false)
			{
				tween.isStopped = true;
				tweener.completedTweens.AddLast(tween);
			}
		}
	}

	public static void stopTween(CTweenDesc tween)
	{
		if(tween == null)
			return;

		if(tweener == null)
		{
			tween.isStopped = true;
			return;
		}

		int index = tweener.tweens.IndexOf(tween);
		if(index != -1)
		{
			if(tween.isStopped == false)
			{
				tween.isStopped = true;
				tweener.completedTweens.AddLast(tween);
			}
		}
		else
		{
			tween.isStopped = true;
		}
	}

	public static void stopAll()
	{
		if(tweener == null)
			return;

		for(int i = 0; i < tweener.tweens.Count; i++)
		{
			if(tweener.tweens[i].isStopped == false)
			{
				tweener.tweens[i].isStopped = true;
				tweener.completedTweens.AddLast(tweener.tweens[i]);
			}
		}
	}
}

public class CTweenConfig
{
	public CTweenType type = CTweenType.Once;
	public CTEaseType ease = CTEaseType.Linear;
	public float delay;
	public int repeatCount;

	public System.Action completeAction;
	public System.Action<object> completeActionWithArg;
	public object completeParams;

	public CTweenConfig setTweenType(CTweenType tweenType)
	{
		type = tweenType;
		return this;
	}

	public CTweenConfig setEaseType(CTEaseType easeType)
	{
		ease = easeType;
		return this;
	}

	public CTweenConfig setDelay(float delay)
	{
		this.delay = delay;
		return this;
	}

	public CTweenConfig setRepeatCount(int count)
	{
		repeatCount = count;
		return this;
	}

	public CTweenConfig setCompleteCallback(System.Action callback)
	{
		completeAction = callback;
		return this;
	}

	public CTweenConfig setCompleteCallback(System.Action<object> callback, object args)
	{
		completeActionWithArg = callback;
		completeParams = args;
		return this;
	}
}

public enum CTweenAction
{
	Move,
	Rotate,
	Scale,
	Alpha,
	Color,
	MoveBezier,
	MoveWaypoint
}

public enum CTweenType
{
	Once,
	PingPong,
	Loop
}

public class CTweenController
{
	private CTweenDesc tween;
	public CTweenController(CTweenDesc tw)
	{
		tween = tw;
	}

	public bool IsCompleted{
		get{
			return tween.isComplete;
		}
	}

	public bool IsPaused{
		get{
			return tween.isPaused;
		}
	}

	public bool IsStopped{
		get{
			return tween.isStopped;
		}
	}

	public bool IsStarted{
		get{
			return tween.isStarted;
		}
	}

	public float tweenTime{
		get{
			return tween.tweenTime;
		}
	}

	public CTweenType tweenType{
		get{
			return tween.tweenType;
		}
	}

	public CTEaseType easeType{
		get{
			return tween.easeType;
		}
	}

	public int repeatsLeft{
		get{
			return tween.RepeatsLeft;
		}
	}
	
	public void stop(){
		tween.stop();
	}

	public void pause(){
		tween.pause();
	}

	public void resume(){
		tween.resume();
	}
}

public class CTweenDesc
{
	public string id;
	public Transform target;
	public float duration;
	public CTEaseType easeType;
	public int repeatCount;
	public System.Action completeAction;
	public System.Action<object> completeActionWithArg;
	public object completeParams;
	public CTBezierPath bezierPath;
	public CTWaypoints waypointPath;
	public Vector3 startValue;
	public Vector4 startValue4;
	public Vector3 endValue;
	public Vector4 endValue4;
	public float startTime;
	public float pauseTime;
	public float pauseDuration;
	public float delay;
	public int repeats;
	public int direction = 1;
	public int waypointIndex;
	public CTweenAction tweenAction;
	public CTweenType tweenType;
	public bool isStarted;
	public bool isStopped;
	public bool isComplete;
	public float tweenTime;
	public bool isPaused;

	public int RepeatsLeft{
		get{
			return repeatCount-repeats;
		}
	}

	public Color colorEndValue
	{
		get
		{
			_color.r = endValue4.x;
			_color.g = endValue4.y;
			_color.b = endValue4.z;
			_color.a = endValue4.w;
			return _color;
		}
		set
		{
			endValue4.x = value.r;
			endValue4.y = value.g;
			endValue4.z = value.b;
			endValue4.w = value.a;
		}
	}
	
	private Vector3 _change;
	private Vector4 _change4;
	public Renderer _renderer;
	public Color _color;

	public CTweenDesc()
	{
		id = "Tween " + Time.time;
	}

	public void stop()
	{
		CoolestTween.stopTween(this);
	}

	public void pause()
	{
		CoolestTween.pauseTween(this);
	}

	public void resume()
	{
		CoolestTween.resumeTween(this);
	}

	virtual public void update(float time)
	{
		_change =  endValue-startValue;

		tweenTime = time - (startTime+delay+pauseDuration);
		if(duration == 0 || time > startTime+duration+delay+pauseDuration)
		{
			if(tweenAction == CTweenAction.Move)
				target.position = endValue;
			else if(tweenAction == CTweenAction.Rotate)
				target.eulerAngles = endValue;
			else if(tweenAction == CTweenAction.Scale)
				target.localScale = endValue;
			else if(tweenAction == CTweenAction.Alpha)
			{
				if(_renderer is SpriteRenderer)
				{
					_color = (_renderer as SpriteRenderer).color;
					_color.a = endValue.x;
					(_renderer as SpriteRenderer).color = _color;
				}	
				else 
				{
					_color = _renderer.sharedMaterial.color;
					_color.a = endValue.x;
					_renderer.sharedMaterial.color = _color;
				}
			}
			else if(tweenAction == CTweenAction.Color)
			{
				_color.r = endValue4.x;
				_color.g = endValue4.y;
				_color.b = endValue4.z;
				_color.a = endValue4.w;
				if(_renderer is SpriteRenderer)
				{
					(_renderer as SpriteRenderer).color = _color;
				}
				else 
				{
					_renderer.sharedMaterial.color = _color;
				}
			}
			else if(tweenAction == CTweenAction.MoveBezier)
			{
				target.position = endValue;
			}
			else if(tweenAction == CTweenAction.MoveWaypoint)
			{
				target.position = endValue;
			}

			isComplete = true;
			return;
		}

		if(tweenAction == CTweenAction.Move)
			target.position = CTEasing.changeVector3(tweenTime,startValue,_change,duration,easeType);
		else if(tweenAction == CTweenAction.Rotate)
			target.eulerAngles = CTEasing.changeVector3(tweenTime,startValue,_change,duration,easeType);
		else if(tweenAction == CTweenAction.Scale)
			target.localScale = CTEasing.changeVector3(tweenTime,startValue,_change,duration,easeType);
		else if(tweenAction == CTweenAction.Alpha)
		{
			if(_renderer is SpriteRenderer)
			{
				_color = (_renderer as SpriteRenderer).color;
				_color.a = CTEasing.changeValue(tweenTime,startValue.x,_change.x,duration,easeType);
				(_renderer as SpriteRenderer).color = _color;
			}
			else 
			{
				_color = _renderer.sharedMaterial.color;
				_color.a = CTEasing.changeValue(tweenTime,startValue.x,_change.x,duration,easeType);
				_renderer.sharedMaterial.color = _color;
			}
		}
		else if(tweenAction == CTweenAction.Color)
		{
			_change4 = endValue4-startValue4;
			_color.r = CTEasing.changeValue(tweenTime,startValue4.x,_change4.x,duration,easeType);
			_color.g = CTEasing.changeValue(tweenTime,startValue4.y,_change4.y,duration,easeType);
			_color.b = CTEasing.changeValue(tweenTime,startValue4.z,_change4.z,duration,easeType);
			_color.a = CTEasing.changeValue(tweenTime,startValue4.w,_change4.w,duration,easeType);
			
			if(_renderer is SpriteRenderer)
			{
				(_renderer as SpriteRenderer).color = _color;
			}
			else 
			{
				_renderer.sharedMaterial.color = _color;
			}
		}
		else if(tweenAction == CTweenAction.MoveBezier)
		{
			float t = CTEasing.changeValue(tweenTime,0,1,duration,easeType);
			target.position = bezierPath.getPoint(direction == -1 ? 1-t : t);
		}
		else if(tweenAction == CTweenAction.MoveWaypoint)
		{
			float t = CTEasing.changeValue(tweenTime,0,1,duration,easeType);
			target.position = waypointPath.getPoint(direction == -1 ? 1-t : t);
		}
	}
}

public class CTBezierCurve
{
	public Vector3 startP;
	public Vector3 endP;
	public Vector3 controlP1,controlP2;
	public float length;
	public float smoothness = 0.05f;
	public int segments;

	public CTBezierCurve(Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4, float smoothness = 0.05f)
	{
		startP = p1;
		controlP1 = p2;
		controlP2 = p3;
		endP = p4;
		this.smoothness = smoothness;
		segments = (int)(1f/smoothness);

		Vector3 lastV = getPoint(0);
		Vector3 v;
		for(int i = 1; i < segments; i++)
		{
			v = getPoint((float)i/segments);
			length += (lastV-v).magnitude;
			lastV = v;
		}
	}
	
	public Vector3 getPoint(float t)
	{
		//TODO THIS CAN BE IMPROVED
		float a = (1-t);
		float aa = a*a;
		float aaa = aa*a;
		float tt = t*t;
		float ttt = tt*t;
		
		return aaa*startP + 3*aa*t*controlP1 + 3*a*tt*controlP2 + ttt*endP;
	}

	public Vector3[] getAll()
	{
		Vector3[] points = new Vector3[segments+1];
		
		for(int i = 0; i < segments; i++)
		{
			points[i] = getPoint((float)i/segments);
		}
		points[points.Length-1] = getPoint(1f);
		
		return points;
	}
}

public class CTWaypoints
{
	public Vector3[] points;
	public float[] parts;
	public float length;

	public CTWaypoints(Vector3[] points)
	{
		this.points = points;

		if(points.Length < 2)
			Debug.LogError("Waypoint count must be 2 or more");

		parts = new float[points.Length-1];

		for(int i = 0; i < points.Length-1; i++)
			length += (points[i]-points[i+1]).magnitude;

		for(int i = 0; i < points.Length-1; i++)
			parts[i] = (points[i]-points[i+1]).magnitude/length;
	}

	public Vector3 getPoint(float t)
	{
		float sum = 0;
		for(int i = 0; i < points.Length-1; i++){
			
			sum += parts[i];
			if(t < sum)
				return points[i] + (points[i+1]-points[i])*((t+parts[i]-sum) /parts[i]);
		}
		
		return points[points.Length-1];
	}
	
	public Vector3[] getAll()
	{
		int segments = (int)(1f/0.03f);

		Vector3[] points = new Vector3[segments+1];
		
		for(int i = 0; i < segments; i++)
		{
			points[i] = getPoint((float)i/segments);
		}
		points[points.Length-1] = getPoint(1f);
		
		return points;
	}
}

public class CTBezierPath
{
	public Vector3[] controllPoints;
	public CTBezierCurve[] curves;
	public float[] curveParts;
	
	public float length;
	public int curvesCount;
	
	public CTBezierPath(Vector3[] points)
	{
		if(points.Length < 4)
			Debug.LogError("Bezier points count must be 4 or more");
		if(points.Length%4 != 0)
			Debug.LogError("Bezier points must be sets of 4 points");
		
		controllPoints = points;
		curvesCount = points.Length/4;
		curves = new CTBezierCurve[curvesCount];
		curveParts = new float[curvesCount];
		
		for(int i = 0; i < curvesCount; i++)
		{
			curves[i] = new CTBezierCurve(controllPoints[i*4 + 0],
			                              controllPoints[i*4 + 1],
			                              controllPoints[i*4 + 2],
			                              controllPoints[i*4 + 3]);
			length += curves[i].length;
		}

		for(int i = 0; i < curvesCount; i++)
		{
			curveParts[i] = curves[i].length/length;
		}
	}
	
	public Vector3 getPoint(float t)
	{
		float sum = 0;
		for(int i = 0; i < curvesCount; i++){
			
			sum += curveParts[i];
			if(t < sum)
				return curves[i].getPoint((t+curveParts[i]-sum) /curveParts[i]);
		}
		
		return curves[curvesCount-1].getPoint(1f);
	}
}

public enum CTEaseType
{
	Linear,
	EaseInQuad,
	EaseOutQuad
}

public static class CTEasing
{
	public static float changeValue(float t,float b,float c,float d, CTEaseType type)
	{
		if(type == CTEaseType.Linear)	{
			return Linear(t,b,c,d);
		}
		else if(type == CTEaseType.EaseInQuad){
			return EaseInQuad(t,b,c,d);
		}
		else if(type == CTEaseType.EaseOutQuad){
			return EaseOutQuad(t,b,c,d);
		}
		else{
			return 0;
		}
			
	}
	
	public static Vector3 changeVector3(float t,Vector3 b,Vector3 c,float d, CTEaseType type)
	{
		float x,y,z;
		if(type == CTEaseType.Linear){
			x = Linear(t,b.x,c.x,d);
			y = Linear(t,b.y,c.y,d);
			z = Linear(t,b.z,c.z,d);
		}
		else if(type == CTEaseType.EaseInQuad){
			x = EaseInQuad(t,b.x,c.x,d);
			y = EaseInQuad(t,b.y,c.y,d);
			z = EaseInQuad(t,b.z,c.z,d);
		}
		else if(type == CTEaseType.EaseOutQuad){
			x = EaseOutQuad(t,b.x,c.x,d);
			y = EaseOutQuad(t,b.y,c.y,d);
			z = EaseOutQuad(t,b.z,c.z,d);
		}
		else{
			x = 0;
			y = 0;
			z = 0;
		}
		
		return new Vector3(x,y,z);
	}
	
	public static float Linear(float t, float b, float c, float d){
		return c * t/d + b;
	}

	public static float EaseInQuad (float t, float b, float c, float d) {
		return c * (t/=d) * t + b;
	}

	public static float EaseOutQuad (float t, float b, float c, float d) {
		return -c *(t/=d)*(t-2) + b;
	}
}