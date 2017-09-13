using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	public class IDERowsLimit : MonoBehaviour {

		readonly Color baseColor = new Color(0.85f,0.85f,0.85f);

		public Text theText;
		public int showWhenRowsLeft = 3;
		public float fadeTime = 0.3f;
		public float flashTime = 0.1f;

		[System.NonSerialized]
		public float redness = 0;
		[System.NonSerialized]
		public bool textVisible = false;
		private float alpha;

		private void Update() {
			Color tmp = theText.color;

			if (redness > 0 && flashTime > 0) {
				tmp = Color.Lerp(baseColor, Color.red, redness);
				redness -= Time.deltaTime / flashTime;
			} else {
				tmp = baseColor;
			}

			if (fadeTime > 0) {
				if (textVisible && alpha < 1)
					alpha += Time.deltaTime / fadeTime;
				else if (!textVisible && alpha > 0)
					alpha -= Time.deltaTime / fadeTime;
			}

			tmp.a = alpha;

			theText.color = tmp;
		}

		public void ValidateRowsLimit(int count, int limit) {
			textVisible = limit - count <= showWhenRowsLeft;
			theText.text = "Rader kvar: " + Mathf.Max(0, limit - count);
		}

	}

}