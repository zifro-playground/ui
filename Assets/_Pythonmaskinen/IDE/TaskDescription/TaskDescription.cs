using UnityEngine;
using UnityEngine.UI;
using PM;

public class TaskDescription : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
{
	public Animator IconAnimator;

	[Header("Big task description")]
	public GameObject LevelStartParent;
	public Text LevelStartText;

	[Header("Small task description")]
	public GameObject TaskBarParent;
	public Text TaskBarText;

	[Header("Positive Feedback")]
	public GameObject PositiveParent;
	public Text PositiveText;

	[Header("Positive Feedback")]
	public GameObject NegativeParent;
	public Text NegativeText;

	private Animator anim;

	private void Awake()
	{
		anim = IconAnimator;
	}

	public void SetTaskDescription (string taskDescription)
	{
		LevelStartParent.SetActive (false);
		if (taskDescription.Length < 1)
		{
			TaskBarParent.SetActive (false);
		}
		else
		{
			TaskBarParent.SetActive (true);
			TaskBarText.text = taskDescription;
			if (!UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription)
			{
				LevelStartParent.SetActive (true);
				LevelStartText.text = taskDescription;
				UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription = true;
			}
		}

	}

	public void ShowTaskError(string message)
	{
		NegativeParent.SetActive(true);
		NegativeText.text = message;

		anim.SetTrigger("Shake");
	}

	public void HideTaskFeedback()
	{
		NegativeParent.SetActive(false);
		PositiveParent.SetActive(false);

	}

	public void ShowPositiveMessage(string message)
	{
		PositiveParent.SetActive(true);
		PositiveText.text = message;

		anim.SetTrigger("Jump");
	}

	public void OnPMLevelChanged()
	{
		HideTaskFeedback();
	}
	public void OnPMCompilerStarted()
	{
		HideTaskFeedback();
	}
}
