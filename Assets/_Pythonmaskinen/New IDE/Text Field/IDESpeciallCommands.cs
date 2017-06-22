using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace PM {

	public class IDESpeciallCommands : MonoBehaviour {

		private IDETextHistory theHistory = new IDETextHistory ();
		private bool isInHistoryCommand = false;

		private float thresholdCounter = 0;
		private float thresholdTime = 0.4f;

		//Saves text and resets timer if there are no history commands going on
		public string checkHistoryCommands(string currentText) {
			if (isStepingBackInHistory() || isStepingForwardkInHistory())
				return handleHistoryEvent(currentText);

			theHistory.saveText(currentText);
			thresholdCounter = 0;
			isInHistoryCommand = false;
			return currentText;
		}


		//If the thresholdtime is not fulfilled we return currenttext
		private string handleHistoryEvent(string currentText) {
			if (isInHistoryCommand) {
				thresholdCounter += Time.deltaTime;

				if (thresholdCounter < thresholdTime)
					return currentText;
			}
			isInHistoryCommand = true;

			if (isStepingBackInHistory())
				return theHistory.stepBackInHistory();

			return theHistory.stepForwardInHistory();
		}


		#region Speciall Commands Check
		public bool speciallCommandIsRunning() {
			return isPasting() || isStepingBackInHistory() || isStepingForwardkInHistory();
		}

		// CTRL + V
		private bool isPasting() {
			return AnyKey(KeyCode.LeftCommand, KeyCode.RightCommand, KeyCode.LeftControl, KeyCode.RightControl) && Input.GetKey(KeyCode.V);
		}

		// CTRL + Z
		private bool isStepingBackInHistory() {
			return AnyKey(KeyCode.LeftCommand, KeyCode.RightCommand, KeyCode.LeftControl, KeyCode.RightControl) && Input.GetKey(KeyCode.Z);
		}

		// CTRL + Y
		// or CTRL + SHIFT + Z
		private bool isStepingForwardkInHistory() {
			return 
				AnyKey(KeyCode.LeftCommand, KeyCode.RightCommand, KeyCode.LeftControl, KeyCode.RightControl)
				&& (Input.GetKey(KeyCode.Y) || (AnyKey(KeyCode.LeftShift, KeyCode.RightShift) && Input.GetKey(KeyCode.Z)));

			
		}
		#endregion

		public static readonly ReadOnlyCollection<KeyCode> keyboardKeys = new ReadOnlyCollection<KeyCode>(
			Enum.GetValues(typeof(KeyCode))
			.Cast<KeyCode>()
			.Where(k => !k.ToString().StartsWith("Mouse") && !k.ToString().StartsWith("Joystick"))
			.ToList()
		);
		public static readonly ReadOnlyCollection<KeyCode> mouseKeys = new ReadOnlyCollection<KeyCode>(
			Enum.GetValues(typeof(KeyCode))
			.Cast<KeyCode>()
			.Where(k => k.ToString().StartsWith("Mouse"))
			.ToList()
		);
		public static bool AnyKeyboardKey() {
			return AnyKey(keyboardKeys.ToArray());
		}
		public static bool AnyKeyboardKeyDown() {
			return AnyKeyDown(keyboardKeys.ToArray());
		}
		public static bool AnyKeyboardKeyUp() {
			return AnyKeyUp(keyboardKeys.ToArray());
		}

		public static bool AnyMouseKey() {
			return AnyKey(mouseKeys.ToArray());
		}
		public static bool AnyMouseKeyDown() {
			return AnyKeyDown(mouseKeys.ToArray());
		}
		public static bool AnyMouseKeyUp() {
			return AnyKeyUp(mouseKeys.ToArray());
		}

		public static bool AnyKey(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (Input.GetKey(keys[i]))
					return true;
			return false;
		}
		public static bool AnyKeyDown(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (Input.GetKeyDown(keys[i]))
					return true;
			return false;
		}
		public static bool AnyKeyUp(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (Input.GetKeyUp(keys[i]))
					return true;
			return false;
		}

		public static bool AllKeys(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (!Input.GetKey(keys[i]))
					return false;
			return keys.Length > 0;
		}
		public static bool AllKeysDown(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (!Input.GetKeyDown(keys[i]))
					return false;
			return keys.Length > 0;
		}
		public static bool AllKeysUp(params KeyCode[] keys) {
			for (int i = keys.Length - 1; i >= 0; i--)
				if (!Input.GetKeyUp(keys[i]))
					return false;
			return keys.Length > 0;
		}
	}

}