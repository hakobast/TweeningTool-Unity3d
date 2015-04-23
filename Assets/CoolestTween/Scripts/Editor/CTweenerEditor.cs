using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(CBaseTweener),true)]
public class CTweenerEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		CBaseTweener myTarget = target as CBaseTweener;
	
		GUILayout.BeginHorizontal();

		SetLabelWidth(48);
		if(myTarget is CTweenPosition)
		{
			(myTarget as CTweenPosition).toPosition = EditorGUILayout.Vector3Field("To",(myTarget as CTweenPosition).toPosition);
			Transform tr = (Transform)EditorGUILayout.ObjectField(null,typeof(Transform),GUILayout.Width(64));
			if(tr != null)
				(myTarget as CTweenPosition).toPosition = tr.position;
		}
		else if(myTarget is CTweenRotation)
		{
			(myTarget as CTweenRotation).toRotation = EditorGUILayout.Vector3Field("To",(myTarget as CTweenRotation).toRotation);
		}
		else if(myTarget is CTweenScale)
		{
			(myTarget as CTweenScale).toScale = EditorGUILayout.Vector3Field("To",(myTarget as CTweenScale).toScale);
		}
		else if(myTarget is CTweenColor)
		{
			(myTarget as CTweenColor).toColor = EditorGUILayout.ColorField("To",(myTarget as CTweenColor).toColor);
		}
		
		if(GUILayout.Button("Switch",GUILayout.Width(64)))
		{
			if(myTarget is CTweenPosition)
			{
				Vector3 temp = myTarget.transform.position;
				myTarget.transform.position = (myTarget as CTweenPosition).toPosition;
				(myTarget as CTweenPosition).toPosition = temp;
			}
			else if(myTarget is CTweenRotation)
			{
				Vector3 temp = myTarget.transform.eulerAngles;
				myTarget.transform.eulerAngles = (myTarget as CTweenRotation).toRotation;
				(myTarget as CTweenRotation).toRotation = temp;
			}
			else if(myTarget is CTweenScale)
			{
				Vector3 temp = myTarget.transform.localScale;
				myTarget.transform.localScale = (myTarget as CTweenScale).toScale;
				(myTarget as CTweenScale).toScale = temp;
			}
			else if(myTarget is CTweenColor)
			{
				Color temp;
				if(myTarget.renderer is SpriteRenderer)
				{
					temp = (myTarget.renderer as SpriteRenderer).color;
					(myTarget.renderer as SpriteRenderer).color = (myTarget as CTweenColor).toColor;
				}
				else
				{
					temp = myTarget.renderer.sharedMaterial.color;
					myTarget.renderer.sharedMaterial.color = (myTarget as CTweenColor).toColor;
				}
				(myTarget as CTweenColor).toColor = temp;
			}
		}

		GUILayout.EndHorizontal();

		if (DrawHeader("Tween"))
		{
			BeginContents();
			SetLabelWidth(110f);
			
			GUILayout.BeginHorizontal();
			myTarget.duration = EditorGUILayout.FloatField("Duration", myTarget.duration, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			myTarget.delay = EditorGUILayout.FloatField("Delay", myTarget.delay, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			myTarget.playOnAwake = EditorGUILayout.Toggle("PlayOnAwake", myTarget.playOnAwake);

			myTarget.type = (CTweenType)EditorGUILayout.EnumPopup("Tween type",myTarget.type,GUILayout.Width(270f));
			myTarget.easeType = (CTEaseType)EditorGUILayout.EnumPopup("Ease type",myTarget.easeType,GUILayout.Width(270f));
			
			if(myTarget.type == CTweenType.Loop || myTarget.type == CTweenType.PingPong)
			{
				GUILayout.BeginHorizontal();
				myTarget.repeatCount = EditorGUILayout.IntField("repeats", myTarget.repeatCount,GUILayout.Width(170f));
				if(myTarget.repeatCount == 0)
					GUILayout.Label("0 = infinity");
				GUILayout.EndHorizontal();
			}

			EndContents();
		}

		if(myTarget.tweenController != null)
		{
			ProgressBar(myTarget.tweenController.tweenTime/myTarget.duration);
		}
		else
			ProgressBar(0);

		if(myTarget is CTweenColor)
		{
			EditorGUILayout.HelpBox("The progress bar is not working corectly", MessageType.Info);
		}

		if (DrawHeader("Callbacks"))
		{
			myTarget.completeAction = EditorGUILayout.TextField("Call when finished",myTarget.completeAction);
			myTarget.completeHandler = (MonoBehaviour)EditorGUILayout.ObjectField("Target",myTarget.completeHandler,typeof(MonoBehaviour));
		}
	}

	static bool mEndHorizontal = false;
	static public void BeginContents (bool minimalistic = true)
	{
		if (!minimalistic)
		{
			mEndHorizontal = true;
			GUILayout.BeginHorizontal();
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
		}
		else
		{
			mEndHorizontal = false;
			EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
			GUILayout.Space(10f);
		}
		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}

	static public void SetLabelWidth (float width)
	{
		EditorGUIUtility.labelWidth = width;
	}

	static public void EndContents ()
	{
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (mEndHorizontal)
		{
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(3f);
	}

	static public bool DrawHeader (string text) { return DrawHeader(text, text, false, true); }

	static public bool DrawHeader (string text, string key, bool forceOn, bool minimalistic)
	{
		bool state = EditorPrefs.GetBool(key, true);
		
		if (!minimalistic) GUILayout.Space(3f);
		if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;
		
		if (minimalistic)
		{
			if (state) text = "\u25BC" + (char)0x200a + text;
			else text = "\u25BA" + (char)0x200a + text;
			
			GUILayout.BeginHorizontal();
			GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
			if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();
		}
		else
		{
			text = "<b><size=11>" + text + "</size></b>";
			if (state) text = "\u25BC " + text;
			else text = "\u25BA " + text;
			if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
		}
		
		if (GUI.changed) EditorPrefs.SetBool(key, state);
		
		if (!minimalistic) GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state) GUILayout.Space(3f);
		return state;
	}

	void ProgressBar (float value) {
		// Get a rect for the progress bar using the same margins as a textfield:
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");

		float v = Mathf.Clamp01(value/1f)*100f;
		EditorGUI.ProgressBar (rect, value, v.ToString("0") + "%");
		EditorGUILayout.Space ();
	}
}
