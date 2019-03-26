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

			// If theres more levels && next level is locked
			if (PMWrapper.currentLevelIndex < PMWrapper.numOfLevels - 1 && PMWrapper.currentLevelIndex == PMWrapper.unlockedLevel)
				// Unlock next level
				UISingleton.instance.levelbar.UpdateButtons(PMWrapper.currentLevelIndex, PMWrapper.currentLevelIndex + 1);

			_ShowWinScreen();

			foreach (IPMLevelCompleted ev in UISingleton.FindInterfaces<IPMLevelCompleted>())
				ev.OnPMLevelCompleted();
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
			PMWrapper.currentLevelIndex += 1;
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

			if (PMWrapper.currentLevelIndex < PMWrapper.numOfLevels - 1)
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