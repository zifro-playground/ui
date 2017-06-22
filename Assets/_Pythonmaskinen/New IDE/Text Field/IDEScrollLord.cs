using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDEScrollLord : MonoBehaviour {

		public IDETextField theTextField;
		public RectTransform theScrollView;
		public RectTransform theContent;
		public float approxLineHeight = 22;
		public float margin = 12;

		public void FocusOnLineNumber(int lineNumber) {

			if (theContent.sizeDelta.y <= theScrollView.sizeDelta.y) {
				theContent.anchoredPosition = new Vector2(theContent.anchoredPosition.x, 0);
				return;
			}

			float offset = theTextField.inputRect.anchoredPosition.y;
			// Here Y 0 is top, -Y is up and +Y is down
			// Since that's how it is in the contents anchor positions
			float topY = -offset - theTextField.DetermineYOffset(lineNumber);
			float botY = topY + approxLineHeight;

			topY -= margin;
			botY += margin;

			Rect current = new Rect(theContent.anchoredPosition, theScrollView.sizeDelta);
			if (botY > current.yMax) {
				// Move down to line
				theContent.anchoredPosition = new Vector2(current.x, Mathf.Max(Mathf.Min(botY - current.height, theContent.sizeDelta.y - current.height), 0));
			} else if (topY < current.y) {
				// Move up to line
				theContent.anchoredPosition = new Vector2(current.x, Mathf.Max(Mathf.Min(topY, theContent.sizeDelta.y - current.height), 0));
			}
		}
	}

}