using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class ProgressBar : MonoBehaviour {

		public Vector3 spriteScale;
		public Button levelPrefab;

		public Sprite midCurrent;
		public Sprite midUnlocked;
		public Sprite midLocked;
		public Sprite leftCurrent;
		public Sprite leftUnlocked;
		public Sprite leftLocked;
		public Sprite rightCurrent;
		public Sprite rightUnlocked;
		public Sprite rightLocked;

		//public Sprite spr_done_star;
		//public Sprite spriteDemoDone;
		//public Sprite spriteDemoCurrent;
		//public Sprite spriteDemoLocked;

		public int numOfLevels { get { return levels.Count; } }
		public int current { get; private set; }
		public int previous { get; private set; }
		public int unlocked { get; private set; }
		private List<Button> levels = new List<Button>();

		private void Awake() {
			previous = -1;
			current = -1;
		}

		public void RecreateButtons(int numOfLevels, int current = 0, int unlocked = 0) {
			UISingleton.instance.manusSelectables.RemoveAll(s => s.selectable is Button && levels.IndexOf(s.selectable as Button) != -1);
			levels.ForEach(b => { if (b != null) Destroy(b.gameObject); });
			levels.Clear();

			for (int i = 0; i < numOfLevels; i++) {
				var btn = Instantiate(levelPrefab);

				// TEST
				btn.image.type = Image.Type.Filled;

				btn.transform.SetParent(transform, false);

				var rect = btn.GetComponent<RectTransform>();
				rect.anchorMin =
				rect.anchorMax = new Vector2(Mathf.Lerp(0.1f, 0.9f, i / (numOfLevels - 1f)), 0.1f);

				if (i == 0 || i == numOfLevels-1) {
					//rect.localScale = new Vector2(0.6f,0.45f);
					rect.localScale = new Vector2(0.4f,0.45f);
				} else {
					rect.localScale = new Vector2(0.4f,0.45f);
				}


				btn.name = "Level " + i + " button";

				// Saving it outside the lambda, otherwise it would still give me value of i, which has by that point been changed from the for loop.
				int levelIndex = i;
				btn.onClick.AddListener(() => {
					// Any variables used in here will be kept in memory, and not GC'ed.
					ChangeLevel(levelIndex);
					//SaveData.gottenLevelTip = true;
				});

				// Add to list of selectables
				UISingleton.instance.manusSelectables.Add(new UISingleton.ManusSelectable {
					selectable = btn,
					names = new List<string> { "level"+i },
				});

				levels.Add(btn);
			}
			Manus.Loader.BuildAll();

			UpdateButtons(current, unlocked);
		}

		private void UpdateButtons() {
			for (int i = 0; i < levels.Count; i++) {
				UpdateSingleButton(i);
			}
		}

		public void UpdateButtons(int newCurrent, int newUnlocked) {
			bool levelChange = newCurrent != this.current;

			this.unlocked = newUnlocked;

			if (levelChange) {
				this.previous = this.current;
				this.current = newCurrent;
			}

			UpdateButtons();
		}

		private void UpdateSingleButton(int level) {
			var btn = levels[level];

			if (level == 0) {
				btn.image.sprite = level == current ? leftCurrent : (level > unlocked ? leftLocked : leftUnlocked);
			} else if (level < numOfLevels-1) {
				btn.image.sprite = level == current ? midCurrent : (level > unlocked ? midLocked : midUnlocked);
			} else {
				btn.image.sprite = level == current ? rightCurrent : (level > unlocked ? rightLocked : rightUnlocked);
			}
			
			bool oldInterac = btn.interactable;
			btn.interactable = level <= unlocked && level != current;

			if (!oldInterac && btn.interactable)
				// Going from disabled to enabled
				// This is an easy bug fix for Unitys UI highlight bug.
				btn.image.CrossFadeColor(btn.colors.normalColor, 0.1f, true, false);

			UITooltip tooltip = btn.GetComponent<UITooltip>();
			if (tooltip) {
				tooltip.text = Manus.Loader.allManuses[level] != null ? "Demo" : "Nivå " + GetLevelNumber(level);
				if (level == current) tooltip.text = "<color=green><b>" + tooltip.text + "</b></color> <color=grey>(Nuvarande)</color>";
				if (level > unlocked) tooltip.text += " <color=grey>(Låst)</color>";
				tooltip.ApplyTooltipTextChange();
			}
		}

		public static int GetLevelNumber(int level) {
			int num = 1;
			for (int i=0; i<PMWrapper.numOfLevels; i++) {
				if (level == i) break;
				if (Manus.Loader.allManuses[i] == null) num++;
			}
			return num;
		}

		public void ChangeLevel(int level) {
			if (level == current) return;

			unlocked = Mathf.Max(level, unlocked);

			// Update which one is current one
			UpdateButtons(level, unlocked);

			UISingleton.instance.levelHandler.LoadLevel (level);
		}

	}

}