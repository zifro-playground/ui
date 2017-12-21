using UnityEngine;
using UnityEngine.UI;

namespace PM
{

	public class RunCodeButton : MonoBehaviour, IPMCompilerStarted, IPMCompilerUserPaused, IPMCompilerUserUnpaused, IPMCompilerStopped
	{

		public Sprite PlayImage;
		public Sprite PauseImage;
		public Sprite ResumeImage;

		public UITooltip Tooltip;
		public Button ThisButton;

		public void OnRunCodeButtownClick()
		{
			if (PMWrapper.IsCompilerRunning)
			{
				UISingleton.instance.walker.SetWalkerUserPaused(!PMWrapper.IsCompilerUserPaused);
			}
			else
			{
				PMWrapper.RunCode();
			}
		}

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			Tooltip.text = "Pausa koden!";
			Tooltip.ApplyTooltipTextChange();

			ThisButton.image.sprite = PauseImage;
		}

		void IPMCompilerUserUnpaused.OnPMCompilerUserUnpaused()
		{
			Tooltip.text = "Pausa koden!";
			Tooltip.ApplyTooltipTextChange();

			ThisButton.image.sprite = PauseImage;
		}

		void IPMCompilerUserPaused.OnPMCompilerUserPaused()
		{
			Tooltip.text = "Kör koden!";
			Tooltip.ApplyTooltipTextChange();

			ThisButton.image.sprite = ResumeImage;
		}

		void IPMCompilerStopped.OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			Tooltip.text = "Kör koden!";
			Tooltip.ApplyTooltipTextChange();

			ThisButton.image.sprite = PlayImage;
		}
	}

}