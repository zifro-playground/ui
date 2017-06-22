using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compiler;
using System;

namespace PM.GlobalFunctions {
	public class PrintFunction : Compiler.Function {

		public PrintFunction() {
			this.name = "print";
			this.inputParameterAmount.Add(0);
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = false;
			this.pauseWalker = true;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			UISingleton.instance.printBubble.ShowMessage(lineNumber);

			if (inputParas.Length > 0)
				UISingleton.instance.printBubble.SetPrintPopupValue(inputParas[0]);
			else
				UISingleton.instance.printBubble.SetPrintPopupValue(new Variable("NULL"));

			return new Variable("NULL");
		}

	}

	public class ConvertToInt : Compiler.Function {

		public const string onFail = "Misslyckades göra värde till heltal!";
		public const string onFail2 = "Basen måste vara ett heltal!";
		public const string onFail3 = "Basen måste vara mellan 2 och 16!";


		public ConvertToInt(string name) {
			this.name = name;
			this.inputParameterAmount.Add(0);
			this.inputParameterAmount.Add(1);
			this.inputParameterAmount.Add(2);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		private const string all_characters = "0123456789ABCDEF";
		private static long ParseString(string inputString, int inputBase) {
			string str = inputString.Trim('$','£','€',' ').TrimEnd(':',';','-').ToUpper();
			string characters = all_characters.Substring(0,inputBase);
			long result = 0;
			bool neg = false;

			for (int i = 0; i < str.Length; i++) {
				char c = str[i];

				if (i == 0 && c == '-') neg = true;
				else {
					var indx = characters.IndexOf(c);
					if (indx == -1) throw new Exception();
					result = inputBase * result + indx;
				}
			}
			return neg ? -result : result;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			if (inputParas.Length == 0) return new Variable("Heltal", 0);

			Variable v = inputParas[0];
			Variable b = inputParas.Length == 2 ? inputParas[1] : null;

			// if got b but b isnt int
			if (b != null && (b.variableType != VariableTypes.number || Math.Abs(b.getNumber() - Math.Round(b.getNumber())) > double.Epsilon))
				PMWrapper.RaiseError(lineNumber, onFail2);

			int inputBase = 10;
			if (b != null) inputBase = (int) Math.Round(b.getNumber());
			if (inputBase < 2 || inputBase > 16) PMWrapper.RaiseError(lineNumber, onFail3);

			try {
				switch (v.variableType) {
					case VariableTypes.boolean:
						return new Variable(v.name, v.getBool() ? 1 : 0);

					case VariableTypes.textString:
						return new Variable(v.name, ParseString(v.getString(), inputBase));

					case VariableTypes.number:
						return new Variable(v.name, (int) v.getNumber());
				}
			} catch {
				PMWrapper.RaiseError(lineNumber, onFail);
			}

			return null;
		}
	}

	/// <summary>
	/// Actually converts to double. But in original python the function is called float()
	/// </summary>
	public class ConvertToFloat : Compiler.Function {

		public const string onFail = "Misslyckades göra värde till reellt tal!";

		public ConvertToFloat() {
			this.name = "float";
			this.inputParameterAmount.Add(0);
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			if (inputParas.Length == 0) return new Variable("Float", 0);

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.boolean:
					return new Variable(v.name, v.getBool() ? 1 : 0);

				case VariableTypes.textString:
					double num;
					if (double.TryParse(v.getString(), out num))
						return new Variable(v.name, num);
					else
						PMWrapper.RaiseError(lineNumber, onFail);
					break;

				case VariableTypes.number:
					return v;
			}

			PMWrapper.RaiseError(lineNumber, onFail);
			return null;
		}
	}

	public class ConvertToString : Compiler.Function {

