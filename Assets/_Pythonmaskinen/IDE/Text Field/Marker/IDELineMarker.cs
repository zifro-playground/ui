using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDELineMarker : MonoBehaviour, IPMCompilerStopped, IPMCompilerStarted {
		
		public IDEErrorBubble theErrorBubble;
		private RectTransform theMarkerRect;
		private IDEFocusLord theFocusLord;
		private IDETextField theTextField;

		public static int lineNumber { get; private set; }
		private static float currentMakerPos;

		public State state { get; private set; }

		private Image theImage;
		//private static readonly Color functionColor = new Color(0, 1, 1, 0.3f);
		private static readonly Color walkerColor =  new Color(0, 1, 0, 0.3f);
		private static readonly Color errorColor = new Color(1, 0, 0, 0.3f);
		private static readonly Color IDEColor = new Color(1, 1, 1, 0.1f);

		public static IDELineMarker instance { get; private set; }

		public static void SetWalkerPosition(int newLineNumber) {
			lineNumber = newLineNumber - 1;
			instance.moveMarker();
			instance.SetState(State.Walker);
			instance.theTextField.theScrollLord.FocusOnLineNumber(lineNumber);
		}

		public static void SetIDEPosition(int newLineNumber) {
			lineNumber = newLineNumber - 1;
			instance.moveMarker();
			instance.SetState(State.IDE);
		}

		public static void activateFunctionCall() {
		}

		public void initLineMarker(IDETextField theTextField, IDEFocusLord theFocusLord) {
			instance = this;

			this.theTextField = theTextField;
			this.theFocusLord = theFocusLord;
			theMarkerRect = transform as RectTransform;
			this.enabled = false;
			theImage = GetComponentInChildren<Image>();
			theErrorBubble.init(this);
		}
		
		private void moveMarker() {
			if (lineNumber < 0)
				theMarkerRect.anchoredPosition = new Vector2(0, -5000);
			else //+20 on x to fix offset. Should possibly be changed in UI instead
				theMarkerRect.anchoredPosition = new Vector2(20, theTextField.DetermineYOffset(lineNumber));
		}
		
		#region All setErrorMarkers
		private void _setErrorMarker(string message) {
			theFocusLord.selectEndOfLine(lineNumber + 1);
			moveMarker();
			SetState(State.Error);

			theErrorBubble.SetErrorMessage(message);
			UISingleton.instance.compiler.stopCompiler(HelloCompiler.StopStatus.RuntimeError);
			//UISingleton.instance.exceptionHandler.sendErrorToAnalytics (message);
			throw new PMRuntimeException(message);
		}

		public void setErrorMarker(string message) {
			theErrorBubble.ShowMessage(lineNumber);
			_setErrorMarker(message);
		}
		public void setErrorMarker(int newLineNumber, string message) {
			lineNumber = newLineNumber - 1;
			theErrorBubble.ShowMessage(newLineNumber);
			_setErrorMarker(message);
		}
		public void setErrorMarker(Vector2 targetCanvasPos, string message) {
			theErrorBubble.ShowMessage(targetCanvasPos);
			_setErrorMarker(message);
		}
		public void setErrorMarker(Vector3 targetWorldPos, string message) {
			theErrorBubble.ShowMessage(targetWorldPos);
			_setErrorMarker(message);
		}
		public void setErrorMarker(RectTransform targetRectTransform, string message) {
			theErrorBubble.ShowMessage(targetRectTransform);
			_setErrorMarker(message);
		}
		public void setErrorMarker(Selectable targetSelectable, string message) {
			theErrorBubble.ShowMessage(targetSelectable);
			_setErrorMarker(message);
		}
		#endregion

		public void removeErrorMessage() {
			if (state != State.Error)
				return;

			SetState(State.Hidden);
			theErrorBubble.HideMessage();
		}

		public void enteredChar() {
			if (state == State.Error)
				removeErrorMessage();
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status) {
			if (status != HelloCompiler.StopStatus.RuntimeError)
				SetState(State.Hidden);
		}

		void IPMCompilerStarted.OnPMCompilerStarted() {
			SetState(State.Hidden);
		}

		public void SetState(State newState) {
			if (newState == state) return;
			switch (newState) {
				case State.Error: theImage.color = errorColor; break;
				case State.Hidden: theImage.color = Color.clear; break;
				case State.Walker: theImage.color = walkerColor; break;
				case State.IDE: theImage.color = IDEColor; break;
			}
			theImage.enabled = newState != State.Hidden;
			state = newState;
		}

		public enum State {
			Hidden,
			Error,
			Walker,
			IDE
		}
	}

}