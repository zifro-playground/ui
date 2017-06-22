using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	public class IDETextHistory {

		private List<string> history = new List<string>();
		private int currentIndex = -1;

		//Runs every Update, but only saves if something has changed
		public void saveText(string currentText) {
			if (currentText != getLatestHistory()) {
				//If something has changed it cheks if it is currently at the end of the history
				//If this is not the case we rewrite history and forgets the old history
				if (history.Count > currentIndex) {
					string[] saveHistory = history.GetRange (0, currentIndex + 1).ToArray();
					history.Clear();
					history.AddRange(saveHistory);
				}

				history.Add(currentText);
				currentIndex++;
			}
		}

		public string stepBackInHistory() {
			if (currentIndex <= 0) {
				currentIndex = -1;
				return "";
			}

			return history[--currentIndex];
		}

		public string stepForwardInHistory() {
			if (history.Count > currentIndex + 1)
				return history[++currentIndex];

			return history[currentIndex];
		}


		private string getLatestHistory() {
			if (currentIndex == -1)
				return "";

			return history[currentIndex];
		}
	}

}