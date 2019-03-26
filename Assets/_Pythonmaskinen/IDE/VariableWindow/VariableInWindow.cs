using System.Collections;
using System.Collections.Generic;
using Mellis.Core.Interfaces;
using Mellis.Lang.Base.Entities;
using PM;
using UnityEngine;
using UnityEngine.UI;

public class VariableInWindow : MonoBehaviour
{
	public Text nameText;
	public Text valueText;

	#region Colors

	#endregion

	public void InitVariable(string varName, Color nameColor, string value, Color valueColor)
	{
		nameText.text = varName;
		nameText.color = nameColor;

		valueText.text = value;
		valueText.color = valueColor;
	}

	public void Remove()
	{
		Destroy(gameObject);
	}

	public void SetWidth(float width)
	{
		RectTransform rect = GetComponent<RectTransform>();

		Vector2 sizeDelta = rect.sizeDelta;
		sizeDelta = new Vector2(width, sizeDelta.y);

		rect.sizeDelta = sizeDelta;
	}

}