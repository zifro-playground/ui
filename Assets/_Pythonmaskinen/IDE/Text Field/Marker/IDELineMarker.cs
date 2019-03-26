using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class IDELineMarker : MonoBehaviour, IPMCompilerStopped, IPMCompilerStarted
	{
		public IDEErrorBubble theErrorBubble;
		private RectTransform theMarkerRect;
		private IDEFocusLord theFocusLord;
		private IDETextField theTextField;

		public static int lineNumber { get; private set; }

		public State state { get; private set; }

		private Image theImage;

		//private static readonly Color functionColor = new Color(0, 1, 1, 0.3f);
		private static readonly Color WALKER_COLOR = new Color(0.2470588f, 0.7215686f, 0.6117647f, 0.4f);
		private static readonly Color ERROR_COLOR = new Color(1, 0, 0, 0.3f);
		private static readonly Color IDE_COLOR = new Color(1, 1, 1, 0.1f);

		public static IDELineMarker instance { get; private set; }

		public static void SetWalkerPosition(int newLineNumber)
		{
			lineNumber = newLineNumber - 1;
			instance.MoveMarker();
			instance.SetState(State.Walker);
			instance.theTextField.theScrollLord.FocusOnLineNumber(lineNumber);
		}

		public static void SetIDEPosition(int newLineNumber)
		{
			lineNumber = newLineNumber - 1;
			instance.MoveMarker();
			instance.SetState(State.IDE);
		}

		public void InitLineMarker(IDETextField theTextField, IDEFocusLord theFocusLord)
		{
			instance = this;

			this.theTextField = theTextField;
			this.theFocusLord = theFocusLord;
			theMarkerRect = transform as RectTransform;
			enabled = false;
			theImage = GetComponentInChildren<Image>();
			theErrorBubble.Init(this);
		}

		private void MoveMarker()
		{
			if (lineNumber < 0)
			{
				theMarkerRect.anchoredPosition = new Vector2(0, -5000);
			}
			else //+20 on x to fix offset. Should possibly be changed in UI instead
			{
				theMarkerRect.anchoredPosition = new Vector2(20, theTextField.DetermineYOffset(lineNumber));
			}
		}

		#region All setErrorMarkers

		private void _setErrorMarker(string message)
		{
			theFocusLord.SelectEndOfLine(lineNumber + 1);
			MoveMarker();
			SetState(State.Error);

			theErrorBubble.SetErrorMessage(message);
			//UISingleton.instance.exceptionHandler.sendErrorToAnalytics (message);
			throw new PMRuntimeException(message);
		}

		public void SetErrorMarker(string message)
		{
			theErrorBubble.ShowMessage(lineNumber);
			_setErrorMarker(message);
		}

		public void SetErrorMarker(int newLineNumber, string message)
		{
			lineNumber = newLineNumber - 1;
			theErrorBubble.ShowMessage(newLineNumber);
			_setErrorMarker(message);
		}

		public void SetErrorMarker(Vector2 targetCanvasPos, string message)
		{
			theErrorBubble.ShowMessage(targetCanvasPos);
			_setErrorMarker(message);
		}

		public void SetErrorMarker(Vector3 targetWorldPos, string message)
		{
			theErrorBubble.ShowMessage(targetWorldPos);
			_setErrorMarker(message);
		}

		public void SetErrorMarker(RectTransform targetRectTransform, string message)
		{
			theErrorBubble.ShowMessage(targetRectTransform);
			_setErrorMarker(message);
		}

		public void SetErrorMarker(Selectable targetSelectable, string message)
		{
			theErrorBubble.ShowMessage(targetSelectable);
			_setErrorMarker(message);
		}

		#endregion

		public void RemoveErrorMessage()
		{
			if (state != State.Error)
			{
				return;
			}

			SetState(State.Hidden);
			theErrorBubble.HideMessage();
		}

		public void EnteredChar()
		{
			if (state == State.Error)
			{
				RemoveErrorMessage();
			}
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			if (status != HelloCompiler.StopStatus.RuntimeError)
			{
				SetState(State.Hidden);
			}
		}

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			SetState(State.Hidden);
		}

		public void SetState(State newState)
		{
			if (newState == state)
			{
				return;
			}

			switch (newState)
			{
			case State.Error:
				theImage.color = ERROR_COLOR;
				break;
			case State.Hidden:
				theImage.color = Color.clear;
				break;
			case State.Walker:
				theImage.color = WALKER_COLOR;
				break;
			case State.IDE:
				theImage.color = IDE_COLOR;
				break;
			}

			theImage.enabled = newState != State.Hidden;
			state = newState;
		}

		public enum State
		{
			Hidden,
			Error,
			Walker,
			IDE
		}
	}
}