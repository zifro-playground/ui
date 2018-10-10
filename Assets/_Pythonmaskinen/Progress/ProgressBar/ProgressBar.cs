
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
		public int Unlocked { get; private set; }
		private List<Button> levels = new List<Button>();

		private void Awake()
		{
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

			UpdateButtons(current, unlocked);
		}

		public void UpdateButtons(int newCurrent, int newUnlocked)
		{
			Unlocked = newUnlocked;
			Current = newCurrent;
			
			for (int i = 0; i < levels.Count; i++)
			{
				UpdateSingleButton(i);
			}
		}

		private void UpdateSingleButton(int levelIndex)
		{
			var btn = levels[levelIndex];

			var levelId = Main.Instance.GameDefinition.activeLevels[levelIndex].levelId;
			var levelData = Progress.Instance.LevelData[levelId];

			if (levelIndex == 0)
			{
				btn.image.sprite = levelIndex == Current ? LeftCurrent : (levelIndex <= Unlocked || levelData.IsStarted ? LeftUnlocked : LeftLocked);
				if (NumberOfLevels == 1)
					ProgressBarParent.SetActive(false);
			}
			else if (levelIndex < NumberOfLevels - 1)
			{
				btn.image.sprite = levelIndex == Current ? MidCurrent : (levelIndex <= Unlocked || levelData.IsStarted ? MidUnlocked : MidLocked);
			}
			else
			{
				btn.image.sprite = levelIndex == Current ? RightCurrent : (levelIndex <= Unlocked || levelData.IsStarted ? RightUnlocked : RightLocked);
			}

			btn.interactable = (levelIndex <= Unlocked || levelData.IsStarted) && levelIndex != Current;

			UITooltip tooltip = btn.GetComponent<UITooltip>();
			if (tooltip)
			{
				tooltip.text = "Nivå " + levelIndex;
				if (levelIndex == Current) tooltip.text = "<color=green><b>" + tooltip.text + "</b></color> <color=grey>(Nuvarande)</color>";
				if (levelIndex > Unlocked && !levelData.IsStarted) tooltip.text += " <color=grey>(Låst)</color>";
				tooltip.ApplyTooltipTextChange();
			}
		}

		public void ChangeLevel(int levelIndex)
		{
			if (levelIndex == Current) return;

			foreach (var ev in UISingleton.FindInterfaces<IPMUnloadLevel>())
				ev.OnPMUnloadLevel();

			Unlocked = Mathf.Max(levelIndex, Unlocked);

			// Update which one is current one
			UpdateButtons(levelIndex, Unlocked);

			Main.Instance.StartLevel(levelIndex);
		}

	}

}