using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class IDEFocusLord : MonoBehaviour
	{
		private InputField theInputField;
		private IDETextField theTextField;

		public bool stealFocus = true;
		private Color startSelectionColor;
		private int lastMarkerIndexStart;
		private int lastMarkerIndexEnd;

		public void InitReferences(InputField inputField, IDETextField textField)
		{
			theInputField = inputField;
			theTextField = textField;
			startSelectionColor = inputField.selectionColor;
		}

		void Update()
		{
			if (theInputField.isFocused == false && stealFocus)
			{
				FocusTheField();
			}

			if (theInputField.isFocused && stealFocus)
			{
				lastMarkerIndexStart = theInputField.selectionAnchorPosition;
				lastMarkerIndexEnd = theInputField.selectionFocusPosition;
			}
		}

		#region Force select 

		private void FocusTheField()
		{
			theInputField.selectionColor = new Color(0, 0, 0, 0);
			theInputField.ActivateInputField();
			StartCoroutine(ForceMoveMarker());
		}

		IEnumerator ForceMoveMarker()
		{
			yield return new WaitForEndOfFrame();
			theInputField.selectionAnchorPosition = lastMarkerIndexStart;
			theInputField.selectionFocusPosition = lastMarkerIndexEnd;
			theInputField.selectionColor = startSelectionColor;
		}

		public void SelectEndOfLine(int lineNumber)
		{
			theTextField.SetNewCaretPos(IDEParser.CalcSelectedLineLastIndex(lineNumber, theInputField.text));
		}

		#endregion
	}
}