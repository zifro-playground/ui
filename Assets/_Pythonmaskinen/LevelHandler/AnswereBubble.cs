using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class AnswereBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("AnswereBubble fields")]
		public Text theAnswereText;
		public Image responseImage;
		public Sprite correct;
		public Sprite wrong;


		public void SetAnswereMessage(string answereMessage, bool correctAnswere) {
			theAnswereText.text = answereMessage;
			ResizeToFit(theAnswereText, bubbleRect);

			responseImage.sprite = correctAnswere ? correct : wrong;
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