using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class GUIUtil
{
	public static float Slider(float v, string label = "") { return Slider(v, 0f, 1f, label); }
	public static float Slider(float v, ref string unparsedStr, string label = "") { return Slider(v, 0f, 1f, ref unparsedStr, label); }

	public static T Slider<T>(T v, T min, T max, string label = "", string[] elementLabels = null)
	{
		string unparsedStr = null;
		return Slider(v, min, max, ref unparsedStr, label, elementLabels);
	}

	public static T Slider<T>(T v, T min, T max, ref string unparsedStr, string label = "", string[] elementLabels = null)
	{
		return (T)_typeSliderFuncTable[typeof(T)](v, min, max, ref unparsedStr, label, elementLabels);
	}

	#region Slider() Implement
	delegate object SliderFunc(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null);

	public static object SliderInt(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
	{
		return Mathf.FloorToInt((float)SliderFloat((float)(int)v, (float)(int)min, (float)(int)max, ref unparsedStr, label));
	}

	public static object SliderFloat(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
	{
		float ret = default(float);
		using (var h = new GUILayout.HorizontalScope())
		{
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, GUILayout.ExpandWidth(false));
			ret = GUILayout.HorizontalSlider((float)v, (float)min, (float)max, GUILayout.MinWidth(200));
			ret = (float)StandardField(ret, ref unparsedStr, GUILayout.MaxWidth(100f));
		}

		return ret;
	}

	static readonly string[] defaultElemLabelsRect = new[] { "x", "y", "w", "h" };
	static object SliderFuncRect(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
	{
		const int elementNum = 4;
		var eLabels = elemLabels ?? defaultElemLabelsRect;

		using (var h0 = new GUILayout.HorizontalScope())
		{
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
			using (var vertical = new GUILayout.VerticalScope())
			{
				var strs = SplitUnparsedStr(unparsedStr, elementNum);
				var rect = (Rect)v;
				var rectMin = (Rect)min;
				var rectMax = (Rect)max;

				rect.x = Slider(rect.x, rectMin.x, rectMax.x, ref strs[0], eLabels[0]);
				rect.y = Slider(rect.y, rectMin.y, rectMax.y, ref strs[1], eLabels[1]);
				rect.width = Slider(rect.width, rectMin.width, rectMax.width, ref strs[2], eLabels[2]);
				rect.height = Slider(rect.height, rectMin.height, rectMax.height, ref strs[3], eLabels[3]);

				v = rect;

				unparsedStr = JoinUnparsedStr(strs);
			}
		}

		return v;
	}


	static readonly string[] defaultElemLabelsVector = new[] { "x", "y", "z", "w" };
	static object SliderFuncVector<T>(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
	{
		var elementNum = AbstractVector.GetElementNum<T>();
		var eLabels = elemLabels ?? defaultElemLabelsVector;

		using (var h0 = new GUILayout.HorizontalScope())
		{
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
			using (var vertical = new GUILayout.VerticalScope())
			{
				var strs = SplitUnparsedStr(unparsedStr, elementNum);
				for (var i = 0; i < elementNum; ++i)
				{
					using (var h1 = new GUILayout.HorizontalScope())
					{
						var elem = Slider(AbstractVector.GetAtIdx<T>(v, i), AbstractVector.GetAtIdx<T>(min, i), AbstractVector.GetAtIdx<T>(max, i), ref strs[i], eLabels[i]);
						v = AbstractVector.SetAtIdx<T>(v, i, elem);
					}
				}
				unparsedStr = JoinUnparsedStr(strs);
			}
		}

		return v;
	}

	static readonly Dictionary<Type, SliderFunc> _typeSliderFuncTable = new Dictionary<Type, SliderFunc>()
	{
		{typeof(int), SliderInt },
		{typeof(float), SliderFloat },
		{typeof(Rect), SliderFuncRect },
		{typeof(Vector2), SliderFuncVector<Vector2> },
		{typeof(Vector3), SliderFuncVector<Vector3> },
		{typeof(Vector4), SliderFuncVector<Vector4> },
	};

	#endregion
}
