using UnityEngine;
using UnityEngine.UI;
using PM;

public class TaskDescription : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
{
	[Header("Object references")]
	public GameObject bigTDParent;
	public GameObject smallTDParent;
	public GameObject feedbackParent;
	public GameObject iconObject;

	[Header("Text references")]
	public Text taskDescriptionSmall;
	public Text taskDescriptionBig;
	public Text feedbackText;

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
		feedbackParent.SetActive(true);
		feedbackText.text = message;

		Animator anim = iconObject.GetComponent<Animator>();
		anim.SetTrigger("Jump");
	}

	public void HideTaskError()
	{
		feedbackParent.SetActive(false);
	}

	public void OnPMLevelChanged()
	{
		HideTaskError();
	}
	public void OnPMCompilerStarted()
	{
		HideTaskError();
	}
}
