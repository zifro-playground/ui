using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PM
{
	public class IDEParser
	{
		public static List<string> ParseIntoLines(string fullText)
		{
			return new List<string>(fullText.Split('\n'));
		}

		public static int CalcSelectedLineLastIndex(int selectedLine, string fullText)
		{
			List<string> lines = ParseIntoLines(fullText);

			if (--selectedLine >= lines.Count)
			{
				return -1;
			}

			int carPos = 0;
			for (int i = -1; i < selectedLine; i++)
			{
				carPos += lines[i + 1].Length;
			}

			return carPos;
		}

		public static int CalcCurrentSelectedLine(int caretPos, string fullText)
		{
			return fullText.Substring(0, caretPos).Split('\n').Length - 1;
		}

		public static int GetIndentLevel(int caretIndex, string fullText)
		{
			List<string> rows = ParseIntoLines(fullText);
			int lineIndex = CalcCurrentSelectedLine(caretIndex, fullText);

			int rowCaretIndex = GetRowCaretIndex(caretIndex, rows);
			if (rowCaretIndex == 0)
			{
				return 0;
			}

			return StandingAfterIndentOperator(rowCaretIndex, rows[lineIndex]) + CalcRowIndentLevel(rows[lineIndex]);
		}

		#region internal Calculations

		private static int CalcRowIndentLevel(string rowText)
		{
			int indentLevel = 0;
			for (int i = 0; i < rowText.Length; i++)
			{
				if (char.IsWhiteSpace(rowText[i]))
				{
					if (rowText[i] == '\t')
					{
						indentLevel++;
					}
				}
				else
				{
					break;
				}
			}

			return indentLevel;
		}

		private static int StandingAfterIndentOperator(int caretIndex, string rowText)
		{
			for (int i = caretIndex - 1; i > 0 && i < rowText.Length; i--)
			{
				if (char.IsWhiteSpace(rowText[i]) == false)
				{
					if (rowText[i] == ':')
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
			}

			return 0;
		}

		private static int GetRowCaretIndex(int caretPos, List<string> textLines)
		{
			int caretIndex = 0;
			string textSum = "";
			for (int i = 0; i < textLines.Count; i++)
			{
				if (i != 0)
				{
					textSum += "\n";
				}

				textSum += textLines[i];

				if (caretPos <= textSum.Length)
				{
					caretIndex = caretPos - (textSum.Length - textLines[i].Length);
					break;
				}
			}

			return caretIndex;
		}

		#endregion
	}
}