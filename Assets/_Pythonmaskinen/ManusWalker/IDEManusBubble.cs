using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PM {

	public class IDEManusBubble : AbstractPopupBubble {

		[Header("Manus bubble fields")]
		public Text theHeaderText;
		public VerticalLayoutGroup theBodyGroup;
		public CanvasGroup theNextButton;
		public GameObject rawTextPrefab;
		public GameObject codePrefab;

		public Transform cloneHome;
		public float cloneScale = 1;
		public float nextButtonDelay = 1;
		public float nextButtonFade = 1;
		
		private Selectable selectableClone;
		private Coroutine fadeNextButtonRoutine;

		private void DuplicateSelectable(Selectable target) {
			// Duplicate the selectable
			var clone = Instantiate(target.gameObject, target.transform.parent);

			clone.transform.SetParent(cloneHome, true);
			clone.transform.localScale *= cloneScale;
			selectableClone = clone.GetComponent<Selectable>();
			selectableClone.interactable = true;
			target.gameObject.SetActive(false);
			clone.transform.localPosition = new Vector3(clone.transform.localPosition.x, clone.transform.localPosition.y, -50);

			// Remove unnessesary components
			foreach (var comp in clone.GetComponents<MonoBehaviour>()) {
				if (comp is Button) (comp as Button).onClick.RemoveAllListeners();
				if (comp is Slider) (comp as Slider).onValueChanged.RemoveAllListeners();

				if (comp is Selectable || comp is Graphic || comp is UITooltip)
					continue;
				else
					Destroy(comp);
			}

			// Add click listener
			EventTrigger trigger = clone.gameObject.AddComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(data => HideMessage());
			trigger.triggers.Add(entry);

			// Add outline effect
			Outline outline = clone.gameObject.AddComponent<Outline>();
			outline.effectDistance = Vector2.one * 4;
			outline.effectColor = new Color(0, 1, 0, 0.5f);
		}

		public void SetMessageContent(string header, string body) {
			string[] all_splits = Regex.Split(body, @"(<code>[\s\S]+?<\/code>)").Where(s => s != string.Empty).ToArray();

			for (int i=0; i<all_splits.Length; i++) {
				string str = all_splits[i];

				GameObject prefab;

				if (str.StartsWith("<code>") && str.EndsWith("</code>")) {
					str = str.Substring(6, str.Length - 13);
					str = IDEColorCoding.runColorCode(str);
					prefab = codePrefab;
				} else {
					prefab = rawTextPrefab;
				}

				// Remove 1 newline at end and start
				if (str[str.Length - 1] == '\n') str = str.Substring(0, str.Length - 1);
				if (str.Length > 0 && str[0] == '\n') str = str.Substring(1, str.Length - 1);
				if (str.Length == 0) continue;

				GameObject clone = Instantiate(prefab, prefab.transform.parent);
				clone.SetActive(true);
				clone.GetComponentInChildren<Text>().text = str;
			}

			theHeaderText.text = header;
			ResizeToFit(theBodyGroup, bubbleRect);
		}

		protected override void OnShowMessage() {
			// Always hide
			theNextButton.alpha = 0;
			theNextButton.interactable = false;
			theNextButton.blocksRaycasts = false;

			if (target is Selectable)
				DuplicateSelectable(target as Selectable);
			else {
				// Restarts fade routine
				if (fadeNextButtonRoutine != null) {
					StopCoroutine(fadeNextButtonRoutine);
					fadeNextButtonRoutine = null;
				}
				fadeNextButtonRoutine = StartCoroutine(FadeNextButton());
			}

			// Override for standalone manus bubbles
			if (!PMWrapper.IsDemoingLevel) {
				UISingleton.instance.textField.deActivateField();
				UISingleton.instance.uiCanvasGroup.blocksRaycasts = false;
			}
		}

		protected override void OnHideMessage() {
			if (selectableClone != null) {
				(target as Selectable).gameObject.SetActive(true);
				Destroy(selectableClone.gameObject);
				selectableClone = null;
			}

			foreach (Transform obj in theBodyGroup.transform)
				if (obj.gameObject.activeSelf)
					Destroy(obj.gameObject, fadeTime);

			// Stops fade routine if currently fading
			if (fadeNextButtonRoutine != null) {
				StopCoroutine(fadeNextButtonRoutine);
				fadeNextButtonRoutine = null;
			}

			// Override for standalone manus bubbles
			if (!PMWrapper.IsDemoingLevel) {
				UISingleton.instance.textField.reActivateField();
				UISingleton.instance.uiCanvasGroup.blocksRaycasts = true;
			}
		}

		IEnumerator FadeNextButton() {
			yield return new WaitForSecondsRealtime(nextButtonDelay);
			
			theNextButton.interactable = true;
			theNextButton.blocksRaycasts = true;

			while (!Mathf.Approximately(theNextButton.alpha, 1)) {
				theNextButton.alpha = Mathf.Max(0, theNextButton.alpha + Time.deltaTime / nextButtonFade);
				yield return null;
			}

			theNextButton.alpha = 1;
			fadeNextButtonRoutine = null;
		}
	}

}