using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PM
{
	public class TaskDescriptionController : MonoBehaviour, IPMLevelChanged, IPMCompilerStarted
	{
		[FormerlySerializedAs("IconAnimator")]
		public Animator iconAnimator;

		[Header("Big task description")]
		[FormerlySerializedAs("BigTaskDescription")]
		public GameObject bigTaskDescription;

		[FormerlySerializedAs("BigTaskDescriptionHead")]
		public Text bigTaskDescriptionHead;

		[FormerlySerializedAs("BigTaskDescriptionBody")]
		public Text bigTaskDescriptionBody;

		[Header("Small task description")]
		[FormerlySerializedAs("SmallTaskDescription")]
		public GameObject smallTaskDescription;

		[FormerlySerializedAs("ReadMoreButton")]
		public GameObject readMoreButton;

		[FormerlySerializedAs("SmallTaskDescriptionText")]
		public Text smallTaskDescriptionText;

		[Header("[+] Positive Feedback")]
		[FormerlySerializedAs("PositiveParent")]
		public GameObject positiveParent;

		[FormerlySerializedAs("PositiveText")]
		public Text positiveText;

		[Header("[-] Negative Feedback")]
		[FormerlySerializedAs("NegativeParent")]
		public GameObject negativeParent;

		[FormerlySerializedAs("NegativeText")]
		public Text negativeText;

		Animator anim;
		static readonly int Shake = Animator.StringToHash("Shake");
		static readonly int Jump = Animator.StringToHash("Jump");

		void Awake()
		{
			anim = iconAnimator;
		}

		public void SetTaskDescription(string header, string body)
		{
			bigTaskDescription.SetActive(false);
			if (header.Length < 1)
			{
				smallTaskDescription.SetActive(false);
			}
			else
			{
				SetSmallTaskDescription(header, body);
				SetBigTaskDescription(header, body);
			}
		}

		private void SetSmallTaskDescription(string header, string body)
		{
			smallTaskDescription.SetActive(true);
			smallTaskDescriptionText.text = header;
			if (string.IsNullOrEmpty(body))
			{
				readMoreButton.SetActive(false);
			}
			else
			{
				readMoreButton.SetActive(true);
			}
		}

		private void SetBigTaskDescription(string header, string body)
		{
			bigTaskDescriptionHead.text = header;
			bigTaskDescriptionBody.text = body;

			LevelData levelData = Progress.Instance.LevelData[PMWrapper.currentLevel.id];

			if (!levelData.HasShownDescription && !levelData.IsCompleted)
			{
				bigTaskDescription.SetActive(true);
				levelData.HasShownDescription = true;
			}
		}

		public void ShowTaskError(string message)
		{
			negativeParent.SetActive(true);
			negativeText.text = message;

			anim.SetTrigger(Shake);
		}

		public void HideTaskFeedback()
		{
			negativeParent.SetActive(false);
			positiveParent.SetActive(false);
		}

		public void ShowPositiveMessage(string message)
		{
			positiveParent.SetActive(true);
			positiveText.text = message;

			anim.SetTrigger(Jump);
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
}