using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class UISingleton : MonoBehaviour {

		public static UISingleton instance;

		public Transform ideRoot { get { return transform.parent; } }

		[Header("PM")]
		public HelloCompiler compiler;
		public IDETextField textField;
		public CodeWalker walker;
		public GlobalSpeed speed;
		public Manus.ManusPlayer manusPlayer;
		public Guide.GuidePlayer guidePlayer;
		public SaveData saveData;
		//public ExceptionHandler exceptionHandler;
		public Level.LevelHandler levelHandler;
		[Header("UI")]
		public IDERowsLimit rowsLimit;
		public LevelHints levelHints;
		public RectTransform gameCameraRect;
		public SmartButtonController smartButtons;
		public WinScreen winScreen;
		public ProgressBar levelbar;
		public GameObject uiTooltipPrefab;
		public GameObject varTooltipPrefab;
		public RectTransform tooltipParent;
		public IDETaskDescription taskDescription;
		public CanvasGroup uiCanvasGroup;
		[Header("Bubbles")]
		public IDEPrintBubble printBubble;
		public IDEManusBubble manusBubble;
		public IDEGuideBubble guideBubble;
		public AnswereBubble answereBubble;
		[Header("Misc")]
		public Camera uiCamera;
		public Camera popupCamera;

		[HideInInspector]
		public List<ManusSelectable> manusSelectables = new List<ManusSelectable>();

		[Serializable]
		public struct ManusSelectable {
			public Selectable selectable;
			public List<string> names;
		}

		private void Awake() {
			instance = this;

			/* Old. Should be replaced with something new
			// Load error api config
			TextAsset file = Resources.Load<TextAsset>("game_token");
			if (file == null) 
				gameToken = "1234";

			try {
				gameToken = file.text.Trim();
			} catch { }

			if (gameToken == null || gameToken.Length != 32 || !System.Text.RegularExpressions.Regex.IsMatch(gameToken, @"[a-fA-F0-9]+")) {
				Debug.LogError("Invalid game token!");
				gameToken = null;
			}*/
		}

		/// <summary>
		/// This function is made for finding objects. Similar to <seealso cref="UnityEngine.Object.FindObjectsOfType{T}"/> but also works for interfaces.
		/// <para>The catch though is that it can only search amoung classes that inherit from <see cref="UnityEngine.Object"/></para>
		/// </summary>
		public static T[] FindInterfaces<T>() where T : class {
			var list = new List<UnityEngine.Object>(FindObjectsOfType<UnityEngine.Object>()).ConvertAll(o => o as T);
			list.RemoveAll(o => o == null);
			return list.ToArray();
		}

	}

}