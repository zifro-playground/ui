using UnityEngine;
using UnityEngine.UI;
using PM;

public class TaskDescription : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
{
	[Header("Object references")]
	public GameObject bigTDParent;
	public GameObject smallTDParent;
	public GameObject feedbackParent;
	public Image FeedbackImage;
	public GameObject iconObject;

	[Header("Feedback icons")]
	public Sprite IconPositive;
	public Sprite IconNegative;

	[Header("Text references")]
	public Text taskDescriptionSmall;
	public Text taskDescriptionBig;
	public Text feedbackText;

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
		feedbackParent.SetActive(true);
		feedbackText.text = message;
		FeedbackImage.sprite = IconNegative;

		anim.SetTrigger("Shake");
	}

	public void HideTaskFeedback()
	{
		feedbackParent.SetActive(false);
	}

	public void ShowPositiveMessage(string message)
	{
		feedbackParent.SetActive(true);
		feedbackText.text = message;
		FeedbackImage.sprite = IconPositive;

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
