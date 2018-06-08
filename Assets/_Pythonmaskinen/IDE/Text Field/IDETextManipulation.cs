using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace PM
{
	public class IDETextManipulation
	{
		public static char MyValidate(char c, int charIndex, bool isPasting, List<IndentLevel> toAddLevels, string currentText, IDETextField theTextField)
		{
			if (isPasting)
				return c;

			if (c == '\n')
			{
				toAddLevels.Clear();
				toAddLevels.Add(new IndentLevel(charIndex, IDEPARSER.getIndentLevel(charIndex, currentText)));
				theTextField.setNewCaretPos(toAddLevels[0].getNewCaretPos());

				return '\n';
			}

			return c;
		}

        public static int countCodeLines(List<string> textLines){
            int count = 0; 
            var regex = new Regex(@"^#|^\s*$|^\s*#");
            foreach (string line in textLines){
                line.Trim();
                if (regex.Match(line).Success||line=="") { }
                else { count++; }   
            }
            return count;
        }

		public static bool validateText(string fullText, int maxLines, int maxPerLine)
		{
			List<string> textLines = IDEPARSER.parseIntoLines(fullText);
            var numCodeLines = countCodeLines(textLines);

            UISingleton.instance.rowsLimit.UpdateRowsLeft(numCodeLines, maxLines);

            if (numCodeLines > maxLines)
			{
				// Too many rows
				UISingleton.instance.rowsLimit.redness = 1;
				return false;
			}

			foreach (string t in textLines)
			{
				if (lineSize(t) > maxPerLine)
					return false;
			}

			return true;
		}

		private static int lineSize(string lineText)
		{
			int sizeCounter = 0;

			for (int c = 0; c < lineText.Length; c++)
			{
				sizeCounter++;
				if (lineText[c] == '\t')
					sizeCounter += 4 - (sizeCounter % 4);
			}
			return sizeCounter;
		}
	}

	

	public sealed class IndentLevel
	{
		public readonly int caretPos;
		public readonly string indentText;
		public readonly int indentLevel;

		public IndentLevel(int caretPos, int indentLevel)
		{
			this.indentLevel = indentLevel;
			this.caretPos = caretPos + 1;
			this.indentText = new string('\t', indentLevel);
		}

		public int getNewCaretPos()
		{
			return indentLevel + caretPos;
		}
	}

}