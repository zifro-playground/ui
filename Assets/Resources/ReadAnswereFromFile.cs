using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAnswereFromFile : MonoBehaviour {

	/*private void BuildAnsweresFromFile () {
		string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
		TextAsset asset = Resources.Load<TextAsset> ("answeres");

		if (asset == null)
			return;

		string[] textRows = asset.text.Split (linebreaks, StringSplitOptions.RemoveEmptyEntries);

		if (textRows.Length != PMWrapper.numOfLevels)
			throw new Exception ("The number of answeres in \"Assets/Resources/answeres.txt\" does not match the number of levels. Should be " + PMWrapper.numOfLevels);

		for (int i = 0; i < textRows.Length; i++) {
			int parameterAmount = 0;
			Compiler.VariableTypes type = Compiler.VariableTypes.None;
			string[] answere = new string[0];

			if (!textRows [i].StartsWith ("-")) {
				// TODO Parse Variable Type
				string[] splittedRow = textRows[i].Split(':');

				if (splittedRow.Length != 2)
					throw new Exception ("A row should contain a variable type and a answere separated by :");

				string textType = splittedRow [0].Trim ().ToLower();
				switch (textType) {
				case "number": 
					type = Compiler.VariableTypes.number;
					break;
				case "text":
					type = Compiler.VariableTypes.textString;
					break;
				case "bool":
					type = Compiler.VariableTypes.boolean;
					break;
				default: 
					throw new Exception (textType + " is not a supported variable type. Choose number, text or bool.");
				}

				answere = splittedRow [1].Trim().Replace(" ", "").Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
				parameterAmount = answere.Length;
			}

			LevelAnswere levelAnswere = new LevelAnswere (parameterAmount, type, answere);
			levels [i].answere = levelAnswere;
		}
	}*/
}
