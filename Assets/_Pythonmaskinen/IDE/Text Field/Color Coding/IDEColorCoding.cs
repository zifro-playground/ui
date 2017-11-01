using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PM {

	public class IDEColorCoding {

		private static string[] keyWords = { "in", "while", "for", "if", "else", "True", "False", "not" , "def", "and", "or", "return",
	"as", "assert", "break", "class", "continue", "del", "elif", "except", "exec", "finally", "from", "global", "import", "is",
	"lambda", "pass", "raise", "try", "with", "yield", "None"};
		private static char[] operatorCharacters = { '*', '/', '-', '+', '<', '>', '=', '%', '\\', '|', '^', '~', '&' };
		private static string[] operators = {
			// Math
			"*", "**", "/", "-", "+", "%",
			// Bitwise
			"<<", ">>", "&", "|", "~", "^",
			// Comparission 
			"<", "<=", ">", ">=", "==",
			// Asignments
			"=", "+=", "-=", "*=", "%=", "/=", "**=",
			"<<=", ">>=", "&=", "|=", "^=",
		};

		private static List<string> functionNames {
			get {
				var list = UISingleton.instance.compiler.allAddedFunctions.ConvertAll(f => f.name);
				list.Add("range");
				return list;
			}
		}

		private const string keyWordsColor = "#DE5170";
		private const string functionColor = "#DDDD11";
		private const string textHexaColor = "#7CBC4F";
		private const string commentColor = "#53C0ED";
		private const string numberColor = "#E8A64E";
		private const char commentSign = '#';
		private const char stringSign = '"';

		public static string runColorCode(string currentText) {
			string[] lines = currentText.Split('\n');
			string all = string.Empty;

			for (int i=0; i<lines.Length; i++) {
				var segments = SplitLineIntoSegments(lines[i]);
				for (int j = 0; j < segments.Count; j++) {
					// Find next and prev non-whitespace
					//int next = -1, prev = -1;
					//for (int k = 0; k < segments.Count; k++) {
					//	if (segments[k].type == SegmentType.Whitespace) continue;
					//	if (k < j) prev = k;
					//	if (k > j) {
					//		next = k;
					//		break;
					//	}
					//}

					//all += segments[j].GetColored(prev != -1 ? (Segment?) segments[prev] : null, next != -1 ? (Segment?) segments[next] : null);
					all += segments[j].GetColored();
				}
				if (i != lines.Length - 1) all += '\n';
			}

			return all;
		}

		#region Parsing
		private static List<Segment> SplitLineIntoSegments(string line) {
			List<Segment> segments = new List<Segment>();

			Segment current = new Segment();

			for (int i=0; i<line.Length; i++) {
				char c = line[i];
				SegmentType charType = SegmentType.Unknown;

				// The main parsing done here

				if (current.type == SegmentType.Comment) {
					// Continue comment
					charType = SegmentType.Comment;
				} else if (current.type == SegmentType.String) {
					if (c == stringSign) {
						// End of string
						current.text += c;
						segments.Add(current);
						current = new Segment();
						continue;
					} else {
						// Continue string
						charType = SegmentType.String;
					}
				} else if (c == commentSign) {
					// Start of comment
					charType = SegmentType.Comment;

				} else if (c == stringSign) {
					// Start of string
					charType = SegmentType.String;
				} else if (char.IsWhiteSpace(c)) {
					// Just whitespace
					charType = SegmentType.Whitespace;
				} else if (char.IsNumber(c) && current.type == SegmentType.Variable) {
					// Continue variable
					charType = SegmentType.Variable;
				} else if (char.IsNumber(c) || c == '.') {
					if (current.type == SegmentType.Number && c == '.' && current.text.Contains('.'))
						// Invalid number (multiple decimal points)
						charType = current.type = SegmentType.Symbol;
					else {
						// Unary operator (/and bitwise complement/ commented out)
						if (current.text == "-" || current.text == "~")
							current.type = SegmentType.Number;

						charType = SegmentType.Number;
					}
				} else if (char.IsLetter(c) || c == '_') {
					// Variable start (or continue)
					charType = SegmentType.Variable;
				} else if (operatorCharacters.Contains(c) && current.type != SegmentType.Symbol) {
					// Possibly an operator
					charType = SegmentType.Operator;
				} else {
					// Anything else, just mark as symbol
					charType = SegmentType.Symbol;
				}

				if (charType != current.type && current.type != SegmentType.Unknown) {
					segments.Add(current);
					current = new Segment { type = charType, text = c.ToString() };
				} else {
					current.type = charType;
					current.text += c;
				}
			}

			// Add last one
			if (current.type != SegmentType.Unknown)
				segments.Add(current);

			return segments;
		}

		private struct Segment {
			public string text;
			public SegmentType type;

			public int lastNonWhitespace;
			public int nextNonWhitespace;

			public string GetColored(/*Segment? previousNonWhitespace = null, Segment? nextNonWhitespace = null*/) {
				switch(type) {
					case SegmentType.Comment: return colorComment(text);
					case SegmentType.Number: return colorNumber(text);
					case SegmentType.String: return colorText(text);
					case SegmentType.Variable: return colorKeyWords(text);
					case SegmentType.Operator: return colorOperator(text);
					default: return text;
				}
			}
		}
		
		private enum SegmentType {
			Unknown,
			Whitespace,
			Number,
			String,
			Comment,
			Symbol,
			Variable,
			Operator,
		}
		#endregion


		#region Coloring
		private static string colorKeyWords(string text) {
			if (keyWords.Contains(text))
				return string.Format("<color={0}>{1}</color>", keyWordsColor, text);
			//else if (functionNames.Contains(text))
			//	return string.Format("<color={0}>{1}</color>", functionColor, text);
			else return text;
		}

		private static string colorOperator(string text) {
			if (operators.Contains(text))
				return string.Format("<color={0}>{1}</color>", keyWordsColor, text);
			else return text;
		}

		private static string colorComment(string text) {
			return string.Format("<color={0}>{1}</color>", commentColor, text);
		}

		private static string colorText(string text) {
			return string.Format("<color={0}>{1}</color>", textHexaColor, text);
		}

		private static string colorNumber(string text) {
			return string.Format("<color={0}>{1}</color>", numberColor, text);
		}
		#endregion
		
	}

}