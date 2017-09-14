using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEGuideBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("GuideBubble fields")]
		public Text theGuideText;


		public void SetGuideMessage(string guideMessage) {
			theGuideText.text = guideMessage;
			ResizeToFit(theGuideText, bubbleRect);
		}

		void IPMLevelChanged.OnPMLevelChanged() {
			// Fade away on level change
			HideMessageInstantly();
		}

		protected override void OnShowMessage() {
		}

		protected override void OnHideMessage() {
		}
	}
}