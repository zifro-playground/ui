using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.Serialization;

namespace PM
{
	public class IDETextField : MonoBehaviour, IPMCompilerStarted, IPMCompilerStopped
	{
		public RectTransform contentContainer;
		public IDEFocusLord theFocusLord;
		public InputField theInputField;
		public IDELineMarker theLineMarker;
		public IDEScrollLord theScrollLord;
		public Text preText;
		public Text text;
		public Text visibleText;
		[FormerlySerializedAs("theSpeciallCommands")]
		public IDESpecialCommands theSpecialCommands;
		public int codeRowsLimit = 32;
		public bool devBuild = false;

		[NonSerialized]
		public string preCode = string.Empty;

		[NonSerialized]
		public string postCode = string.Empty;

		[NonSerialized]
		public RectTransform inputRect;

		const int MAX_CHARS = 100;

		private string lastText = "";
		private float startYPos;
		private bool settingNewCaretPos = false;
		private bool inserting = false;

		public int rowLimit
		{
			get => codeRowsLimit;
			set
			{
				codeRowsLimit = value;
				IDETextManipulation.ValidateText(theInputField.text, codeRowsLimit, MAX_CHARS);
			}
		}

		private List<IndentLevel> toAddLevels = new List<IndentLevel>();

		void Start()
		{
			text.horizontalOverflow = HorizontalWrapMode.Overflow;

			theLineMarker.InitLineMarker(this, theFocusLord);

			inputRect = theInputField.GetComponent<RectTransform>();

			startYPos = inputRect.anchoredPosition.y;
			InitTextFields();

			theInputField.onValidateInput += delegate(string input, int charIndex, char addedChar)
			{
				return MyValidate(addedChar, charIndex);
			};
			theInputField.onValueChanged.AddListener(_ => DoValidateInput());

			theFocusLord.InitReferences(theInputField, this);

			// poor mans reActivate
			theFocusLord.stealFocus = true;
			theInputField.interactable = true;

			ColorCodeDaCode();
			FocusCursor();
		}

		public void InitTextFields()
		{
			preText.text = IDEColorCoding.RunColorCode(preCode);

			float preHeight = CalcAndSetPreCode();
			inputRect.anchoredPosition = new Vector2(inputRect.anchoredPosition.x, startYPos - preHeight);
		}

		void Update()
		{
#if UNITY_WEBGL
			WebTabSupport();
#elif UNITY_EDITOR
			if (devBuild)
				webTabSupport();
#endif

			if (Input.inputString.Length > 0
			    || IDESpecialCommands.AnyKey(
				    KeyCode.UpArrow,
				    KeyCode.DownArrow,
				    KeyCode.LeftArrow,
				    KeyCode.RightArrow,
				    KeyCode.PageDown,
				    KeyCode.PageUp,
				    KeyCode.Home,
				    KeyCode.End
			    ))
			{
				FocusCursor();
			}
		}

		void LateUpdate()
		{
			if (inserting)
			{
				toAddLevels.Clear();
				inserting = false;
			}

			if (toAddLevels.Count > 0)
			{
				AddAutoIndent();
			}

			if (settingNewCaretPos)
			{
				settingNewCaretPos = false;
			}

			theInputField.text = theSpecialCommands.CheckHistoryCommands(theInputField.text);
		}

		/// <summary>Updates main code syntax highlighting and size of rects (for scrolling)</summary>
		public void ColorCodeDaCode()
		{
			visibleText.text = IDEColorCoding.RunColorCode(theInputField.text);
			inputRect.sizeDelta = new Vector2(inputRect.sizeDelta.x, visibleText.preferredHeight + 6);
			contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x,
				inputRect.sizeDelta.y - inputRect.anchoredPosition.y);
		}

		public void MoveLineMarker()
		{
			int preRows = (string.IsNullOrEmpty(preCode) ? 0 : preCode.Split('\n').Length) + 1;
			int selected = preRows + IDEParser.CalcCurrentSelectedLine(theInputField.caretPosition, theInputField.text);
			IDELineMarker.SetIDEPosition(selected);
		}

