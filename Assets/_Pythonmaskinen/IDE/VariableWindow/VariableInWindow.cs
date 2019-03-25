using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableInWindow : MonoBehaviour {

	public int maxChars = 16;
	public Text nameText;
	public Text valueText;
	// TODO
	//private Variable targetVariable;
	private object targetVariable;
	private const string nullValue = "None";

	#region
	private Color nameTextColor = new Color(0.37f, 0.38f, 0.36f),
	boolText = new Color(0.87f, 0.031f, 0.44f),
	stringText = new Color(0.49f, 0.74f, 0.3f),
	numberText = new Color(0.9f, 0.65f, 0.3f),
	nullText = Color.red;
	#endregion Color Coding


	// TODO
	//public void initVariable(Variable targetVariable) {
	public void initVariable<T>(T targetVariable) {
		this.targetVariable = targetVariable;
		setVariableAttributes();
	}

	private void setVariableAttributes() {
		// TODO
		//nameText.text = targetVariable.name;
		nameText.color = nameTextColor;

		// TODO
  //      switch (targetVariable.variableType) {

		//case VariableTypes.boolean:
		//	valueText.color = boolText;
		//	valueText.text = compressString(targetVariable.getBool().ToString());
		//	break;

		//case VariableTypes.number:
		//	valueText.color = numberText;
		//	valueText.text = compressString(targetVariable.getNumber().ToString(), isNumberValue: true);
		//	break;

		//case VariableTypes.textString:
		//	valueText.color = stringText;
		//	valueText.text = compressString(targetVariable.getString(), isStringValue: true);
		//	break;

		//default:
		//	valueText.color = nullText;
		//	valueText.text = nullValue;
		//	break;
		//}
	}

	private string compressString(string s, bool isStringValue = false, bool isNumberValue = false) {
		if (isStringValue)
			s = '"' + s + '"';

		if (s.Length <= maxChars)
			return s;


		if (isStringValue)
			return s.Substring(0, maxChars - 4) + "...\"";
		else if (isNumberValue)
			return s.Substring(0, maxChars - 9) + "... " + s.Substring(s.Length - 4, 4);
		else
			return s.Substring(0, maxChars - 3) + "...";
	}

	public void remove() {
		Destroy(gameObject);
	}

	public void setWidth(float width) {
		RectTransform rect = GetComponent<RectTransform> ();

		Vector2 sizeDelta = rect.sizeDelta;
		sizeDelta = new Vector2(width, sizeDelta.y);

		rect.sizeDelta = sizeDelta;
	}
}