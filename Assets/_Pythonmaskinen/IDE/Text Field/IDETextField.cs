using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace PM {

	public class IDETextField : MonoBehaviour, IPMCompilerStarted, IPMCompilerStopped {

		public RectTransform contentContainer;
		public IDEFocusLord theFocusLord;
		public InputField theInputField;
		public IDELineMarker theLineMarker;
		public IDEScrollLord theScrollLord;
		public Text preText;
		public Text visibleText;
		public Text postText;
		public IDESpeciallCommands theSpeciallCommands;
		public int codeRowsLimit = 32;
		public bool devBuild = false;

		[NonSerialized]
		public string preCode = string.Empty;
		[NonSerialized]
		public string postCode = string.Empty;

		[NonSerialized]
		public RectTransform inputRect;
		//private float rowHeight = 27f;
		//private float maskHeight = 500;
		private int maxChars = 53;

		private string lastText = "";
		private float startYPos;
		private bool settingNewCaretPos = false;
		//private int oldCaretPos = 0;
		private bool inserting = false;

		public int rowLimit {
			get { return codeRowsLimit; } 
			set { 
				codeRowsLimit = value;
				IDETextManipulation.validateText (theInputField.text, codeRowsLimit, maxChars);
			}
		}

		//private int preErrorCaretIndex = 0;
		private List<IndentLevel> toAddLevels = new List<IndentLevel> ();

		void Start() {
			theLineMarker.initLineMarker(this, theFocusLord);

			inputRect = theInputField.GetComponent<RectTransform>();

			startYPos = inputRect.anchoredPosition.y;
			InitTextFields();
			
			theInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar, charIndex); };
			theInputField.onValueChanged.AddListener(_ => doValidateInput());

			theFocusLord.initReferences(theInputField, this);
			//Camera.main.GetComponent<HelloCompiler> ().startStopCompilerEvent (deActivateField);

			// poor mans reActivate
			theFocusLord.stealFocus = true;
			theInputField.interactable = true;

			ColorCodeDaCode();
			FocusCursor();
		}

		public void InitTextFields() {
			preText.text = IDEColorCoding.runColorCode(preCode);

			float preHeight = CalcAndSetPreCode();
			inputRect.anchoredPosition = new Vector2(inputRect.anchoredPosition.x, startYPos - preHeight);
		}

		void Update() {
#if UNITY_WEBGL
			webTabSupport();
#elif UNITY_EDITOR
			if (devBuild)
				webTabSupport();
#endif

			if (Input.inputString.Length > 0 
			|| IDESpeciallCommands.AnyKey(
				KeyCode.UpArrow, 
				KeyCode.DownArrow, 
				KeyCode.LeftArrow, 
				KeyCode.RightArrow, 
				KeyCode.PageDown, 
				KeyCode.PageUp, 
				KeyCode.Home, 
				KeyCode.End
			)) {
				FocusCursor();
			}
		}

		void LateUpdate() {
			if (inserting) {
				toAddLevels.Clear();
				inserting = false;
			}

			if (toAddLevels.Count > 0)
				addAutoIndent();

			if (settingNewCaretPos)
				settingNewCaretPos = false;

			theInputField.text = theSpeciallCommands.checkHistoryCommands(theInputField.text);
		}
		
		/// <summary>Updates main code syntax highlighting and size of rects (for scrolling)</summary>
		public void ColorCodeDaCode() {
			visibleText.text = IDEColorCoding.runColorCode(theInputField.text);
			inputRect.sizeDelta = new Vector2(inputRect.sizeDelta.x, visibleText.preferredHeight + 6);
			contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, inputRect.sizeDelta.y - inputRect.anchoredPosition.y);
		}
		
		public void MoveLineMarker() {
			if (PMWrapper.isDemoingLevel) {
				IDELineMarker.instance.SetState(IDELineMarker.State.Hidden);
			} else {
				int preRows = (string.IsNullOrEmpty(preCode) ? 0 : preCode.Split('\n').Length) + 1;
				int selected = preRows + IDEPARSER.calcCurrentSelectedLine(theInputField.caretPosition, theInputField.text);
				IDELineMarker.SetIDEPosition(selected);
			}
		}

		public float DetermineYOffset(int lineNumber) {
			float topY, lineHeight;
			DetermineYOffset(lineNumber, out topY, out lineHeight);
			return topY;
		}

		public void DetermineYOffset(int lineNumber, out float topY, out float lineHeight) {
			// Determine if we're calculating preCode line or mainCode line
			int preCodeLines = preCode.Length == 0 ? 0 : preCode.Split('\n').Length;
			bool isPre = lineNumber < preCodeLines;

			// lineIndex of the textfield, either preCodeField or mainCodeField
			int lineIndex = isPre ? lineNumber : lineNumber - preCodeLines;
			var gen = isPre ? preText.cachedTextGenerator : theInputField.textComponent.cachedTextGenerator;
			
			if (gen.lineCount == 0) {
				topY = (isPre ? preText : theInputField.textComponent).rectTransform.anchoredPosition.y;
				lineHeight = 0;
				return;
			}

			// Get topmost Y position of line
			topY = gen.lines[Mathf.Clamp(lineIndex, 0, gen.lineCount-1)].topY;

			// topY is in pixels. We need units, therefore div by ppu.
			// +1.4 is just a small offset to make it look better
			if (isPre) {
				topY = (topY/ preText.pixelsPerUnit + preText.rectTransform.anchoredPosition.y) + 1.4f;
				lineHeight = gen.lines[0].height / theInputField.textComponent.pixelsPerUnit;
			} else {
				topY = (topY / theInputField.textComponent.pixelsPerUnit + theInputField.textComponent.rectTransform.anchoredPosition.y)+1.4f;
				lineHeight = gen.lines[0].height*1.5f / theInputField.textComponent.pixelsPerUnit;
			}

		}

		/// <summary>If has precode, returns the height of the precode text component, else return 0.</summary>
		private float CalcAndSetPreCode() {
			if (preCode.Length == 0) {
				preText.gameObject.SetActive(false);
				return 0;
			} else {
				preText.gameObject.SetActive(true);
				float height = preText.preferredHeight + 8;
				preText.rectTransform.sizeDelta = new Vector2(preText.rectTransform.sizeDelta.x, height);
				return height;
			}
		}

		#region Text Input Manipulation
		private int lastCharInsertIndex = 0;
		private char MyValidate(char c, int charIndex) {
			
			if (inserting)
				return c;

			lastCharInsertIndex = charIndex;

			return IDETextManipulation.MyValidate(c, charIndex, isPasting(), toAddLevels, theInputField.text, this);
		}

		private bool isPasting() {
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) ||
				Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.V))
				return true;

			return false;
		}

		private void addAutoIndent() {
			string startString = theInputField.text;
			foreach (IndentLevel l in toAddLevels) {
				startString = startString.Insert(l.caretPos, l.indentText);
			}

			if (IDETextManipulation.validateText(startString, codeRowsLimit, maxChars)) {
				inserting = true;
				theInputField.text = startString;
			}

			toAddLevels.Clear();
		}

		//Cheks if the current input will fit into the speciefied borders!
		//Restricted by amount of rows in Y
		//and by maxChars in X
		private void doValidateInput() {
			if (IDETextManipulation.validateText(theInputField.text, codeRowsLimit, maxChars)) {
				lastText = theInputField.text;
				ColorCodeDaCode();
			} else {
				inserting = true;
				theInputField.text = lastText;
				theInputField.caretPosition = lastCharInsertIndex;
			}

			FocusCursor();

			// Save
			SaveData.SaveMainCode();

			theLineMarker.removeErrorMessage();
		}
		#endregion

		#region caret & scroll position
		public void setNewCaretPos(int newPos) {
			if (settingNewCaretPos)
				return;

			//oldCaretPos = newPos;
			StopCoroutine(setCaretPos(newPos));
			StartCoroutine(setCaretPos(newPos));
			settingNewCaretPos = true;
		}

		private IEnumerator setCaretPos(int newPos) {
			Color preColor = theInputField.caretColor;
			theInputField.caretColor = Vector4.zero;
			yield return new WaitForEndOfFrame();
			theInputField.caretPosition = newPos;
			theInputField.caretColor = preColor;
		}

		public void FocusCursor() {
			Canvas.ForceUpdateCanvases();
			int preCodeLines = preCode.Length == 0 ? 0 : preCode.Split('\n').Length;
			theScrollLord.FocusOnLineNumber(IDEPARSER.calcCurrentSelectedLine(theInputField.selectionAnchorPosition, theInputField.text) + preCodeLines);
			MoveLineMarker();
		}
		#endregion

		#region insert extraText

		public void InsertText(string newText, bool smartButtony = false) {
			if (theInputField.isFocused) {
				// If selection, replace selection
				// Else, just insert
				int start = Mathf.Min(theInputField.selectionAnchorPosition, theInputField.selectionFocusPosition);
				int end = Mathf.Max(theInputField.selectionAnchorPosition, theInputField.selectionFocusPosition);
				string checkText;

				if (start == end) 
					// No selection
					checkText = theInputField.text.Insert (start, newText);
				else
					// Yes selection
					checkText = theInputField.text.Substring(0, start) + newText + theInputField.text.Substring(end);

				// Try adding extra newline if it's the end of the line
				string checkText2 = null;
				string after = checkText.Substring(start + newText.Length);
				if (smartButtony && (after.Length == 0 || after[0] == '\n')) {
					int indent = IDEPARSER.getIndentLevel(start, checkText);

					// Add newline directly
					if (start > 0 && checkText[start-1] == ':') {
						checkText = checkText.Insert(start, "\n" + new string('\t', indent));
						start += 1 + indent;
					}
					
					checkText2 = checkText.Insert(start + newText.Length, "\n" + new string('\t', indent));
					start += 1 + indent;
				}

				// Check if it fits
				if (checkText2 == null || !IDETextManipulation.validateText(checkText2, codeRowsLimit, maxChars)) {
					checkText2 = null;
					if (!IDETextManipulation.validateText(checkText, codeRowsLimit, maxChars))
						checkText = null;
				}

				// One of them succeded
				if (checkText2 != null || checkText != null) {
					inserting = true;

					setNewCaretPos(start + newText.Length);
					theInputField.text = checkText2 ?? checkText;
				}

			}
		}

		public void insertMainCodeAtStart (string code){
			if (theInputField.text.Length == 0) {
				theInputField.text = code;
				inserting = true;
			}
		}

		private void webTabSupport() {
			if (Input.GetKeyDown(KeyCode.Tab))
				InsertText("\t");
		}


		public void clearText() {
			inserting = true;
			theInputField.text = "";
			toAddLevels.Clear();
			setCaretPos(0);
		}
		#endregion

		#region public Get/Setters
		public void deActivateField() {
			this.enabled = false;
			theFocusLord.stealFocus = false;
			theInputField.interactable = false;
		}

		public void reActivateField() {
			this.enabled = true;
			theFocusLord.stealFocus = true;
			theInputField.interactable = true;
		}
		#endregion

		void IPMCompilerStarted.OnPMCompilerStarted() {
			if (!PMWrapper.isDemoingLevel)
				deActivateField();
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status) {
			if (!PMWrapper.isDemoingLevel)
				reActivateField();
		}
	}

}