		public ConvertToString() {
			this.name = "str";
			this.inputParameterAmount.Add(0);
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			if (inputParas.Length == 0) return new Variable("Sträng", string.Empty);

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.boolean:
					return new Variable(v.name, v.getBool() ? "True" : "False");

				case VariableTypes.textString:
					return v;

				case VariableTypes.number:
					return new Variable(v.name, v.getNumber().ToString("0.#####################################################################################################################################################################################################################################################################################################################################"));

				default:
					return new Variable(v.name, "None");
			}
		}
	}

	public class ConvertToBoolean : Compiler.Function {

		public ConvertToBoolean() {
			this.name = "bool";
			this.inputParameterAmount.Add(0);
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			if (inputParas.Length == 0) return new Variable("Bool", false);

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.boolean:
					return v;

				case VariableTypes.textString: // return v != ''
					return new Variable(v.name, v.getString().Length != 0);

				case VariableTypes.number: // return v != 0
					return new Variable(v.name, Math.Abs(v.getNumber()) > double.Epsilon);

				default:
					return new Variable(v.name, false);
			}
		}
	}

	public class LengthOf : Compiler.Function {

		public LengthOf() {
			this.name = "len";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.textString:
					return new Variable("LengthOf" + v.name, v.getString().Length);

				case VariableTypes.boolean:
					PMWrapper.RaiseError(lineNumber, "En boolean har ingen längd!");
					return null;

				case VariableTypes.number:
					PMWrapper.RaiseError(lineNumber, "Siffror har ingen längd!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "'None' har ingen längd!");
					return null;
			}
		}
	}

	public class AbsoluteValue : Compiler.Function {

		public AbsoluteValue() {
			this.name = "abs";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.number:
					return new Variable('|' + v.name + '|', Math.Abs(v.getNumber()));

				case VariableTypes.boolean:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna absoluta värdet av en boolean!");
					return null;

				case VariableTypes.textString:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna absoluta värdet av en text sträng!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna absoluta värdet av 'None'!");
					return null;
			}
		}
	}

	public class RoundedValue : Compiler.Function {

		public RoundedValue() {
			this.name = "round";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.number:
					return new Variable("Rounded" + v.name, Math.Round(v.getNumber()));

				case VariableTypes.boolean:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna avrundat värdet av en boolean!");
					return null;

				case VariableTypes.textString:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna avrundat värdet av en text sträng!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "Kan inte beräkna avrundat värdet av 'None'!");
					return null;
			}
		}
	}

	public class ConvertToBinary : Compiler.Function {

		public ConvertToBinary() {
			this.name = "bin";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.number:
					if (Math.Abs(v.getNumber() - Math.Round(v.getNumber())) <= double.Epsilon)
						// Close enough to whole number
						return new Variable(v.name, Convert.ToString((long) Math.Round(v.getNumber()), 2));
					PMWrapper.RaiseError(lineNumber, "Kan endast konvertera heltal till binärt!");
					return null;

				case VariableTypes.boolean:
					return new Variable(v.name, v.getBool() ? "1" : "0");

				case VariableTypes.textString:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera text sträng till binärt!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera 'None' till binärt!");
					return null;
			}
		}
	}

	public class ConvertToHexadecimal : Compiler.Function {

		public ConvertToHexadecimal() {
			this.name = "hex";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.number:
					if (Math.Abs(v.getNumber() - Math.Round(v.getNumber())) <= double.Epsilon)
						// Close enough to whole number
						return new Variable(v.name, Convert.ToString((long) Math.Round(v.getNumber()), 16));
					PMWrapper.RaiseError(lineNumber, "Kan endast konvertera heltal till hexadecimal!");
					return null;

				case VariableTypes.boolean:
					return new Variable(v.name, v.getBool() ? "1" : "0");

				case VariableTypes.textString:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera text sträng till hexadecimal!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera 'None' till hexadecimal!");
					return null;
			}
		}
	}

	public class ConvertToOctal : Compiler.Function {

		public ConvertToOctal() {
			this.name = "oct";
			this.inputParameterAmount.Add(1);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			Variable v = inputParas[0];

			switch (v.variableType) {
				case VariableTypes.number:
					if (Math.Abs(v.getNumber() - Math.Round(v.getNumber())) <= double.Epsilon)
						// Close enough to whole number
						return new Variable(v.name, Convert.ToString((long) Math.Round(v.getNumber()), 8));
					PMWrapper.RaiseError(lineNumber, "Kan endast konvertera heltal till oktaler!");
					return null;

				case VariableTypes.boolean:
					return new Variable(v.name, v.getBool() ? "1" : "0");

				case VariableTypes.textString:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera text sträng till octaler!");
					return null;

				default:
					PMWrapper.RaiseError(lineNumber, "Kan inte konvertera 'None' till octaler!");
					return null;
			}
		}
	}

	public class MinimumValue : Compiler.Function {

		public MinimumValue() {
			this.name = "min";
			this.inputParameterAmount.Add(2);
			this.inputParameterAmount.Add(3);
			this.inputParameterAmount.Add(4);
			this.inputParameterAmount.Add(5);
			this.inputParameterAmount.Add(6);
			this.inputParameterAmount.Add(7);
			this.inputParameterAmount.Add(8);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			object min = null;

			for (int i=0; i<inputParas.Length; i++) {
				var v = inputParas[i];
				
				// Checka så det är rätt datatyp
				if ((v.variableType != VariableTypes.boolean && v.variableType != VariableTypes.number && v.variableType != VariableTypes.textString)
					|| (v.variableType == VariableTypes.textString && v.getString() == null))
					// If (not any of the allowed types)
					// or (null string) then error
					PMWrapper.RaiseError(lineNumber, "Kan inte mäta 'None'!");

				if (min == null) {
					if (v.variableType == VariableTypes.number) min = v.getNumber();
					if (v.variableType == VariableTypes.boolean) min = v.getBool();
					if (v.variableType == VariableTypes.textString) min = v.getString();
					continue;
				}

				switch (v.variableType) {
					case VariableTypes.boolean:
						if (min is string) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med boolean!");
						if (min is double)
							// True=1 False=0
							if ((v.getBool() ? 1 : 0) < (double)min) min = v.getBool();
						if (min is bool) min = (bool)min == false || v.getBool() == false;
						break;

					case VariableTypes.number:
						if (min is string) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med siffra!");
						if (min is double) min = Math.Min((double)min, v.getNumber());
						if (min is bool)
							// True=1 False=0
							if (v.getNumber() < ((bool)min == true ? 1 : 0)) min = v.getNumber();
						break;

					case VariableTypes.textString:
						if (min is string) if (v.getString().CompareTo((string)min) < 0) min = v.getString();
						if (min is double) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med siffra!");
						if (min is bool) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med boolean!");
						break;
				}
			}

			if (min is string)
				return new Variable("Minimum", (string) min);
			if (min is double)
				return new Variable("Minimum", (double) min);
			if (min is bool)
				return new Variable("Minimum", (bool) min);

			return null;
		}
	}


	public class MaximumValue : Compiler.Function {

		public MaximumValue() {
			this.name = "max";
			this.inputParameterAmount.Add(2);
			this.inputParameterAmount.Add(3);
			this.inputParameterAmount.Add(4);
			this.inputParameterAmount.Add(5);
			this.inputParameterAmount.Add(6);
			this.inputParameterAmount.Add(7);
			this.inputParameterAmount.Add(8);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			object max = null;

			for (int i = 0; i < inputParas.Length; i++) {
				var v = inputParas[i];

				// Checka så det är rätt datatyp
				if ((v.variableType != VariableTypes.boolean && v.variableType != VariableTypes.number && v.variableType != VariableTypes.textString)
					|| (v.variableType == VariableTypes.textString && v.getString() == null))
					// If (not any of the allowed types)
					// or (null string) then error
					PMWrapper.RaiseError(lineNumber, "Kan inte mäta 'None'!");

				if (max == null) {
					if (v.variableType == VariableTypes.number) max = v.getNumber();
					if (v.variableType == VariableTypes.boolean) max = v.getBool();
					if (v.variableType == VariableTypes.textString) max = v.getString();
					continue;
				}

				switch (v.variableType) {
					case VariableTypes.boolean:
						if (max is string) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med boolean!");
						if (max is double)
							// True=1 False=0
							if ((v.getBool() ? 1 : 0) > (double) max) max = v.getBool();
						if (max is bool) max = (bool) max || v.getBool();
						break;

					case VariableTypes.number:
						if (max is string) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med siffra!");
						if (max is double) max = Math.Max((double) max, v.getNumber());
						if (max is bool)
							// True=1 False=0
							if (v.getNumber() > ((bool) max == true ? 1 : 0)) max = v.getNumber();
						break;

					case VariableTypes.textString:
						if (max is string) if (v.getString().CompareTo((string) max) > 0) max = v.getString();
						if (max is double) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med siffra!");
						if (max is bool) PMWrapper.RaiseError(lineNumber, "Kan inte jämföra text sträng med boolean!");
						break;
				}
			}

			if (max is string)
				return new Variable("Minimum", (string) max);
			if (max is double)
				return new Variable("Minimum", (double) max);
			if (max is bool)
				return new Variable("Minimum", (bool) max);

			return null;
		}
	}

	public class GetTime : Compiler.Function {

		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public GetTime() {
			this.name = "time";
			this.inputParameterAmount.Add(0);
			this.hasReturnVariable = true;
			this.pauseWalker = false;
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {
			return new Variable("Unix", (int) (DateTime.UtcNow - epoch).TotalSeconds);
		}
	}

}