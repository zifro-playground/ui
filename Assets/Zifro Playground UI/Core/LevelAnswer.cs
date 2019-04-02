using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mellis.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
	public abstract class LevelAnswer
	{
		public bool compilerHasBeenStopped;

		protected IEnumerator ShowAnswerBubble(string answer, bool correct)
		{
			compilerHasBeenStopped = false;

			UISingleton.instance.answerBubble.ShowMessage();
			UISingleton.instance.answerBubble.SetAnswerMessage("Svar: " + answer);

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			if (compilerHasBeenStopped)
			{
				Debug.LogError("Compiler stopped prematurely.");
				AbortCase();
				yield break;
			}

			if (correct)
			{
				// Call every implemented event
				foreach (IPMCorrectAnswer ev in UISingleton.FindInterfaces<IPMCorrectAnswer>())
				{
					ev.OnPMCorrectAnswer(answer);
				}
			}
			else
			{
				Main.instance.caseHandler.CaseFailed();

				// Call every implemented event
				foreach (IPMWrongAnswer ev in UISingleton.FindInterfaces<IPMWrongAnswer>())
				{
					ev.OnPMWrongAnswer(answer);
				}
			}
		}

		private void AbortCase()
		{
			UISingleton.instance.answerBubble.HideMessage();
			Main.instance.caseHandler.ResetHandlerAndButtons();
		}

		public abstract void CheckAnswer(IScriptType[] inputParams);
	}

	public class LevelAnswer<T> : LevelAnswer
	{
		private readonly T[] expectedInputs;

		public LevelAnswer(params T[] expectedInputs)
		{
			Debug.LogFormat("Expecting answers: {0}", string.Join(", ", expectedInputs));
			this.expectedInputs = expectedInputs;
		}

		public override void CheckAnswer(IScriptType[] inputParams)
		{
			int answersLength = expectedInputs.Length;
			if (answersLength == 0)
			{
				PMWrapper.RaiseError("I detta problem behövs inte svara() för att klara problemet");
				return;
			}

			if (inputParams.Length < answersLength)
			{
				PMWrapper.RaiseError($"För få svar, det ska vara {answersLength} st svar.");
				return;
			}

			if (inputParams.Length > answersLength)
			{
				PMWrapper.RaiseError($"För många svar, det ska vara {answersLength} st svar.");
				return;
			}

			bool correctAnswer = true;

			for (int i = 0; i < answersLength; i++)
			{
				IScriptType input = inputParams[i];
				T expected = expectedInputs[i];

				if (!input.TryConvert(out T actual))
				{
					if (typeof(T) == typeof(bool))
					{
						PMWrapper.RaiseError($"Fel typ, svar nr {i + 1} ska vara True eller False.");
					}
					else if (typeof(T) == typeof(int))
					{
						PMWrapper.RaiseError($"Fel typ, svar nr {i + 1} ska vara ett heltal.");
					}
					else if (typeof(T) == typeof(double))
					{
						PMWrapper.RaiseError($"Fel typ, svar nr {i + 1} ska vara ett tal.");
					}
					else if (typeof(T) == typeof(string))
					{
						PMWrapper.RaiseError($"Fel typ, svar nr {i + 1} ska vara en textsträng.");
					}
					else
					{
						PMWrapper.RaiseError($"Fel typ på svar nr {i + 1}.");
					}
				}

				if (!expected.Equals(actual))
				{
					correctAnswer = false;
					break;
				}
			}

			string joined = string.Join(", ", inputParams.Select(o => o.ToString()));

			Main.instance.StartCoroutine(ShowAnswerBubble(joined, correctAnswer));
		}
	}
}