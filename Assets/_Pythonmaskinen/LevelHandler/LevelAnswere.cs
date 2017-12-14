using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Compiler;

namespace PM.Level
{
	public class LevelAnswer
	{
		private int parameterAmount;
		private VariableTypes type;
		private string[] answer;

		public bool compilerHasBeenStopped;

		public LevelAnswer(int paramAmount = 0, VariableTypes t = VariableTypes.unknown, string[] ans = null)
		{
			parameterAmount = paramAmount;
			type = t;
			answer = ans ?? new string[0];
		}

		public LevelAnswer(params int[] answers)
		{
			parameterAmount = answers.Length;
			type = VariableTypes.number;
			this.answer = answers.Select(ans => ans.ToString()).ToArray();
		}

		public LevelAnswer(params string[] answers)
		{
			parameterAmount = answers.Length;
			type = VariableTypes.textString;
			this.answer = answers;
		}

		public LevelAnswer(params bool[] answers)
		{
			parameterAmount = answers.Length;
			type = VariableTypes.boolean;
			this.answer = answers.Select(ans => ans.ToString()).ToArray();
		}

		public void CheckAnswer(Variable[] inputParams, int lineNumber)
		{
			if (parameterAmount == 0)
				PMWrapper.RaiseError(lineNumber, "I detta problem behövs inte svara() för att klara problemet");
			if (inputParams.Length < parameterAmount)
				PMWrapper.RaiseError(lineNumber, "För få svar, det ska vara " + parameterAmount + " st svar.");
			if (inputParams.Length > parameterAmount)
				PMWrapper.RaiseError(lineNumber, "För många svar, det ska vara " + parameterAmount + " st svar.");

			foreach (Variable param in inputParams)
			{
				if (param.variableType != type)
				{
					switch (type)
					{
						case VariableTypes.boolean:
							PMWrapper.RaiseError(lineNumber, "Fel typ, svaret ska vara True eller False.");
							break;
						case VariableTypes.number:
							PMWrapper.RaiseError(lineNumber, "Fel typ, svaret ska vara ett tal.");
							break;
						case VariableTypes.textString:
							PMWrapper.RaiseError(lineNumber, "Fel typ, svaret ska vara en text.");
							break;
						default:
							PMWrapper.RaiseError(lineNumber, "Fel typ av svar.");
							break;
					}
				}
			}

			string guess;
			string ans = "";
			bool correctAnswer = true;

			switch (type)
			{
				case VariableTypes.boolean:
					for (int i = 0; i < inputParams.Length; i++)
					{
						guess = inputParams[i].getBool().ToString();

						if (guess != answer[i])
							correctAnswer = false;

						ans += guess;

						if (i < inputParams.Length - 1)
							ans += ", ";
						else
							ans += ".";
					}

					UISingleton.instance.levelHandler.StartCoroutine(ShowAnswerBubble(lineNumber, ans, correctAnswer));
					break;

				case VariableTypes.number:
					for (int i = 0; i < inputParams.Length; i++)
					{
						guess = inputParams[i].getNumber().ToString();

						if (guess != answer[i])
							correctAnswer = false;

						ans += guess;

						if (i < inputParams.Length - 1)
							ans += ", ";
						else
							ans += ".";
					}
					UISingleton.instance.levelHandler.StartCoroutine(ShowAnswerBubble(lineNumber, ans, correctAnswer));
					break;

				case VariableTypes.textString:
					for (int i = 0; i < inputParams.Length; i++)
					{
						guess = inputParams[i].getString().ToString();

						if (guess != answer[i])
							correctAnswer = false;

						ans += guess;

						if (i < inputParams.Length - 1)
							ans += ", ";
						else
							ans += ".";
					}
					UISingleton.instance.levelHandler.StartCoroutine(ShowAnswerBubble(lineNumber, ans, correctAnswer));
					break;
			}
		}

		private IEnumerator ShowAnswerBubble(int lineNumber, string answer, bool correct)
		{
			compilerHasBeenStopped = false;

			UISingleton.instance.answerBubble.ShowMessage(lineNumber);
			UISingleton.instance.answerBubble.SetAnswerMessage("Svar: " + answer);

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			if (compilerHasBeenStopped)
			{
				AbortCase();
				yield break;
			}

			UISingleton.instance.answerBubble.SetAnswerSprite(correct);
			PMWrapper.StopCompiler();
			compilerHasBeenStopped = false;

			yield return new WaitForSeconds(3 * (1 - PMWrapper.speedMultiplier));

			if (compilerHasBeenStopped)
			{
				AbortCase();
				yield break;
			}

			UISingleton.instance.answerBubble.HideMessage();

			if (correct)
			{
				// Call every implemented event
				foreach (var ev in UISingleton.FindInterfaces<IPMCorrectAnswer>())
					ev.OnPMCorrectAnswer(answer);
			}
			else
			{
				UISingleton.instance.levelHandler.currentLevel.caseHandler.CaseFailed();

				// Call every implemented event
				foreach (var ev in UISingleton.FindInterfaces<IPMWrongAnswer>())
					ev.OnPMWrongAnswer(answer);
			}

		}

		private void AbortCase()
		{
			UISingleton.instance.answerBubble.HideMessage();
			UISingleton.instance.levelHandler.currentLevel.caseHandler.ResetHandlerAndButtons();
		}
	}
}