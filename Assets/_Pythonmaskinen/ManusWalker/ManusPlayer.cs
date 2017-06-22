using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Manus {

	public class ManusPlayer : MonoBehaviour, IPMLevelChanged, IPMCompilerStopped {

		public static bool isPlaying { get; private set; }
		public static Manus currentManus;

		public Button skipManusBubble;

		public void SetIsManusPlaying(bool state) {
			if (state == isPlaying) return;

			isPlaying = state;
			UISingleton.instance.uiCanvasGroup.blocksRaycasts = !state;
			skipManusBubble.gameObject.SetActive(state);
			PMWrapper.speedMultiplier = 0.5f;

			if (state) {
				UISingleton.instance.textField.deActivateField();
				IDELineMarker.instance.SetState(IDELineMarker.State.Hidden);
				currentManus = Loader.allManuses[PMWrapper.currentLevel].Copy();
				currentManus.NextStep();
			} else {
				if (currentManus != null) {
					currentManus.FastForwardAllSteps();
					currentManus = null;
				}

				UISingleton.instance.textField.reActivateField();
				UISingleton.instance.manusBubble.HideMessage();

				PMWrapper.SetLevelCompleted();
				IDELineMarker.SetIDEPosition(PMWrapper.preCode == string.Empty ? 1 : (PMWrapper.preCode.Split('\n').Length + 1));
			}
		}

		private void FixedUpdate() {
			if (currentManus != null) {
				currentManus.UpdateSteps();

				if (currentManus.allStepsDone) {
					currentManus = null;

					// Enable UI
					SetIsManusPlaying(false);
				}
			}
		}
		
		void IPMLevelChanged.OnPMLevelChanged() {
			if (!PMWrapper.isDemoLevel) {
				SetIsManusPlaying(false);
			}
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status) {
			if (status == HelloCompiler.StopStatus.RuntimeError)
				SetIsManusPlaying(false);
		}
	}

}