		public float DetermineYOffset(int lineNumber)
		{
			DetermineYOffset(lineNumber, out float topY, out float lineHeight);
			return topY;
		}

		public void DetermineYOffset(int lineNumber, out float topY, out float lineHeight)
		{
			// Determine if we're calculating preCode line or mainCode line
			int preCodeLines = preCode.Length == 0 ? 0 : preCode.Split('\n').Length;
			bool isPre = lineNumber < preCodeLines;

			// lineIndex of the textfield, either preCodeField or mainCodeField
			int lineIndex = isPre ? lineNumber : lineNumber - preCodeLines;
			TextGenerator gen = isPre ? preText.cachedTextGenerator : theInputField.textComponent.cachedTextGenerator;

			if (gen.lineCount == 0)
			{
				topY = (isPre ? preText : theInputField.textComponent).rectTransform.anchoredPosition.y;
				lineHeight = 0;
				return;
			}

			// Get topmost Y position of line
			topY = gen.lines[Mathf.Clamp(lineIndex, 0, gen.lineCount - 1)].topY;

			// topY is in pixels. We need units, therefore div by ppu.
			// +1.4 is just a small offset to make it look better
			if (isPre)
			{
				topY = topY / preText.pixelsPerUnit + preText.rectTransform.anchoredPosition.y + 1.4f;
				lineHeight = gen.lines[0].height / theInputField.textComponent.pixelsPerUnit;
			}
			else
			{
				topY = topY / theInputField.textComponent.pixelsPerUnit +
				       theInputField.textComponent.rectTransform.anchoredPosition.y + 1.4f;
				lineHeight = gen.lines[0].height * 1.5f / theInputField.textComponent.pixelsPerUnit;
			}
		}

		/// <summary>If has precode, returns the height of the precode text component, else return 0.</summary>
		private float CalcAndSetPreCode()
		{
			if (preCode.Length == 0)
			{
				preText.gameObject.SetActive(false);
				return 0;
			}
			else
			{
				preText.gameObject.SetActive(true);
				float height = preText.preferredHeight + 8;
				preText.rectTransform.sizeDelta = new Vector2(preText.rectTransform.sizeDelta.x, height);
				return height;
			}
		}

		#region Text Input Manipulation

		private int lastCharInsertIndex = 0;

		private char MyValidate(char c, int charIndex)
		{
			if (inserting)
			{
				return c;
			}

			lastCharInsertIndex = charIndex;

			return IDETextManipulation.MyValidate(c, charIndex, IsPasting(), toAddLevels, theInputField.text, this);
		}

