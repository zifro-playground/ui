using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PM {

	public class IDEInputField : InputField {

		// This disables it to be deselected ^^
		//public override void OnDeselect(BaseEventData eventData) { }
		//protected override void OnDisable() { }

		public override void OnDrag(PointerEventData eventData) {
			base.OnDrag(eventData);

			PMWrapper.FocusOnLineNumber();
		}

		public override void OnPointerClick(PointerEventData eventData) {
			base.OnPointerClick(eventData);

			PMWrapper.FocusOnLineNumber();
		}

	}

}