using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mellis.Core.Interfaces;
using Mellis.Lang.Base.Entities;

namespace PM
{
	public class VariableWindow : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
	{
		public RectTransform contentRect;

		public GameObject theWindow;

		public VariableInWindow variablePrefab;

		[Header("For variable width calculation")]
		public float minVariableWidth = 80;

		public float maxVariableWidth = 240;
		public float characterWidth = 10;
		public float paddingWidth = 15;

		[Header("Color settings")]
		public Color nameTextColor = new Color(0.37f, 0.38f, 0.36f);

		public Color boolText = new Color(0.87f, 0.031f, 0.44f);
		public Color stringText = new Color(0.49f, 0.74f, 0.3f);
		public Color numberText = new Color(0.9f, 0.65f, 0.3f);
		public Color nullText = Color.red;

		private readonly List<VariableInWindow> currentVariables = new List<VariableInWindow>();

		public void UpdateList(IProcessor processor)
		{
			foreach (VariableInWindow v in currentVariables)
			{
				v.Remove();
			}

			currentVariables.Clear();

			foreach (IScriptType variable in processor.CurrentScope.Variables.Values)
			{
				VariableInWindow clone = Instantiate(variablePrefab, contentRect.transform, false);

				string valueString = variable.ToString();

				float width = CalcVariableWidth(Mathf.Max(variable.Name.Length, valueString.Length));
				int maxChars = CalcNoCharacters(width);
				clone.SetWidth(width);

				ToStringAndCompressVariable(variable, valueString, maxChars, out valueString, out Color valueColor);
				clone.InitVariable(variable.Name, nameTextColor, valueString, valueColor);

				currentVariables.Add(clone);
			}

			theWindow.SetActive(true);
		}

		public void ResetList()
		{
			foreach (VariableInWindow v in currentVariables)
			{
				v.Remove();
			}

			currentVariables.Clear();

			theWindow.SetActive(false);
		}

		private float CalcVariableWidth(int maxCharCount)
		{
			float variableWidth = (maxCharCount + 1) * characterWidth + 2 * paddingWidth;

			return Mathf.Clamp(variableWidth, minVariableWidth, maxVariableWidth);
		}

		private int CalcNoCharacters(float variableWidth)
		{
			return (int)Mathf.Floor((variableWidth - 2 * paddingWidth) / characterWidth) - 1;
		}

		void IPMLevelChanged.OnPMLevelChanged()
		{
			ResetList();
		}

		public void OnPMCompilerStarted()
		{
			ResetList();
		}

		void ToStringAndCompressVariable(IScriptType variable, string s, int maxChars, out string text, out Color color)
		{
			switch (variable)
			{
			case BooleanBase _:
				color = boolText;
				text = CompressString(s, maxChars);
				break;

			case IntegerBase _:
			case DoubleBase _:
				color = numberText;
				text = CompressString(s, maxChars, isNumberValue: true);
				break;

			case StringBase _:
				color = stringText;
				text = CompressString(s, maxChars, isStringValue: true);
				break;

			case NullBase _:
				color = nullText;
				text = variable.ToString();
				break;

			default:
				color = nullText;
				text = IDEColorCoding.RunColorCode(CompressString(s, maxChars));
				break;
			}
		}

		private static string CompressString(
			string s,
			int maxChars,
			bool isNumberValue = false,
			bool isStringValue = false)
		{
			if (s.Length <= maxChars)
			{
				return s;
			}

			if (isStringValue)
			{
				char quote = s[s.Length - 1];
				return s.Substring(0, maxChars - 4) + "..." + quote;
			}

			if (isNumberValue)
			{
				return s.Substring(0, maxChars - 9) + "..." + s.Substring(s.Length - 4, 4);
			}

			return s.Substring(0, maxChars - 3) + "...";
		}
	}
}