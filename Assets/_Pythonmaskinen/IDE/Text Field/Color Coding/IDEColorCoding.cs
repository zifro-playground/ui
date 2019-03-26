using System.Collections.Generic;
using System.Linq;

namespace PM
{
	public class IDEColorCoding
	{
		private static readonly string[] KEYWORDS = {
			"in", "while", "for", "if", "else", "True", "False", "not", "def", "and", "or", "return",
			"as", "assert", "break", "class", "continue", "del", "elif", "except", "exec", "finally", "from", "global",
			"import", "is",
			"lambda", "pass", "raise", "try", "with", "yield", "None"
		};

		private static readonly char[] OPERATOR_CHARACTERS =
			{'*', '/', '-', '+', '<', '>', '=', '%', '|', '^', '~', '&'};

		private static readonly string[] OPERATORS = {
			// Math
			"*", "**", "/", "-", "+", "%",
			// Bitwise
			"<<", ">>", "&", "|", "~", "^",
			// Comparision 
			"<", "<=", ">", ">=", "==",
			// Assignment
			"=", "+=", "-=", "*=", "%=", "/=", "**=",
			"<<=", ">>=", "&=", "|=", "^=",
		};

		private const string KEY_WORDS_COLOR = "#FF3F85";

		//private const string functionColor = "#DDDD11";
		private const string TEXT_HEX_COLOR = "#68CC47";
		private const string COMMENT_COLOR = "#6B9EA5";
		private const string NUMBER_COLOR = "#FF7C26";
		private const char COMMENT_SIGN = '#';
		private const char S_STRING_SIGN = '\'';
		private const char D_STRING_SIGN = '"';

		public static string RunColorCode(string currentText)
		{
			string[] lines = currentText.Split('\n');
			string all = string.Empty;

			for (int i = 0; i < lines.Length; i++)
			{
				List<Segment> segments = SplitLineIntoSegments(lines[i]);
				for (int j = 0; j < segments.Count; j++)
				{
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

				if (i != lines.Length - 1)
				{
					all += '\n';
				}
			}

			return all;
		}

		#region Parsing

		private static List<Segment> SplitLineIntoSegments(string line)
		{
			var segments = new List<Segment>();

			var current = new Segment();

			foreach (char c in line)
			{
				SegmentType charType;

				// The main parsing done here

				switch (current.type)
				{
				case SegmentType.Comment:
					// Continue comment
					charType = SegmentType.Comment;
					break;
				case SegmentType.StringSingleQuote when c == S_STRING_SIGN:
					// End of string
					current.text += c;
					segments.Add(current);
					current = new Segment();
					continue;
				case SegmentType.StringSingleQuote:
					// Continue string
					charType = SegmentType.StringSingleQuote;
					break;
				case SegmentType.StringDoubleQuote when c == D_STRING_SIGN:
					// End of string
					current.text += c;
					segments.Add(current);
					current = new Segment();
					continue;
				case SegmentType.StringDoubleQuote:
					// Continue string
					charType = SegmentType.StringDoubleQuote;
					break;
				default:
					switch (c)
					{
					case COMMENT_SIGN:
						// Start of comment
						charType = SegmentType.Comment;
						break;
					case S_STRING_SIGN:
						// Start of string
						charType = SegmentType.StringSingleQuote;
						break;
					case D_STRING_SIGN:
						// Start of string
						charType = SegmentType.StringDoubleQuote;
						break;
					default:
						if (char.IsWhiteSpace(c))
						{
							// Just whitespace
							charType = SegmentType.Whitespace;
						}
						else if (char.IsNumber(c) && current.type == SegmentType.Variable)
						{
							// Continue variable
							charType = SegmentType.Variable;
						}
						else if (char.IsNumber(c) || c == '.')
						{
							if (current.type == SegmentType.Number && c == '.' && current.text.Contains('.'))
							{
								// Invalid number (multiple decimal points)
								charType = current.type = SegmentType.Symbol;
							}
							else
							{
								// Unary operator (/and bitwise complement/ commented out)
								if (current.text == "-" || current.text == "~")
								{
									current.type = SegmentType.Number;
								}

								charType = SegmentType.Number;
							}
						}
						else if (char.IsLetter(c) || c == '_')
						{
							// Variable start (or continue)
							charType = SegmentType.Variable;
						}
						else if (OPERATOR_CHARACTERS.Contains(c) && current.type != SegmentType.Symbol)
						{
							// Possibly an operator
							charType = SegmentType.Operator;
						}
						else
						{
							// Anything else, just mark as symbol
							charType = SegmentType.Symbol;
						}

						break;
					}

					break;
				}

				if (charType != current.type && current.type != SegmentType.Unknown)
				{
					segments.Add(current);
					current = new Segment {type = charType, text = c.ToString()};
				}
				else
				{
					current.type = charType;
					current.text += c;
				}
			}

			// Add last one
			if (current.type != SegmentType.Unknown)
			{
				segments.Add(current);
			}

			return segments;
		}

		private struct Segment
		{
			public string text;
			public SegmentType type;

			public string GetColored()
			{
				switch (type)
				{
				case SegmentType.Comment:
					return ColorComment(text);
				case SegmentType.Number:
					return ColorNumber(text);
				case SegmentType.StringDoubleQuote:
				case SegmentType.StringSingleQuote:
					return ColorText(text);
				case SegmentType.Variable:
					return ColorKeyWords(text);
				case SegmentType.Operator:
					return ColorOperator(text);
				default:
					return text;
				}
			}
		}

		private enum SegmentType
		{
			Unknown,
			Whitespace,
			Number,
			StringSingleQuote,
			StringDoubleQuote,
			Comment,
			Symbol,
			Variable,
			Operator,
		}

		#endregion

		#region Coloring

		private static string ColorKeyWords(string text)
		{
			if (KEYWORDS.Contains(text))
			{
				return $"<color={KEY_WORDS_COLOR}>{text}</color>";
			}
			//else if (functionNames.Contains(text))
			//	return string.Format("<color={0}>{1}</color>", functionColor, text);

			return text;
		}

		private static string ColorOperator(string text)
		{
			if (OPERATORS.Contains(text))
			{
				return $"<color={KEY_WORDS_COLOR}>{text}</color>";
			}

			return text;
		}

		private static string ColorComment(string text)
		{
			return $"<color={COMMENT_COLOR}>{text}</color>";
		}

		private static string ColorText(string text)
		{
			return $"<color={TEXT_HEX_COLOR}>{text}</color>";
		}

		private static string ColorNumber(string text)
		{
			return $"<color={NUMBER_COLOR}>{text}</color>";
		}

		#endregion
	}
}