using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEPrintBubble : AbstractPopupBubble {

		[Header("PrintBubble fields")]
		public Text thePrintText;

		public void SetPrintPopupValue(Compiler.Variable valueToPrint) {

			thePrintText.text = FormatVariable(valueToPrint);
			ResizeToFit(thePrintText, bubbleRect);
		}

		private static string FormatVariable(Compiler.Variable v) {
			switch (v.variableType) {
				// Escaping to not allow rich text
				case Compiler.VariableTypes.textString: return "<color=#7cbc4f>\"" + EscapeString(v.getString()) + "\"</color>";

				// For ultimate /double/ precision. My eyes are bleeding from this though
				case Compiler.VariableTypes.number: return "<color=#e8a64e>" + v.getNumber().ToString("0." + new string('#',325)) + "</color>";

				case Compiler.VariableTypes.boolean: return "<color=#de5170>" + v.getBool().ToString() + "</color>";
				default: return "<color=#de5170>None</color>";
			}
		}

		private static string EscapeString(string input) {
			return input.Replace('<', '〈').Replace('>', '〉');
		}

		protected override void OnShowMessage() {}

		protected override void OnHideMessage() {
			PMWrapper.UnpauseWalker();
		}
	}

}