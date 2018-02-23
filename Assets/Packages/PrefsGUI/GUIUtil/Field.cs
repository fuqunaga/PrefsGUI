using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class GUIUtil
{
	public static T Field<T>(T v, string label = "", params GUILayoutOption[] options) { string s = null; return Field(v, ref s, label, options); }
	public static T Field<T>(T v, ref string unparsedStr, string label, params GUILayoutOption[] options)
	{
		var type = typeof(T);
		T ret = default(T);

		using (var h = new GUILayout.HorizontalScope())
		{
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);

			ret = (T)(_typeFuncTable.ContainsKey(type)
				? _typeFuncTable[type](v, ref unparsedStr, options)
				: ((type.IsEnum)
					? EnumField(v, options)
					: StandardField(v, ref unparsedStr, options)
					)
				);

		}

		return ret;
	}


	#region UnparsedStr Utility
	const char UnparsedStrSeparator = '_';
	static string[] SplitUnparsedStr(string unparsedStr, int elementNum)
	{
		string[] ret = null;
		if (unparsedStr != null)
		{
			ret = unparsedStr.Split(UnparsedStrSeparator);
			Array.Resize(ref ret, elementNum);
		}
		else
		{
			ret = new string[elementNum];
		}
		return ret;
	}
	static string JoinUnparsedStr(string[] strs)
	{
		return string.Join(UnparsedStrSeparator.ToString(), strs);
	}
	#endregion

	#region Field() Implement
	delegate object FieldFunc(object v, ref string unparsedStr, params GUILayoutOption[] options);
	static object FieldFuncBool(object v, ref string unparsedStr, params GUILayoutOption[] options) { return GUILayout.Toggle(Convert.ToBoolean(v), "", options); }
	static object FieldFuncRect(object v, ref string unparsedStr, params GUILayoutOption[] options)
	{
		const int elementNum = 4;
		var strs = SplitUnparsedStr(unparsedStr, elementNum);

		var rect = (Rect)v;
		rect.x = Field(rect.x, ref strs[0], "x", options);
		rect.y = Field(rect.y, ref strs[1], "y", options);
		rect.width = Field(rect.width, ref strs[2], "w", options);
		rect.height = Field(rect.height, ref strs[3], "h", options);

		unparsedStr = JoinUnparsedStr(strs);
		return rect;
	}

	static object FieldFuncVector<T>(object v, ref string unparsedStr, params GUILayoutOption[] options)
	{
		var elementNum = AbstractVector.GetElementNum<T>();
		var strs = SplitUnparsedStr(unparsedStr, elementNum);
		for (var i = 0; i < elementNum; ++i)
		{
			var elem = Field(AbstractVector.GetAtIdx<T>(v, i), ref strs[i], "", options);
			v = AbstractVector.SetAtIdx<T>(v, i, elem);
		}
		unparsedStr = JoinUnparsedStr(strs);
		return v;
	}

	static readonly Dictionary<Type, FieldFunc> _typeFuncTable = new Dictionary<Type, FieldFunc>()
	{
		{typeof(bool),  FieldFuncBool },
		{typeof(Rect), FieldFuncRect },
		{typeof(Vector2), FieldFuncVector<Vector2> },
		{typeof(Vector3), FieldFuncVector<Vector3> },
		{typeof(Vector4), FieldFuncVector<Vector4> },
	};

	class ForcusChecker
	{
		int time;
		int mouseId;
		int keyboardId;
		bool changed;

		public bool IsChanged()
		{
			if (time != Time.frameCount)
			{
				time = Time.frameCount;

				var currentMouse = GUIUtility.hotControl;
				var currentKeyboard = GUIUtility.keyboardControl;

				changed = (keyboardId != currentKeyboard) || (mouseId != currentMouse);
				if (changed)
				{
					keyboardId = currentKeyboard;
					mouseId = currentMouse;
				}
			}

			return changed;
		}
	}

	static ForcusChecker _forcusChecker = new ForcusChecker();

	static object StandardField<T>(T v, ref string unparsedStr, params GUILayoutOption[] options)
	{
		object ret = v;

		var type = typeof(T);

		// validate when unfocused (unparsedStr=null then v.ToString will to be set)）
		if (_forcusChecker.IsChanged())
		{
			unparsedStr = null;
		}

		var hasUnparsedStr = !string.IsNullOrEmpty(unparsedStr);
		var canParse = false;
		try
		{
			canParse = Convert.ChangeType(unparsedStr, type).ToString() == unparsedStr;
		}
		catch (Exception) { }

		var color = (hasUnparsedStr && !canParse) ? Color.red : GUI.color;

		using (var cs = new ColorScope(color))
		{
			unparsedStr = GUILayout.TextField(hasUnparsedStr ? unparsedStr : v.ToString(), options.Concat(new[] { GUILayout.MinWidth(70f) }).ToArray());
			try
			{
				ret = Convert.ChangeType(unparsedStr, type);
				if (ret.ToString() == unparsedStr)
				{
					unparsedStr = null;
				}
			}
			catch (Exception)
			{
			}
		}
		return ret;
	}

	static object EnumField<T>(T v, params GUILayoutOption[] options)
	{
		var type = typeof(T);
		var enumValues = Enum.GetValues(type).OfType<T>().ToList();

		var isFlag = type.GetCustomAttributes(typeof(System.FlagsAttribute), true).Any();
		if (isFlag)
		{
			var flagV = Convert.ToUInt64(v);
			enumValues.ForEach(value =>
			{
				var flag = Convert.ToUInt64(value);
				if (flag > 0)
				{
					var has = (flag & flagV) == flag;
					has = GUILayout.Toggle(has, value.ToString(), options);

					flagV = has ? (flagV | flag) : (flagV & ~flag);
				}
			});

			v = (T)Enum.ToObject(type, flagV);
		}
		else
		{
			var valueNames = enumValues.Select(value => value.ToString()).ToArray();
			var idx = enumValues.IndexOf(v);
			idx = GUILayout.SelectionGrid(
				idx,
				valueNames,
				valueNames.Length);
			v = enumValues.ElementAtOrDefault(idx);
		}
		return v;
	}
	#endregion
}
