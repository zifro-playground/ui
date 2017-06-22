using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEFocusLord : MonoBehaviour {

		private InputField theInputField;
		private IDETextField theTextField;

		public bool stealFocus = true;
		private Color startSelectionColor;
		private int lastMarerIndexStart;
		private int lastMarerIndexEnd;

		public void initReferences(InputField theInputField, IDETextField theTextField) {
			this.theInputField = theInputField;
			this.theTextField = theTextField;
			startSelectionColor = theInputField.selectionColor;
		}

		void Update() {
			if (theInputField.isFocused == false && stealFocus)
				focusTheField();

			if (theInputField.isFocused && stealFocus) {
				lastMarerIndexStart = theInputField.selectionAnchorPosition;
				lastMarerIndexEnd = theInputField.selectionFocusPosition;
			}
		}

		#region Force select 
		private void focusTheField() {
			theInputField.selectionColor = new Color(0, 0, 0, 0);
			theInputField.ActivateInputField();
			StartCoroutine(forceMoveMarker());
		}

		IEnumerator forceMoveMarker() {
			yield return new WaitForEndOfFrame();
			theInputField.selectionAnchorPosition = lastMarerIndexStart;
			theInputField.selectionFocusPosition = lastMarerIndexEnd;
			theInputField.selectionColor = startSelectionColor;
		}

		public void selectEndOfLine(int lineNumber) {
			theTextField.setNewCaretPos(IDEPARSER.calcSelectedLineLastIndex(lineNumber, theInputField.text));
		}
		#endregion



	}

}