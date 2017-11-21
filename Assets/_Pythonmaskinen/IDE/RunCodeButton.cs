using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace PM {

	public class RunCodeButton : MonoBehaviour, IPMCompilerStarted, IPMCompilerUserPaused, IPMCompilerUserUnpaused, IPMCompilerStopped {

		public Sprite playImage;
		public Sprite pauseImage;
		public Sprite resumeImage;
		public UITooltip tooltip;

		public Button thisButton;

		public void onRunCodeButtownClick() {
			if (PMWrapper.isCompilerRunning) {
				UISingleton.instance.walker.SetWalkerUserPaused(!PMWrapper.isCompilerUserPaused);
			} else {
				PMWrapper.RunCode();
			}
		}

		void IPMCompilerStarted.OnPMCompilerStarted() {
			tooltip.text = "Pausa koden!";
			tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = pauseImage;
		}

		void IPMCompilerUserUnpaused.OnPMCompilerUserUnpaused() {
			tooltip.text = "Pausa koden!";
			tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = pauseImage;
		}

		void IPMCompilerUserPaused.OnPMCompilerUserPaused() {
			tooltip.text = "Kör koden!";
			tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = resumeImage;
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status) {
			tooltip.text = "Kör koden!";
			tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = playImage;
		}
	}

}