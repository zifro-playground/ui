using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Compiler;

namespace PM.Level {

	public class LevelAnswere {

		int parameterAmount;
		VariableTypes type;
		string[] answere;

		public LevelAnswere (int paramAmount = 0, VariableTypes t = VariableTypes.unknown, string[] ans = null){
			parameterAmount = paramAmount;
			type = t;
			answere = ans ?? new string[0];
		}

		public void CheckAnswere (Variable[] inputParams, int lineNumber){
			if (parameterAmount == 0)
				PMWrapper.RaiseError (lineNumber, "I detta problem behövs inte svara() för att klara problemet");
			if (inputParams.Length < parameterAmount)
				PMWrapper.RaiseError (lineNumber, "För få svar, det ska vara " + parameterAmount + " st svar.");
			if (inputParams.Length > parameterAmount)
				PMWrapper.RaiseError (lineNumber, "För många svar, det ska vara " + parameterAmount + " st svar.");

			foreach (Variable param in inputParams) {
				if (param.variableType != type){
					switch (type){
						case VariableTypes.boolean: 
							PMWrapper.RaiseError (lineNumber, "Fel typ, svaret ska vara True eller False.");
							break;
					case VariableTypes.number:
						PMWrapper.RaiseError (lineNumber, "Fel typ, svaret ska vara ett tal.");
						break;
					case VariableTypes.textString:
						PMWrapper.RaiseError (lineNumber, "Fel typ, svaret ska vara en text.");
						break;
					default:
						PMWrapper.RaiseError (lineNumber, "Fel typ av svar.");
						break;
					}
				}
			}
				
			string guess;
			string ans = "";
			bool correctAnswere = true;

			switch (type) {
			case VariableTypes.boolean:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getBool ().ToString ();

					if (guess != answere [i])
						correctAnswere = false;

					ans += guess;

					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}

				UISingleton.instance.levelHandler.StartCoroutine (ShowAnswereBubble(lineNumber, ans, correctAnswere));
				break;

			case VariableTypes.number:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getNumber ().ToString ();

					if (guess != answere [i])
						correctAnswere = false;

					ans += guess;
					
					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}
				UISingleton.instance.levelHandler.StartCoroutine (ShowAnswereBubble(lineNumber, ans, correctAnswere));
				break;

			case VariableTypes.textString:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getString ().ToString ();

					if (guess != answere [i])
						correctAnswere = false;
					
					ans += guess;

					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}
				UISingleton.instance.levelHandler.StartCoroutine (ShowAnswereBubble(lineNumber, ans, correctAnswere));
				break;
			}
		}

		private IEnumerator ShowAnswereBubble (int lineNumber, string answere, bool correct){

			UISingleton.instance.answereBubble.ShowMessage (lineNumber);
			UISingleton.instance.answereBubble.SetAnswerMessage("Svar: " + answere);

			yield return new WaitForSeconds (3 * (1 - PMWrapper.speedMultiplier));
			UISingleton.instance.answereBubble.SetAnswereSprite (correct);
			
			yield return new WaitForSeconds (3 * (1 - PMWrapper.speedMultiplier));

			PMWrapper.StopCompiler ();
			UISingleton.instance.answereBubble.HideMessage ();

			if (correct)
				PMWrapper.SetCaseCompleted ();
			else
				UISingleton.instance.levelHandler.currentLevel.caseHandler.CaseFailed ();

		}
	}
}