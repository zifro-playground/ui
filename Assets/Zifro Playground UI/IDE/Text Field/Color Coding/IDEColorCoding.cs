using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PM
{
    public class IDEColorCoding
    {
        private static string[] keyWords =
        {
            "in", "while", "for", "if", "else", "True", "False", "not", "def", "and", "or", "return",
            "as", "assert", "break", "class", "continue", "del", "elif", "except", "exec", "finally", "from", "global",
            "import", "is",
            "lambda", "pass", "raise", "try", "with", "yield", "None"
        };

        private static readonly char[] operatorCharacters =
            {'*', '/', '-', '+', '<', '>', '=', '%', '|', '^', '~', '&', '!'};

        private static readonly string[] operators =
        {
            // Math
            "*", "**", "/", "-", "+", "%", "//", "**",
            // Bitwise
            "<<", ">>", "&", "|", "~", "^",
            // Comparission 
            "<", "<=", ">", ">=", "==", "!=",
            // Asignments
            "=", "+=", "-=", "*=", "%=", "/=", "//=", "**=",
            "<<=", ">>=", "&=", "|=", "^=",
        };

        private const string keyWordsColor = "#FF3F85";
        private const string textHexColor = "#68CC47";
        private const string commentColor = "#6B9EA5";
        private const string numberColor = "#FF7C26";
        private const char commentSign = '#';
        private const char sStringSign = '\'';
        private const char dStringSign = '"';
        private const char escapeCharacter = '\\';

        public static string RunColorCode(string currentText)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (Segment segment in new Tokenizer(currentText))
                {
                    builder.Append(segment.GetColored());
                }

                return builder.ToString();
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogException(ex);
                return currentText;
            }
        }

        #region Parsing

        private class Tokenizer : IEnumerable<Segment>, IEnumerator<Segment>
        {
            private readonly string text;
            private int index;
            private int lastIndex = -1;
            private int moveCount;

            private const int MOVE_LIMIT = 10_000;

            public Tokenizer(string text)
            {
                this.text = text;
            }

            public bool MoveNext()
            {
                if (index >= text.Length)
                {
                    return false;
                }

                if (lastIndex == index)
                {
                    throw new InvalidOperationException("Index did not iterate.");
                }

                if (moveCount > MOVE_LIMIT)
                {
                    throw new InvalidOperationException("Too many iterations.");
                }

                lastIndex = index;
                moveCount++;

                char c = text[index];

                switch (c)
                {
                case commentSign:
                {
                    // Read until newline
                    int endOfLine = text.IndexOf('\n', index + 1);
                    if (endOfLine == -1)
                    {
                        Current = new Segment(text.Substring(index), SegmentType.Comment);
                        index = text.Length;
                        return true;
                    }

                    Current = new Segment(text.Substring(index, endOfLine - index), SegmentType.Comment);
                    index = endOfLine;
                    return true;
                }
                case sStringSign:
                case dStringSign:
                {
                    // Check if long string
                    Current = new Segment(ReadString(c), SegmentType.String);
                    return true;
                }
                case '_':
                {
                    Current = new Segment(ReadIdentifierUntilStringPrefix(), SegmentType.Identifier);
                    return true;
                }
                default:
                    if (char.IsWhiteSpace(c))
                    {
                        Current = new Segment(ReadWhitespace(), SegmentType.Whitespace);
                        return true;
                    }

                    if (char.IsLetter(c))
                    {
                        int prefixLength = LengthOfStringPrefixAhead();
                        if (prefixLength > 0)
                        {
                            string prefix = text.Substring(index, prefixLength);
                            index += prefixLength;
                            string str = ReadString(text[index]);
                            Current = new Segment(prefix + str, SegmentType.String);
                            return true;
                        }

                        Current = new Segment(ReadIdentifierUntilStringPrefix(), SegmentType.Identifier);
                        return true;
                    }

                    if (operatorCharacters.Contains(c))
                    {
                        Current = new Segment(ReadOperator(), SegmentType.Operator);
                        return true;
                    }

                    string number = ReadNumber();
                    if (number != null)
                    {
                        Current = new Segment(number, SegmentType.Number);
                        return true;
                    }

                    Current = new Segment(text.Substring(index, 1), SegmentType.Symbol);
                    index++;
                    return true;
                }
            }

            private string ReadEscapedCharacter()
            {
                if (text[index] == escapeCharacter &&
                    index + 1 < text.Length)
                {
                    string result = $"{text[index]}{text[index + 1]}";
                    index += 2;
                    return result;
                }

                return text[index++].ToString();
            }

            private string ReadWhitespace()
            {
                int startIndex = index;

                // Assume first was whitespace
                while (++index < text.Length)
                {
                    if (!char.IsWhiteSpace(text[index]))
                    {
                        return text.Substring(startIndex, index - startIndex);
                    }
                }

                index = text.Length;
                return text.Substring(startIndex);
            }

            private string ReadString(char quote)
            {
                bool isLong;
                var builder = new StringBuilder();

                if (text.Length >= index + 3 &&
                    text[index + 1] == quote &&
                    text[index + 2] == quote)
                {
                    isLong = true;
                    builder.Append(text, index, 3);
                    index += 3;
                }
                else
                {
                    isLong = false;
                    builder.Append(text[index]);
                    index++;
                }

                int counter = 0;
                int max = isLong ? 3 : 1;
                while (index < text.Length)
                {
                    string c = ReadEscapedCharacter();
                    builder.Append(c);

                    if (!isLong && c == "\n")
                    {
                        break;
                    }

                    if (c[0] == quote)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                    }

                    if (counter == max)
                    {
                        break;
                    }
                }

                return builder.ToString();
            }

            private int LengthOfStringPrefixAhead()
            {
                // string prefix    ::=  "r" | "u" | "f" | "fr" | "rf"
                // bytes prefix     ::=  "b" | "br" | "rb"

                if (index + 2 < text.Length &&
                    (text[index + 2] == dStringSign || text[index + 2] == sStringSign))
                {
                    // test for: fr'', rf'', br'', rb''
                    string prefix = text.Substring(index, 2).ToLowerInvariant();
                    if (prefix == "fr" || prefix == "rf" ||
                        prefix == "br" || prefix == "rb")
                    {
                        return 2;
                    }
                }

                if (index + 1 < text.Length &&
                    (text[index + 1] == dStringSign || text[index + 1] == sStringSign))
                {
                    // test for: r'', u'', f''
                    char prefix = char.ToLowerInvariant(text[index]);
                    if (prefix == 'r' || prefix == 'u' ||
                        prefix == 'f' || prefix == 'b')
                    {
                        return 1;
                    }
                }

                return 0;
            }

            private string ReadIdentifierUntilStringPrefix()
            {
                int startIndex = index;
                index++;

                while (index < text.Length)
                {
                    char c = text[index];
                    if ((char.IsLetter(c) || char.IsNumber(c) || c == '_')
                        && LengthOfStringPrefixAhead() == 0)
                    {
                        index++;
                    }
                    else
                    {
                        return text.Substring(startIndex, index - startIndex);
                    }
                }

                index = text.Length;
                return text.Substring(startIndex);
            }

            private string ReadOperator()
            {
                if (index + 2 < text.Length)
                {
                    string tripleChars = text.Substring(index, 3);
                    if (operators.Contains(tripleChars))
                    {
                        index += 3;
                        return tripleChars;
                    }
                }

                if (index + 1 < text.Length)
                {
                    string doubleChars = text.Substring(index, 2);
                    if (operators.Contains(doubleChars))
                    {
                        index += 2;
                        return doubleChars;
                    }
                }

                string singleChar = text[index].ToString();
                index++;
                return singleChar;
            }

            private string ReadNumber()
            {
                return (IMAG_NUMBER()
                        ?? FLOAT_NUMBER())
                       ?? INTEGER();

                string FLOAT_NUMBER()
                {
                    // float-number   ::=  point-float | exponent-float
                    return EXPONENT_FLOAT()
                           ?? POINT_FLOAT();

                    string POINT_FLOAT()
                    {
                        // point-float    ::=  [int-part] fraction | int-part "."
                        int startIndex = index;
                        string intPart = INT_PART();
                        string fraction = FRACTION();

                        if (fraction == null)
                        {
                            index = startIndex;
                            return null;
                        }

                        if (intPart == null)
                        {
                            return fraction;
                        }

                        return intPart + fraction;
                    }

                    string EXPONENT_FLOAT()
                    {
                        // exponent-float ::=  (int-part | point-float) exponent
                        return EXPONENT_POINT_FLOAT()
                               ?? EXPONENT_INT_PART();

                        string EXPONENT_POINT_FLOAT()
                        {
                            // point-float exponent
                            int startIndex = index;
                            string intPart = POINT_FLOAT();
                            if (intPart == null)
                            {
                                return null;
                            }

                            string exponent = EXPONENT();
                            if (exponent == null)
                            {
                                index = startIndex;
                                return null;
                            }

                            return intPart + exponent;
                        }

                        string EXPONENT_INT_PART()
                        {
                            // int-part exponent
                            int startIndex = index;
                            string intPart = INT_PART();
                            if (intPart == null)
                            {
                                return null;
                            }

                            string exponent = EXPONENT();
                            if (exponent == null)
                            {
                                index = startIndex;
                                return null;
                            }

                            return intPart + exponent;
                        }

                        string EXPONENT()
                        {
                            // exponent      ::=  ("e" | "E") ["+" | "-"] digit+
                            if (index + 1 >= text.Length ||
                                (text[index] != 'e' && text[index] != 'E'))
                            {
                                return null;
                            }

                            int startIndex = index;
                            int digitIndex = index + 1;
                            if (text[digitIndex] == '-' || text[digitIndex] == '+')
                            {
                                if (index + 2 >= text.Length)
                                {
                                    return null;
                                }

                                digitIndex++;
                            }

                            index = digitIndex;
                            string digits = INT_PART();
                            if (digits == null)
                            {
                                index = startIndex;
                                return null;
                            }

                            return text.Substring(startIndex, digitIndex - startIndex) + digits;
                        }
                    }

                    string FRACTION()
                    {
                        // fraction      ::=  "." digit+
                        if (index + 1 >= text.Length ||
                            text[index] != '.')
                        {
                            return null;
                        }

                        index++;
                        string intPart = INT_PART();
                        if (intPart == null)
                        {
                            index--;
                            return null;
                        }

                        return $".{intPart}";
                    }
                }

                string IMAG_NUMBER()
                {
                    // imag-number ::=  (float-number | int-part) ("j" | "J")
                    int startIndex = index;
                    string number = FLOAT_NUMBER() ?? INT_PART();
                    if (number == null)
                    {
                        return null;
                    }

                    if (index >= text.Length ||
                        text[index] != 'j' &&
                        text[index] != 'J')
                    {
                        index = startIndex;
                        return null;
                    }

                    return number + text[index++];
                }

                string INTEGER()
                {
                    return ((OCT_INTEGER()
                             ?? HEX_INTEGER())
                            ?? BIN_INTEGER())
                           ?? DECIMAL_INTEGER();

                    string DECIMAL_INTEGER()
                    {
                        // integer ::=  nonzero-digit digit* | "0"+
                        int startIndex = index;

                        // "0"+, read all 0's
                        if (text[index] == '0')
                        {
                            while (++index < text.Length)
                            {
                                if (text[index] != '0')
                                {
                                    return text.Substring(startIndex, index - startIndex);
                                }
                            }

                            index = text.Length;
                            return text.Substring(startIndex);
                        }

                        // nonzero-digit
                        if (text[index] >= '1' && text[index] <= '9')
                        {
                            // read all digits
                            while (++index < text.Length)
                            {
                                if (text[index] < '0' || text[index] > '9')
                                {
                                    return text.Substring(startIndex, index - startIndex);
                                }
                            }

                            index = text.Length;
                            return text.Substring(startIndex);
                        }

                        return null;
                    }

                    string OCT_INTEGER()
                    {
                        // oct integer     ::=  "0" ("o" | "O") oct-digit+
                        if (index + 2 >= text.Length ||
                            text[index] != '0' ||
                            char.ToLowerInvariant(text[index + 1]) != 'o' ||
                            text[index + 2] < '0' || text[index + 2] > '7')
                        {
                            return null;
                        }

                        int startIndex = index;
                        index += 2;

                        while (index < text.Length)
                        {
                            if (text[index] < '0' || text[index] > '7')
                            {
                                return text.Substring(startIndex, index - startIndex);
                            }

                            index++;
                        }

                        index = text.Length;
                        return text.Substring(startIndex);
                    }

                    string HEX_INTEGER()
                    {
                        // hex integer     ::=  "0" ("x" | "X") hex-digit+
                        if (index + 2 >= text.Length ||
                            text[index] != '0' ||
                            char.ToLowerInvariant(text[index + 1]) != 'x' ||
                            !IsHexadecimal(text[index + 2]))
                        {
                            return null;
                        }

                        int startIndex = index;
                        index += 2;

                        while (index < text.Length)
                        {
                            if (!IsHexadecimal(text[index]))
                            {
                                return text.Substring(startIndex, index - startIndex);
                            }

                            index++;
                        }

                        index = text.Length;
                        return text.Substring(startIndex);
                    }

                    bool IsHexadecimal(char c)
                    {
                        return c >= '0' && c <= '9' ||
                               c >= 'a' && c <= 'f' ||
                               c >= 'A' && c <= 'A';
                    }

                    string BIN_INTEGER()
                    {
                        // bin integer     ::=  "0" ("b" | "B") bin-digit+
                        if (index + 2 >= text.Length ||
                            text[index] != '0' ||
                            char.ToLowerInvariant(text[index + 1]) != 'b' ||
                            (text[index + 2] != '0' && text[index + 2] != '1'))
                        {
                            return null;
                        }

                        int startIndex = index;
                        index += 2;

                        while (index < text.Length)
                        {
                            if (text[index] != '0' && text[index] != '1')
                            {
                                return text.Substring(startIndex, index - startIndex);
                            }

                            index++;
                        }

                        index = text.Length;
                        return text.Substring(startIndex);
                    }
                }

                string INT_PART()
                {
                    // int-part       ::=  digit+
                    if (index >= text.Length ||
                        text[index] < '0' || text[index] > '9')
                    {
                        return null;
                    }

                    int startIndex = index;

                    while (++index < text.Length)
                    {
                        if (text[index] < '0' || text[index] > '9')
                        {
                            return text.Substring(startIndex, index - startIndex);
                        }
                    }

                    index = text.Length;
                    return text.Substring(startIndex);
                }
            }

            public void Reset()
            {
                index = 0;
            }

            public Segment Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public IEnumerator<Segment> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private struct Segment
        {
            private readonly string text;
            private readonly SegmentType type;

            public Segment(string text, SegmentType type)
            {
                this.text = text;
                this.type = type;
            }

            public string GetColored()
            {
                switch (type)
                {
                case SegmentType.Comment: return ColorComment(text);
                case SegmentType.Number: return ColorNumber(text);
                case SegmentType.String: return ColorText(text);
                case SegmentType.Identifier: return ColorKeyWords(text);
                case SegmentType.Operator: return ColorOperator(text);
                default: return text;
                }
            }
        }

        private enum SegmentType
        {
            Unknown,

            /// <summary>spaces/tabs/newlines between other segments</summary>
            Whitespace,

            /// <summary>10, 1e50, 0b010110, 0o777, 0xc0ffee</summary>
            Number,

            /// <summary>'short string', rb"short string", '''long string''', fr"""long string"""</summary>
            String,

            /// <summary># Comment</summary>
            Comment,

            /// <summary>(, ), [, ], :, ., _, \</summary>
            Symbol,

            /// <summary>var_name, fooBar, f00, b4r</summary>
            Identifier,

            /// <summary>+, -, *, /, //, %, &amp;, ~, &gt;, &lt;, =, +=, -=, etc.</summary>
            Operator
        }

        #endregion

        #region Coloring

        private static string ColorKeyWords(string text)
        {
            if (keyWords.Contains(text))
                return string.Format("<color={0}>{1}</color>", keyWordsColor, text);
            return text;
        }

        private static string ColorOperator(string text)
        {
            return string.Format("<color={0}>{1}</color>", keyWordsColor, text);
        }

        private static string ColorComment(string text)
        {
            return string.Format("<color={0}>{1}</color>", commentColor, text);
        }

        private static string ColorText(string text)
        {
            return string.Format("<color={0}>{1}</color>", textHexColor, text);
        }

        private static string ColorNumber(string text)
        {
            return string.Format("<color={0}>{1}</color>", numberColor, text);
        }

        #endregion
    }
}
