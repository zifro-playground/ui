using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class WinScreen : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
	{
		[Header("UI elements")]
		public Button ContinueButton;
		public Button CloseButton;
		public Text LevelFinishedText;
		public Text GameFinishedText;

		public void OnPMLevelChanged()
		{
			CloseWinScreen();
		}

		public void OnPMCompilerStarted()
		{
			CloseWinScreen();
		}

		public void SetLevelCompleted()
		{
			PMWrapper.StopCompiler();

			foreach (var ev in UISingleton.FindInterfaces<IPMLevelCompleted>())
				ev.OnPMLevelCompleted();

			// If theres more levels && next level is locked
			if (PMWrapper.currentLevel < PMWrapper.numOfLevels - 1 && PMWrapper.currentLevel == PMWrapper.unlockedLevel)
				// Unlock next level
				UISingleton.instance.levelbar.UpdateButtons(PMWrapper.currentLevel, PMWrapper.currentLevel + 1);

			_ShowWinScreen();
		}

		// Called by UnityActions on closeButton
		public void CloseWinScreen()
		{
			_HideWinScrren();
		}

		// Called by UnityActions on continueButton
		public void ContinueToNextLevel()
		{
			_HideWinScrren();
			PMWrapper.currentLevel += 1;
		}

		private void _HideWinScrren()
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}

		private void _ShowWinScreen()
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
			}

			if (PMWrapper.currentLevel < PMWrapper.numOfLevels - 1)
			{
				ContinueButton.gameObject.SetActive(true);
				GameFinishedText.gameObject.SetActive(false);
			}
			else
			{
				ContinueButton.gameObject.SetActive(false);
				GameFinishedText.gameObject.SetActive(true);
			}
		}
	}
}