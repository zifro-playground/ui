using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace PM {
	public class LevelHints : MonoBehaviour, IPMLevelChanged {

		public CanvasGroup canvasGroup;
		public Button helpButton;
		public GameObject topmostObject;
		public Button closeButton;
		public UITooltip leftButton;
		public UITooltip rightButton;
		public Text title;
		public VerticalLayoutGroup contentGroup;
		public GameObject rawTextPrefab;
		public GameObject codePrefab;
		public Text numHints;
		public Image image;
		public float imageMargin = 20;
		public float fadeTime = 0.5f;

		public int storyRevealedForLevel { get; private set; }
		public bool hasStoryHint { get { return currentHints.Count > 0 && currentHints[0].isStory; } }

		private List<Hint> currentHints = new List<Hint>();
		private int currentViewing = 0;
		private float timePassed;
		private State state = State.Idle;

		private void SetCurrentHint(int hintIndex) {
			currentViewing = hintIndex;
			var hint = currentHints[hintIndex];
			bool firstIsStory = currentHints[0].isStory;

			// Apply values
			title.text = hint.title;

			SetHintContent(hint.content);

			numHints.text = "Nivå "
				+ (ProgressBar.GetLevelNumber(hint.level)).ToString()
				+ (hint.isStory
					? " - Story"
					: " - Tips "
					+ (firstIsStory ? hintIndex : hintIndex + 1).ToString().PadLeft(currentHints.Count.ToString().Length, '0')
					+ " av "
					+ (firstIsStory ? currentHints.Count - 1 : currentHints.Count)
				);

			// Enable/disable buttons
			rightButton.gameObject.SetActive(hintIndex < currentHints.Count - 1);
			leftButton.gameObject.SetActive(hintIndex > 0);
			if (leftButton.gameObject.activeSelf) {
				leftButton.text = firstIsStory && hintIndex == 1 ? "Visa story" : "Visa föregående tips";
				leftButton.ApplyTooltipTextChange();
			}
			if (rightButton.gameObject.activeSelf) {
				rightButton.text = firstIsStory && hintIndex == 0 ? "Visa tips" : "Visa nästa tips";
				rightButton.ApplyTooltipTextChange();
			}

			// Hint image
			if (hint.sprite) {

				// To get actual preferred height
				Canvas.ForceUpdateCanvases();

				image.color = Color.white;
				image.sprite = hint.sprite;
				image.preserveAspect = true;
				image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, (contentGroup.transform as RectTransform).rect.height - contentGroup.preferredHeight - imageMargin);
			} else {
				image.color = Color.clear;
			}
		}

		private void SetHintContent(string content) {
			string[] all_splits = Regex.Split(content, @"(<code>[\s\S]+?<\/code>)").Where(s => s != string.Empty).ToArray();

			for (int i = 0; i < all_splits.Length; i++) {
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

		}

		public void LoadHints() {
			currentHints.Clear();
			currentViewing = 0;

			int hint = 0;
			TextAsset asset = null;
			do {
				string path = "levelhint_" + PMWrapper.currentLevel + "_" + hint;
				asset = Resources.Load<TextAsset>(path);

				if (asset) {
					// Analyse it
					string all = asset.text.Trim();
					if (all.Length == 0) break;

					string[] rows = all.Split(new string[] { "\n\r", "\r\n", "\n", "\r" }, StringSplitOptions.None);

					string title = rows[0];
					string content = string.Empty;

					for (int i = 1; i < rows.Length; i++) {
						content += rows[i];
						if (i < rows.Length - 1)
							// not last row
							content += '\n';
					}

					Sprite sprite = Resources.Load<Sprite>(path);

					currentHints.Add(new Hint {
						title = title,
						content = content,
						level = PMWrapper.currentLevel,
						sprite = sprite,
						isStory = hint == 0
					});
				}

				hint++;
			} while (asset != null || hint == 1);

			if (PMWrapper.IsDemoLevel)
				helpButton.interactable = true;
			else
				// No hints? No hints.
				helpButton.interactable = currentHints.Count > 0;
		}
		
		void IPMLevelChanged.OnPMLevelChanged() {
			LoadHints();
			if (hasStoryHint && storyRevealedForLevel < PMWrapper.currentLevel) {
				storyRevealedForLevel = PMWrapper.currentLevel;
				SetCurrentHint(0);
				StartRevealFading();
			} else if (PMWrapper.IsDemoLevel) {
				UISingleton.instance.manusPlayer.SetIsManusPlaying(true);
			}
		}

		private void Awake() {
			storyRevealedForLevel = -1;
		}

		#region UI Button actions
		public void ButtonRightArrowPressed() {
			if (state != State.Idle) return;
			SetCurrentHint(currentViewing + 1);
		}

		public void ButtonLeftArrowPressed() {
			if (state != State.Idle) return;
			SetCurrentHint(currentViewing - 1);
		}

		public void ButtonClosePressed() {
			if (state != State.Idle) return;
			StartHideFading();
		}

		public void ButtonShowHintScreen() {
			if (currentHints.Count > 0) {
				if (state != State.Idle) return;
				SetCurrentHint(0);
				StartRevealFading();
			} else if (PMWrapper.IsDemoLevel) {
				UISingleton.instance.manusPlayer.SetIsManusPlaying(true);
				return;
			}
		}
		#endregion

		public void StartRevealFading() {
			if (topmostObject.activeSelf)
				// Already revealed
				return;

			topmostObject.SetActive(true);
			state = State.Show;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			timePassed = 0;
		}

		public void StartHideFading() {
			if (!topmostObject.activeSelf)
				// Already hidden
				return;

			state = State.Hide;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			timePassed = 0;

			if (PMWrapper.IsDemoLevel) {
				UISingleton.instance.manusPlayer.SetIsManusPlaying(true);
			}
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.F1) && !PMWrapper.IsDemoingLevel) {
				ButtonShowHintScreen();
				return;
			}

			if (state == State.Idle) return;

			timePassed += Time.deltaTime;

			if (timePassed > fadeTime) {
				// Done fading

				// Reset them
				canvasGroup.alpha = state == State.Show ? 1 : 0;

				if (state == State.Hide)
					topmostObject.SetActive(false);

				state = State.Idle;
			} else {
				// Fade them
				canvasGroup.alpha = state == State.Show
					? timePassed / fadeTime
					: 1 - timePassed / fadeTime;
			}
		}

		public struct Hint {
			public string title;
			public string content;
			public Sprite sprite;
			public int level;
			public bool isStory;
		}

		private enum State {
			Idle,
			Show,
			Hide
		}

	}
}