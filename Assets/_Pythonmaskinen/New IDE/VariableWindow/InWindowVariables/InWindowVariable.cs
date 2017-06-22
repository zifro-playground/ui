using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Compiler;

namespace PM {

	public abstract class InWindowVariable : InWindowObject {

		public int maxChars = 16;
		public Text nameText;
		public Text valueText;
		private Variable targetVariable;
		private const string nullValue = "None";

		#region
		private Color boolText = new Color(0.87f, 0.031f, 0.44f),
			stringText = new Color(0.49f, 0.74f, 0.3f),
			numberText = new Color(0.9f, 0.65f, 0.3f),
			nullText = Color.red;
		#endregion Color Coding


		public void initVariable(Variable targetVariable) {
			this.targetVariable = targetVariable;
			setVariableAttributes();

			// Add tooltip
			var tooltip = gameObject.AddComponent<VarTooltip>();
			tooltip.header = targetVariable.name + ":";
			tooltipContent(out tooltip.text, out tooltip.textColor);
			tooltip.ApplyTooltipTextChange();
		}

		private void tooltipContent(out string text, out Color color) {
			switch (targetVariable.variableType) {
				case VariableTypes.boolean:
					color = boolText;
					text = targetVariable.getBool() ? "True" : "False";
					break;

				case VariableTypes.number:
					color = numberText;
					text = targetVariable.getNumber().ToString();
					break;

				case VariableTypes.textString:
					color = stringText;
					text = '"' + targetVariable.getString() + '"';
					break;

				default:
					color = nullText;
					text = nullValue;
					break;
			}
		}

		private void setVariableAttributes() {
			nameText.text = compressString(targetVariable.name + ":");

			switch (targetVariable.variableType) {

				case VariableTypes.boolean:
					valueText.color = boolText;
					valueText.text = compressString(targetVariable.getBool().ToString());
					break;

				case VariableTypes.number:
					valueText.color = numberText;
					valueText.text = compressString(targetVariable.getNumber().ToString(), isNumberValue: true);
					break;

				case VariableTypes.textString:
					valueText.color = stringText;
					valueText.text = compressString(targetVariable.getString(), isStringValue: true);
					break;

				default:
					valueText.color = nullText;
					valueText.text = nullValue;
					break;
			}
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

	}

}