		private bool IsPasting()
		{
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) ||
			     Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.V))
			{
				return true;
			}

			return false;
		}

		private void AddAutoIndent()
		{
			string startString = theInputField.text;
			foreach (IndentLevel l in toAddLevels)
			{
				startString = startString.Insert(l.caretPos, l.indentText);
			}

			if (IDETextManipulation.ValidateText(startString, codeRowsLimit, MAX_CHARS))
			{
				inserting = true;
				theInputField.text = startString;
			}

			toAddLevels.Clear();
		}

		//Cheks if the current input will fit into the speciefied borders!
		//Restricted by amount of rows in Y
		//and by maxChars in X
		private void DoValidateInput()
		{
			if (IDETextManipulation.ValidateText(theInputField.text, codeRowsLimit, MAX_CHARS))
			{
				lastText = theInputField.text;
				ColorCodeDaCode();
			}
			else
			{
				inserting = true;
				theInputField.text = lastText;
				theInputField.caretPosition = lastCharInsertIndex;
			}

			Progress.instance.levelData[PMWrapper.currentLevel.id].mainCode = PMWrapper.mainCode;
			FocusCursor();
			theLineMarker.RemoveErrorMessage();
		}

		#endregion

		#region caret & scroll position

		public void SetNewCaretPos(int newPos)
		{
			if (settingNewCaretPos)
			{
				return;
			}

			//oldCaretPos = newPos;
			StopCoroutine(SetCaretPos(newPos));
			StartCoroutine(SetCaretPos(newPos));
			settingNewCaretPos = true;
		}

		private IEnumerator SetCaretPos(int newPos)
		{
			Color preColor = theInputField.caretColor;
			theInputField.caretColor = Vector4.zero;
			yield return new WaitForEndOfFrame();
			theInputField.caretPosition = newPos;
			theInputField.caretColor = preColor;
		}

		public void FocusCursor()
		{
			Canvas.ForceUpdateCanvases();
			int preCodeLines = preCode.Length == 0 ? 0 : preCode.Split('\n').Length;
			theScrollLord.FocusOnLineNumber(
				IDEParser.CalcCurrentSelectedLine(theInputField.selectionAnchorPosition, theInputField.text) +
				preCodeLines);
			MoveLineMarker();
		}

		#endregion

		#region insert extraText

		public void InsertText(string newText, bool smartButtony = false)
		{
			if (theInputField.isFocused)
			{
				// If selection, replace selection
				// Else, just insert
				int start = Mathf.Min(theInputField.selectionAnchorPosition, theInputField.selectionFocusPosition);
				int end = Mathf.Max(theInputField.selectionAnchorPosition, theInputField.selectionFocusPosition);
				string checkText;

				if (start == end)
				{
					// No selection
					checkText = theInputField.text.Insert(start, newText);
				}
				else
				{
					// Yes selection
					checkText = theInputField.text.Substring(0, start) + newText + theInputField.text.Substring(end);
				}

				// Try adding extra newline if it's the end of the line
				string checkText2 = null;
				string after = checkText.Substring(start + newText.Length);
				if (smartButtony && (after.Length == 0 || after[0] == '\n'))
				{
					int indent = IDEParser.GetIndentLevel(start, checkText);

					// Add newline directly
					if (start > 0 && checkText[start - 1] == ':')
					{
						checkText = checkText.Insert(start, "\n" + new string('\t', indent));
						start += 1 + indent;
					}

					checkText2 = checkText.Insert(start + newText.Length, "\n" + new string('\t', indent));
					start += 1 + indent;
				}

				// Check if it fits
				if (checkText2 == null || !IDETextManipulation.ValidateText(checkText2, codeRowsLimit, MAX_CHARS))
				{
					checkText2 = null;
					if (!IDETextManipulation.ValidateText(checkText, codeRowsLimit, MAX_CHARS))
					{
						checkText = null;
					}
				}

				// One of them succeded
				if (checkText2 != null || checkText != null)
				{
					inserting = true;

					SetNewCaretPos(start + newText.Length);
					theInputField.text = checkText2 ?? checkText;
				}
			}
		}

		public void InsertMainCode(string code)
		{
			theInputField.text = code;
			inserting = true;
			if (!enabled)
			{
				ColorCodeDaCode();
			}
		}

		public void InsertMainCodeAtStart(string code)
		{
			if (!Progress.instance.levelData[PMWrapper.currentLevel.id].isStarted)
			{
				theInputField.text = code;
				inserting = true;
				Progress.instance.levelData[PMWrapper.currentLevel.id].mainCode = PMWrapper.mainCode;
			}
		}

		private void WebTabSupport()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				InsertText("\t");
			}
		}

		public void ClearText()
		{
			inserting = true;
			theInputField.text = "";
			toAddLevels.Clear();
			SetCaretPos(0);
		}

		#endregion

		#region public Get/Setters

		public void DeactivateField()
		{
			enabled = false;
			theFocusLord.stealFocus = false;
			theInputField.interactable = false;
		}

		public void ReactivateField()
		{
			enabled = true;
			theFocusLord.stealFocus = true;
			theInputField.interactable = true;
		}

		#endregion

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			DeactivateField();
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			ReactivateField();
		}
	}
}