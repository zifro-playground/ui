using UnityEngine;
using UnityEngine.UI;
using PM;

public class TaskDescriptionController : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
{
	public Animator IconAnimator;

	[Header("Big task description")]
	public GameObject BigTaskDescription;
	public Text BigTaskDescriptionHead;
    public Text BigTaskDescriptionBody;

	[Header("Small task description")]
	public GameObject SmallTaskDescription;
	public Text SmallTaskDescriptionText;

	[Header("Positive Feedback")]
	public GameObject PositiveParent;
	public Text PositiveText;

	[Header("Positive Feedback")]
	public GameObject NegativeParent;
	public Text NegativeText;

	private Animator anim;
	private bool hasShownBigTaskDescription = false;

	private void Awake()
	{
		anim = IconAnimator;
	}

	public void SetTaskDescription (string taskDescriptionHead,string taskDescriptionBody)
	{
		BigTaskDescription.SetActive (false);
		if (taskDescriptionHead.Length < 1)
		{
			SmallTaskDescription.SetActive (false);
		}
		else
		{
			SmallTaskDescription.SetActive (true);
			SmallTaskDescriptionText.text = taskDescriptionHead;
			hasShownBigTaskDescription = false; // TODO load from database

			if (!hasShownBigTaskDescription)
			{
				BigTaskDescription.SetActive (true);
                BigTaskDescriptionHead.text = taskDescriptionHead;
                BigTaskDescriptionBody.text = taskDescriptionBody;
				hasShownBigTaskDescription = true;
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
