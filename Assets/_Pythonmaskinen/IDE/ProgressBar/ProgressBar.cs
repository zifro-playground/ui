
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class ProgressBar : MonoBehaviour
	{
		public GameObject ProgressBarParent;
		public Button ButtonPrefab;

		[Header("Left Button")]
		public Sprite LeftCurrent;
		public Sprite LeftUnlocked;
		public Sprite LeftLocked;

		[Header("Middle Button")]
		public Sprite MidCurrent;
		public Sprite MidUnlocked;
		public Sprite MidLocked;

		[Header("Right Button")]
		public Sprite RightCurrent;
		public Sprite RightUnlocked;
		public Sprite RightLocked;

		public int NumberOfLevels { get { return levels.Count; } }
		public int Current { get; private set; }
		public int Previous { get; private set; }
		public int Unlocked { get; private set; }
		private List<Button> levels = new List<Button>();

		private void Awake()
		{
			Previous = -1;
			Current = -1;
		}

		public void RecreateButtons(int numOfLevels, int current = 0, int unlocked = 0)
		{
			UISingleton.instance.manusSelectables.RemoveAll(s => s.selectable is Button && levels.IndexOf(s.selectable as Button) != -1);
			levels.ForEach(b => { if (b != null) Destroy(b.gameObject); });
			levels.Clear();

			for (int i = 0; i < numOfLevels; i++)
			{
				var btn = Instantiate(ButtonPrefab);

				// TEST
				btn.image.type = Image.Type.Filled;

				btn.transform.SetParent(transform, false);

				btn.name = "Level " + i + " button";

				// Saving it outside the lambda, otherwise it would still give me value of i, which has by that point been changed from the for loop.
				int levelIndex = i;
				btn.onClick.AddListener(() =>
				{
					ChangeLevel(levelIndex);
				});

				// Add to list of selectables
				UISingleton.instance.manusSelectables.Add(new UISingleton.ManusSelectable
				{
					selectable = btn,
					names = new List<string> { "level" + i },
				});

				levels.Add(btn);
			}
			Manus.Loader.BuildAll();

			UpdateButtons(current, unlocked);
		}

		private void UpdateButtons()
		{
			for (int i = 0; i < levels.Count; i++)
			{
				UpdateSingleButton(i);
			}
		}

		public void UpdateButtons(int newCurrent, int newUnlocked)
		{
			bool levelChange = newCurrent != Current;

			Unlocked = newUnlocked;

			if (levelChange)
			{
				Previous = Current;
				Current = newCurrent;
			}

			UpdateButtons();
		}

		private void UpdateSingleButton(int level)
		{
			var btn = levels[level];

			if (level == 0)
			{
				btn.image.sprite = level == Current ? LeftCurrent : (level > Unlocked ? LeftLocked : LeftUnlocked);
				if (NumberOfLevels == 1)
					ProgressBarParent.SetActive(false);
			}
			else if (level < NumberOfLevels - 1)
			{
				btn.image.sprite = level == Current ? MidCurrent : (level > Unlocked ? MidLocked : MidUnlocked);
			}
			else
			{
				btn.image.sprite = level == Current ? RightCurrent : (level > Unlocked ? RightLocked : RightUnlocked);
			}

			btn.interactable = level <= Unlocked && level != Current;

			UITooltip tooltip = btn.GetComponent<UITooltip>();
			if (tooltip)
			{
				tooltip.text = Manus.Loader.allManuses[level] != null ? "Demo" : "Nivå " + GetLevelNumber(level);
				if (level == Current) tooltip.text = "<color=green><b>" + tooltip.text + "</b></color> <color=grey>(Nuvarande)</color>";
				if (level > Unlocked) tooltip.text += " <color=grey>(Låst)</color>";
				tooltip.ApplyTooltipTextChange();
			}
		}

		public static int GetLevelNumber(int level)
		{
			int num = 1;
			for (int i = 0; i < PMWrapper.numOfLevels; i++)
			{
				if (level == i) break;
				if (Manus.Loader.allManuses[i] == null) num++;
			}
			return num;
		}

		public void ChangeLevel(int level)
		{
			if (level == Current) return;

			Unlocked = Mathf.Max(level, Unlocked);

			// Update which one is current one
			UpdateButtons(level, Unlocked);

			UISingleton.instance.levelHandler.LoadLevel(level);
		}

	}

}