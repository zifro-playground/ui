using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEGuideBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("GuideBubble fields")]
		public Text theGuideText;
		public VerticalLayoutGroup theBodyGroup;
		public CanvasGroup okayButton;

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

		protected override void OnShowMessage() {
			okayButton.interactable = true;
			okayButton.blocksRaycasts = true;
			okayButton.alpha = 1;
		}

		protected override void OnHideMessage() {
		}
	}
}