using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace PM {

	public class GlobalSpeed : MonoBehaviour {

		public Slider speedSlider;
		public float currentSpeed {
			get { return speedSlider.value; }
			set { speedSlider.value = Mathf.Clamp01(value); }
		}

		// Use this for initialization
		void Start() {
			speedSlider.onValueChanged.AddListener(delegate { valueChanged(); });
			valueChanged();
		}

		// Listener
		public void valueChanged() {
			foreach (var ev in UISingleton.FindInterfaces<IPMSpeedChanged>())
				ev.OnPMSpeedChanged(currentSpeed);
		}
	}
}