using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class UISingleton : MonoBehaviour
	{
		public static UISingleton instance;

		public Transform ideRoot { get { return transform.parent; } }

		[Header("PM")]
		public HelloCompiler compiler;
		public IDETextField textField;
		public CodeWalker walker;
		public GlobalSpeed speed;
		public Guide.GuidePlayer guidePlayer;
		public SaveData saveData;

		[Header("UI")]
		public IDERowsLimit rowsLimit;
		public RectTransform gameCameraRect;
		public SmartButtonController smartButtons;
		public WinScreen winScreen;
		public ProgressBar levelbar;
		public GameObject uiTooltipPrefab;
		public GameObject varTooltipPrefab;
		public RectTransform tooltipParent;
		public TaskDescription taskDescription;
		public CanvasGroup uiCanvasGroup;

		[Header("Bubbles")]
		public IDEGuideBubble guideBubble;
		public AnswerBubble answerBubble;

		[Header("Misc")]
		public Camera uiCamera;
		public Camera popupCamera;

		[HideInInspector]
		public List<ManusSelectable> manusSelectables = new List<ManusSelectable>();

		[Serializable]
		public struct ManusSelectable
		{
			public Selectable selectable;
			public List<string> names;
		}

		private void Awake()
		{
			instance = this;
		}

		/// <summary>
		/// This function is made for finding objects. Similar to <seealso cref="UnityEngine.Object.FindObjectsOfType{T}"/> but also works for interfaces.
		/// <para>The catch though is that it can only search amoung classes that inherit from <see cref="UnityEngine.Object"/></para>
		/// </summary>
		public static T[] FindInterfaces<T>() where T : class
		{
			var list = new List<UnityEngine.Object>(FindObjectsOfType<UnityEngine.Object>()).ConvertAll(o => o as T);
			list.RemoveAll(o => o == null);
			return list.ToArray();
		}

	}

}