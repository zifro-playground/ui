using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Compiler;
using System;

namespace PM {

	public class VariableWindow : MonoBehaviour, IPMLevelChanged {

		[SerializeField]
		RectTransform maskRect;
		[SerializeField]
		RectTransform contentRect;

		public InWindowVariable smallVarPref, medVarPref, bigVarPref;

		/* SARA
		public GameObject seperatorPrefab;
		private float variableYPos = 0;
		private float objectSpacing = 0;
		*/

		//private float maskHeight = 100;
		private List<InWindowObject> currentVariables = new List<InWindowObject>();


		public void resetList() {
			foreach (InWindowObject v in currentVariables)
				v.remove();

			currentVariables.Clear();
		}

		public void addVariable(Variable newVariable) {
			InWindowVariable clone = Instantiate (calcVarSize(newVariable));
			clone.transform.SetParent(contentRect.transform, worldPositionStays: false);
			clone.initVariable(newVariable);
			currentVariables.Add(clone);
		}

		private InWindowVariable calcVarSize(Variable newVariable) {
			string longestString = newVariable.name.Length > getValueString (newVariable).Length ? newVariable.name : getValueString (newVariable);

			if (longestString.Length <= smallVarPref.maxChars)
				return smallVarPref;
			if (longestString.Length <= medVarPref.maxChars)
				return medVarPref;

			return bigVarPref;
		}

		private string getValueString(Variable newVariable) {
			switch (newVariable.variableType) {
				case VariableTypes.boolean:
					return newVariable.getBool().ToString();

				case VariableTypes.number:
					return newVariable.getNumber().ToString();

				case VariableTypes.textString:
					return "\"" + newVariable.getString().ToString() + "\"";

				default:
					return "None";
			}
		}

		/* SARA
		#region update list
		public void updateWindow() {
			scaleList();
			setVariablePositions();
		}
		private void scaleList() {
			/*
			float widthSum = 0;
			foreach (InWindowObject o in currentVariables)
				widthSum += o.width;

			float xContetSize = widthSum + (currentVariables.Count-1) * objectSpacing;
			contentRect.sizeDelta = new Vector2(xContetSize , maskHeight);
			/*
		}
		private void setVariablePositions() {
			if (currentVariables.Count == 0)
				return;

			float startPos = contentRect.rect.width * 0.5f;
			float currentStep = 0;

			for (int i = 0; i < currentVariables.Count; i++) {
				currentStep -= currentVariables[i].width * 0.5f;

				if (currentStep > contentRect.rect.width)
					break;

				currentVariables[i].transform.localPosition = new Vector2(startPos + currentStep, variableYPos);
				currentStep -= currentVariables[i].width * 0.5f + objectSpacing;
			}
		}
		#endregion
		*/

		void IPMLevelChanged.OnPMLevelChanged() {
			resetList();
		}

	}

}