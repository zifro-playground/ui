using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEGuideBubble : AbstractPopupBubble {

		[Header("GuideBubble fields")]
		public Text theGuideText;

		private IDELineMarker theMarker;

		public void init(IDELineMarker theMarker) {
			this.theMarker = theMarker;
		}

		public void SetGuideMessage(string guideMessage) {
			theGuideText.text = guideMessage;
			ResizeToFit(theGuideText, bubbleRect);
		}

		void IPMLevelChanged.OnPMLevelChanged() {
			// Fade away on level change
			HideMessage();
		}

		protected override void OnShowMessage() {}

		protected override void OnHideMessage() {
			theMarker.SetState(IDELineMarker.State.Hidden);
			UISingleton.instance.textField.theLineMarker.removeErrorMessage();
		}
	}
}