using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public class AnswerBubble : MonoBehaviour, IPMLevelChanged //, AbstractPopupBubble
	{
		[Header("Colors")]
		[Range(0, 255)]
		public float hidingAlpha = 50;
		[Range(0, 255)]
		public float showingAlpha = 255;

		[Header("AnswerBubble fields")]
		public GameObject answerParent;
		public Text answerText;
		public Image bubbleImage;
		public Sprite correct;
		public Sprite wrong;


		public void SetAnswerMessage(string answerMessage)
		{
			answerText.text = answerMessage;
		}

		void IPMLevelChanged.OnPMLevelChanged()
		{
			HideMessage();

			if (PMWrapper.levelShouldBeAnswered)
			{
				foreach (Transform t in transform)
					t.gameObject.SetActive(true);
			}
			else
			{
				foreach (Transform t in transform)
					t.gameObject.SetActive(false);
			}

		}

		private void OnShowMessage()
		{
			Color newColor = bubbleImage.color;
			newColor.a = showingAlpha / 255;
			bubbleImage.color = newColor;
		}

		private void OnHideMessage()
		{
			Color newColor = bubbleImage.color;
			newColor.a = hidingAlpha / 255;
			bubbleImage.color = newColor;
		}

		// All methods below might be abstract methods later
		public void ShowMessage(int lineNumber)
		{
			answerText.enabled = true;

			OnShowMessage();
		}

		public void HideMessage()
		{
			answerText.enabled = false;

			OnHideMessage();
		}
	}
}