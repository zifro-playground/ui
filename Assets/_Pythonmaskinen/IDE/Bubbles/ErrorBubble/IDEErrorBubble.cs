using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEErrorBubble : AbstractPopupBubble, IPMLevelChanged {

		[Header("ErrorBubble fields")]
		public Text theErrorText;
		
		private IDELineMarker theMarker;

		public void init(IDELineMarker theMarker) {
			this.theMarker = theMarker;
		}

		public void SetErrorMessage(string errorMessage) {
			theErrorText.text = errorMessage;
			ResizeToFit(theErrorText, bubbleRect);
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