using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace PM {
	[RequireComponent(typeof(RectTransform))]
	public class SmartButtonController : MonoBehaviour {

		public SmartButton buttonPrefab;
		public RectTransform container { get { return transform as RectTransform; } }
		public Image background;
		public ScrollRect scrollRect;

		private List<SmartButton> buttons = new List<SmartButton>();
		private Vector2 offset;
		public float padding = 10;
		public float margin = 10;

		private void Awake() {
			offset = Vector2.down * margin;
		}

		private void CompileButtonText(string input, out string rawButtonText, out string rawCallbackCode) {
			rawButtonText = rawCallbackCode = null;

			// Seperate string
			// Man I'm bad at Regex. Looks messy but works /kalle
			Match match = Regex.Match(input.Trim(), @"^(?:(\w*)\s)?(?:(\w+)(\((.+)?\))?)$");

			if (!match.Success) {
				rawButtonText = rawCallbackCode = input;
				return;
			}

			//for (int i=0;i<match.Groups.Count;i++) {
			//	print("GROUP[" + i + "] = success:" + match.Groups[i].Success.ToString() + (match.Groups[i].Success ? " value:" + match.Groups[i].Value : string.Empty));
			//}

			string type = match.Groups[1].Success ? match.Groups[1].Value : null;
			string name = match.Groups[2].Value; // Group 2 is required so no need to check if successful
			bool paranteses = match.Groups[3].Success;
			string argType = match.Groups[4].Success ? match.Groups[4].Value : null;

			// Put together button text
			rawButtonText = "";

			if (type != null)
				rawButtonText += "<color=#de5170>" + type + "</color> ";
			rawButtonText += "<b>" + name + "</b>";
			if (paranteses) {
				rawButtonText += "(";
				if (argType != null)
					rawButtonText += "<color=#de5170>" + argType + "</color>";
				rawButtonText += ")";
			}

			// Put together callback text
			rawCallbackCode = "";

			rawCallbackCode += name;
			if (paranteses)
				rawCallbackCode += "()";
		}

		public void AddSmartButton(string textToBeCompiled) {
			// Using the Rich Text property
			string rawButtonText, rawCallbackCode;
			CompileButtonText(textToBeCompiled, out rawButtonText, out rawCallbackCode);
			AddSmartButton(rawButtonText, rawCallbackCode);
		}

		public void AddSmartButton(string rawButtonText, string rawCallbackCode) {
			// Setting the prefabs text
			// because setting the clones doesnt update the preferredWidth instantly
			buttonPrefab.text.text = rawButtonText;

			var clone = Instantiate(buttonPrefab);
			clone.transform.SetParent(container, false);

			clone.name = "[" + rawCallbackCode + "]";

			clone.button.onClick.AddListener(() => {
				// All variables used in here will be excluded from the GC
				// They're kept in memory as long as the callback exists, which is as long as the button exists.
				PMWrapper.AddCode(rawCallbackCode, true);
			});

			float width = clone.text.preferredWidth + padding * 2; // x2 for left and right padding
			float height = clone.rect.rect.height;

			// Check if fits
			if (offset.x + width > container.rect.width)
				// Move to next line
				offset = new Vector2(0, offset.y - height - margin);

			// min.x = west
			// min.y = south
			// max.x = east
			// max.y = north
			clone.rect.offsetMin = new Vector2(offset.x, offset.y - height);
			clone.rect.offsetMax = new Vector2(offset.x + width, offset.y);

			offset.x += width + margin;
			container.sizeDelta = new Vector2(container.sizeDelta.x, height + margin - offset.y);

			// Enable background if needs scrollbar
			background.enabled = container.sizeDelta.y > background.rectTransform.sizeDelta.y;

			buttons.Add(clone);
			UISingleton.instance.manusSelectables.Add(new UISingleton.ManusSelectable { selectable = clone.button, names = new List<string> { "sb-" + rawCallbackCode } });
		}

		public void ClearSmartButtons() {
			UISingleton.instance.manusSelectables.RemoveAll(s => s.selectable is Button && buttons.FindIndex(b=>b.button == s.selectable as Button) != -1);
			buttons.ForEach(b => Destroy(b.gameObject));
			buttons.Clear();
			scrollRect.verticalNormalizedPosition = 1;
			offset = Vector2.down * margin;
		}

	}

}