using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PM
{
	public class ProgressBar : MonoBehaviour
	{
		[FormerlySerializedAs("ProgressBarParent")]
		public GameObject progressBarParent;

		[FormerlySerializedAs("ButtonPrefab")]
		public Button buttonPrefab;

		[FormerlySerializedAs("LeftCurrent")]
		[Header("Left Button")]
		public Sprite leftCurrent;

		[FormerlySerializedAs("LeftUnlocked")]
		public Sprite leftUnlocked;

		[FormerlySerializedAs("LeftLocked")]
		public Sprite leftLocked;

		[FormerlySerializedAs("MidCurrent")]
		[Header("Middle Button")]
		public Sprite midCurrent;

		[FormerlySerializedAs("MidUnlocked")]
		public Sprite midUnlocked;

		[FormerlySerializedAs("MidLocked")]
		public Sprite midLocked;

		[Header("Right Button")]
		[FormerlySerializedAs("RightCurrent")]
		public Sprite rightCurrent;

		[FormerlySerializedAs("RightUnlocked")]
		public Sprite rightUnlocked;

		[FormerlySerializedAs("RightLocked")]
		public Sprite rightLocked;

		public int numberOfLevels => levels.Count;
		public int current { get; private set; }
		public int unlocked { get; private set; }
		private readonly List<Button> levels = new List<Button>();

		private void Awake()
		{
			current = -1;
		}

		public void RecreateButtons(int numOfLevels, int current = 0, int unlocked = 0)
		{
			UISingleton.instance.manusSelectables.RemoveAll(s =>
				s.selectable is Button item && levels.IndexOf(item) != -1);
			foreach (Button b in levels)
			{
				if (b != null)
				{
					Destroy(b.gameObject);
				}
			}

			levels.Clear();

			for (int i = 0; i < numOfLevels; i++)
			{
				Button btn = Instantiate(buttonPrefab, transform, false);

				// TEST
				btn.image.type = Image.Type.Filled;

				btn.name = "Level " + i + " button";

				// Saving it outside the lambda, otherwise it would still give me value of i, which has by that point been changed from the for loop.
				int levelIndex = i;
				btn.onClick.AddListener(() => { ChangeLevel(levelIndex); });

				// Add to list of selectables
				UISingleton.instance.manusSelectables.Add(new UISingleton.ManusSelectable {
					selectable = btn,
					names = new List<string> {"level" + i},
				});

				levels.Add(btn);
			}

			UpdateButtons(current, unlocked);
		}

		public void UpdateButtons(int newCurrent, int newUnlocked)
		{
			unlocked = newUnlocked;
			current = newCurrent;

			for (int i = 0; i < levels.Count; i++)
			{
				UpdateSingleButton(i);
			}
		}

		private void UpdateSingleButton(int levelIndex)
		{
			Button btn = levels[levelIndex];

			string levelId = Main.instance.gameDefinition.activeLevels[levelIndex].levelId;
			LevelData levelData = Progress.Instance.LevelData[levelId];

			if (levelIndex == 0)
			{
				btn.image.sprite = levelIndex == current ? leftCurrent :
					levelIndex <= unlocked || levelData.IsStarted ? leftUnlocked : leftLocked;
				if (numberOfLevels == 1)
				{
					progressBarParent.SetActive(false);
				}
			}
			else if (levelIndex < numberOfLevels - 1)
			{
				btn.image.sprite = levelIndex == current ? midCurrent :
					levelIndex <= unlocked || levelData.IsStarted ? midUnlocked : midLocked;
			}
			else
			{
				btn.image.sprite = levelIndex == current ? rightCurrent :
					levelIndex <= unlocked || levelData.IsStarted ? rightUnlocked : rightLocked;
			}

			btn.interactable = (levelIndex <= unlocked || levelData.IsStarted) && levelIndex != current;

			UITooltip tooltip = btn.GetComponent<UITooltip>();
			if (tooltip)
			{
				tooltip.text = "Nivå " + levelIndex;
				if (levelIndex == current)
				{
					tooltip.text = "<color=green><b>" + tooltip.text + "</b></color> <color=grey>(Nuvarande)</color>";
				}

				if (levelIndex > unlocked && !levelData.IsStarted)
				{
					tooltip.text += " <color=grey>(Låst)</color>";
				}

				tooltip.ApplyTooltipTextChange();
			}
		}

		public void ChangeLevel(int levelIndex)
		{
			if (levelIndex == current)
			{
				return;
			}

			foreach (IPMUnloadLevel ev in UISingleton.FindInterfaces<IPMUnloadLevel>())
			{
				ev.OnPMUnloadLevel();
			}

			unlocked = Mathf.Max(levelIndex, unlocked);

			// Update which one is current one
			UpdateButtons(levelIndex, unlocked);

			Main.instance.StartLevel(levelIndex);
		}
	}
}