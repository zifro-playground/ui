using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class AnswerBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("AnswerBubble fields")]
		public Text theAnswerText;
		public Image responseImage;
		public Sprite correct;
		public Sprite wrong;


		public void SetAnswerMessage(string answerMessage) {
			theAnswerText.text = answerMessage;
			ResizeToFit(theAnswerText, bubbleRect);
			responseImage.enabled = false;
		}

		public void SetAnswerSprite (bool correctAnswer){
			responseImage.enabled = true;
			responseImage.sprite = correctAnswer ? correct : wrong;
		}

		void IPMLevelChanged.OnPMLevelChanged() {
			HideMessageInstantly ();
		}

		protected override void OnShowMessage() {
		}

		protected override void OnHideMessage() {
			//clooose it
		}
	}
}