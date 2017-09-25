using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PM.Guide;

namespace PM {

	public class IDEGuideBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("GuideBubble fields")]
		public Text theGuideText;


		public void SetGuideMessage(string guideMessage) {
			theGuideText.text = guideMessage;
			ResizeToFit(theGuideText, bubbleRect);
		}

		void IPMLevelChanged.OnPMLevelChanged() {
			HideMessageInstantly ();
		}

		protected override void OnShowMessage() {
		}

		protected override void OnHideMessage() {
			if(UISingleton.instance.guidePlayer.currentGuide != null)
			if (UISingleton.instance.guidePlayer.currentGuide.hasNext)
				UISingleton.instance.guidePlayer.shouldPlayNext = true;
		}
	}
}