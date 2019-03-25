using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PM {

	public class VariableWindow : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted {

		[SerializeField]
		RectTransform contentRect;

		public GameObject theWindow;

		[Header("For variable width calculation")]
		public float minVariableWidth = 80;
		public float maxVariableWidth = 240;
		public float characterWidth = 10;
		public float paddingWidth = 15;

		public VariableInWindow variablePrefab;

		private List<VariableInWindow> currentVariables = new List<VariableInWindow>();

		public void resetList() {
			foreach (VariableInWindow v in currentVariables)
				v.remove();
			currentVariables.Clear();

			theWindow.SetActive(false);
		}

		// TODO
		//public void addVariable(Variable newVariable) {
		public void addVariable(object newVariable) {
			if (currentVariables.Count == 0)
				theWindow.SetActive(true);

			VariableInWindow clone = Instantiate (variablePrefab);
			clone.transform.SetParent (contentRect.transform, false);

			// TODO
			//float width = calcVariableWidth (newVariable);
			//clone.setWidth(width);
			//clone.maxChars = calcNoCharacters(width);

			clone.initVariable(newVariable);

			currentVariables.Add(clone);

		}

		// TODO
		//private float calcVariableWidth(Variable newVariable) {
		private float calcVariableWidth(object newVariable) {
			
			//int nameLength = newVariable.name.Length;
			//int valueLength = getValueString (newVariable).Length;
			//int maxLength = Mathf.Max (nameLength, valueLength);

			//float variableWidth = (maxLength + 1) * characterWidth + 2 * paddingWidth;

			//return Mathf.Clamp (variableWidth, minVariableWidth, maxVariableWidth);
			return 0;
		}

		private int calcNoCharacters(float variableWidth){
			return (int) Mathf.Floor((variableWidth - 2 * paddingWidth) / characterWidth) - 1;
		}

		// TODO
		//private string getValueString(Variable newVariable) {
		private string getValueString(object newVariable) {
			//switch (newVariable.variableType) {
			//	case VariableTypes.boolean:
			//		return newVariable.getBool().ToString();

			//	case VariableTypes.number:
			//		return newVariable.getNumber().ToString();

			//	case VariableTypes.textString:
			//		return "\"" + newVariable.getString().ToString() + "\"";

			//	default:
			//		return "None";
			//}
			return "N/A";
		}

		void IPMLevelChanged.OnPMLevelChanged() {
			resetList();
		}

		public void OnPMCompilerStarted()
		{
			resetList();
		}
	}

}