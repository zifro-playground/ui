using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PM
{
	public class RunCodeButton : MonoBehaviour, IPMCompilerStarted, IPMCompilerUserPaused, IPMCompilerUserUnpaused,
		IPMCompilerStopped
	{
		[FormerlySerializedAs("PlayImage")]
		public Sprite playImage;

		[FormerlySerializedAs("PauseImage")]
		public Sprite pauseImage;

		[FormerlySerializedAs("ResumeImage")]
		public Sprite resumeImage;

		[FormerlySerializedAs("ThisButton")]
		public Button thisButton;

		// UITooltip Tooltip;

		public void OnRunCodeButtonClick()
		{
			if (PMWrapper.isCompilerRunning)
			{
				UISingleton.instance.walker.SetWalkerUserPaused(!PMWrapper.isCompilerUserPaused);
			}
			else
			{
				PMWrapper.RunCode();
			}
		}

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			//Tooltip.text = "Pausa koden!";
			//Tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = pauseImage;
		}

		void IPMCompilerUserUnpaused.OnPMCompilerUserUnpaused()
		{
			//Tooltip.text = "Pausa koden!";
			//Tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = pauseImage;
		}

		void IPMCompilerUserPaused.OnPMCompilerUserPaused()
		{
			//Tooltip.text = "Kör koden!";
			//Tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = resumeImage;
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			//Tooltip.text = "Kör koden!";
			//Tooltip.ApplyTooltipTextChange();

			thisButton.image.sprite = playImage;
		}
	}
}