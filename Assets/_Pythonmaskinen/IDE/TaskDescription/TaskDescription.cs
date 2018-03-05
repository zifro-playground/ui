using UnityEngine;
using UnityEngine.UI;
using PM;

public class TaskDescription : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
{
	[Header("Object references")]
	public GameObject bigTDParent;
	public GameObject smallTDParent;
	public GameObject iconObject;

	[Header("Text references")]
	public Text taskDescriptionSmall;
	public Text taskDescriptionBig;

	[Header("Positive Feedback")]
	public GameObject PositiveParent;
	public Text PositiveText;

	[Header("Positive Feedback")]
	public GameObject NegativeParent;
	public Text NegativeText;

	private Animator anim;

	private void Awake()
	{
		anim = iconObject.GetComponent<Animator>();
	}

	public void SetTaskDescription (string taskDescription)
	{
		bigTDParent.SetActive (false);
		if (taskDescription.Length < 1)
		{
			smallTDParent.SetActive (false);
		}
		else
		{
			smallTDParent.SetActive (true);
			taskDescriptionSmall.text = taskDescription;
			if (!UISingleton.instance.levelHandler.currentLevel.hasShownTaskDescription)
			{
				bigTDParent.SetActive (true);
				taskDescriptionBig.text = taskDescription;
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
