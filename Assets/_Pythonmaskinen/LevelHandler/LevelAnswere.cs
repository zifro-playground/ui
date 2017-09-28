using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compiler;

namespace PM.Level {

	public class LevelAnswere {

		int parameterAmount;
		VariableTypes type;
		string[] answere;

		public LevelAnswere (int paramAmount, VariableTypes t, string[] ans){
			parameterAmount = paramAmount;
			type = t;
			answere = ans;
		}

		public void CheckAnswere (Variable[] inputParams, int lineNumber){
			
			if (inputParams.Length < parameterAmount)
				PMWrapper.RaiseError (lineNumber, "För få svar. Det ska vara " + parameterAmount + " st svar.");
			if (inputParams.Length > parameterAmount)
				PMWrapper.RaiseError (lineNumber, "För många svar. Det ska vara " + parameterAmount + " st svar.");

			foreach (Variable param in inputParams) {
				if (param.variableType != type){
					switch (type){
						case VariableTypes.boolean: 
							PMWrapper.RaiseError (lineNumber, "Fel typ av svar. Svaret ska bara vara True eller False.");
							break;
					case VariableTypes.number:
						PMWrapper.RaiseError (lineNumber, "Fel typ av svar. Svaret ska vara ett tal");
						break;
					case VariableTypes.textString:
						PMWrapper.RaiseError (lineNumber, "Fel typ av svar. Svaret ska vara en text.");
						break;
					default:
						PMWrapper.RaiseError (lineNumber, "Fel typ av svar.");
						break;
					}
				}
			}
				
			string guess;
			string ans = "";

			switch (type) {
			case VariableTypes.boolean:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getBool ().ToString ();

					if (guess != answere [i])
						PMWrapper.RaiseError (lineNumber, "Fel svar. Försök igen!");
					else
						ans += guess;

					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}
				PMWrapper.ShowGuideBubble (lineNumber, "Svar: " + ans);
				break;

			case VariableTypes.number:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getNumber ().ToString ();

					if (guess != answere [i])
						PMWrapper.RaiseError (lineNumber, "Fel svar. Försök igen!");
					else
						ans += guess;
					
					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}
				PMWrapper.ShowGuideBubble (lineNumber, "Svar: " + ans);
				break;

			case VariableTypes.textString:
				for (int i = 0; i < inputParams.Length; i++) {
					guess = inputParams [i].getString ().ToString ();

					if (guess != answere [i])
						PMWrapper.RaiseError (lineNumber, "Fel svar. Försök igen!");
					else
						ans += guess;

					if (i < inputParams.Length - 1)
						ans += ", ";
					else
						ans += ".";
				}
				PMWrapper.ShowGuideBubble (lineNumber, "Svar: " + ans);
				break;
			}
		}
	}
}