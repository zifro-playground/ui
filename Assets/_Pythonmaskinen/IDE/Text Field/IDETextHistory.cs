using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
	public class IDETextHistory
	{
		private readonly List<string> history = new List<string>();
		private int currentIndex = -1;

		//Runs every Update, but only saves if something has changed
		public void SaveText(string currentText)
		{
			if (currentText != GetLatestHistory())
			{
				//If something has changed it cheks if it is currently at the end of the history
				//If this is not the case we rewrite history and forgets the old history
				if (history.Count > currentIndex)
				{
					string[] saveHistory = history.GetRange(0, currentIndex + 1).ToArray();
					history.Clear();
					history.AddRange(saveHistory);
				}

				history.Add(currentText);
				currentIndex++;
			}
		}

		public string StepBackInHistory()
		{
			if (currentIndex <= 0)
			{
				currentIndex = -1;
				return "";
			}

			return history[--currentIndex];
		}

		public string StepForwardInHistory()
		{
			if (history.Count > currentIndex + 1)
			{
				return history[++currentIndex];
			}

			return history[currentIndex];
		}

		private string GetLatestHistory()
		{
			if (currentIndex == -1)
			{
				return "";
			}

			return history[currentIndex];
		}
	}
}