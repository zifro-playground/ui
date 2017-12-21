using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class WinScreen : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted {

		[Header("UI elements")]
		public GameObject topmostObject;
		public Button continueButton;
		public Button closeButton;
		public Text winText;
		public CanvasGroup canvasGroup;
		[Header("Settings")]
		[TextArea]
		public string winMsg;
		[TextArea]
		public string lastWinMsg;
		[TextArea]
		public string demoWinMsg;
		public float fadeTime = 0.5f;

		private float timePassed = 0;
		private State state = State.Idle;

		public void OnPMLevelChanged(){
			CloseWinScreen ();
		}

		public void OnPMCompilerStarted(){
			CloseWinScreen ();
		}

		public void SetLevelCompleted() {
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
		public void CloseWinScreen() {
			_HideWinScrren();

			//if (!SaveData.gottenLevelTip) {
			//	SaveData.gottenLevelTip = true;
			//	UISingleton.instance.manusBubble.SetMessageContent("Nivåfältet","Här kan du klicka fram och tillbaka mellan nivåerna!\n\nDu kan även hoppa tillbaka till en gammal nivå för att se din tidigare lösning.");
			//	UISingleton.instance.manusBubble.ShowMessage(UISingleton.instance.manusSelectables.First(x=>x.names.Contains("level0")).selectable.transform as RectTransform);
			//}
		}

		// Called by UnityActions on continueButton
		public void ContinueToNextLevel() {
			_HideWinScrren();
			PMWrapper.currentLevel += 1;
		}

		private void _HideWinScrren() {
			state = State.Hide;
			timePassed = 0;
			this.enabled = true;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
		}

		private void _ShowWinScreen() {
			continueButton.gameObject.SetActive(PMWrapper.currentLevel < PMWrapper.numOfLevels - 1);

			if (PMWrapper.IsDemoLevel) winText.text = demoWinMsg;
			else if (continueButton.gameObject.activeSelf) winText.text = winMsg;
			else winText.text = lastWinMsg;

			UISingleton.instance.levelHints.StartHideFading();
			topmostObject.SetActive(true);
			state = State.Show;
			timePassed = 0;
			this.enabled = true;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}

		private void Update() {
			if (state == State.Idle) return;

			timePassed += Time.deltaTime;

			if (timePassed > fadeTime) {
				// Done fading

				// Reset them
				canvasGroup.alpha = state == State.Show ? 1 : 0;

				if (state == State.Hide)
					topmostObject.SetActive(false);

				state = State.Idle;
				this.enabled = false;
			} else {
				// Fade them
				canvasGroup.alpha = state == State.Show
					? timePassed / fadeTime
					: 1 - timePassed / fadeTime;
			}
		}
		
		private enum State {
			Idle,
			Show,
			Hide
		}

	}

}