using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PM;
using UnityEngine;

public class IDETextManipulationTests
{
	[Test]
	public void CountCodeLinesIgnoresWhiteSpaceLines()
	{
		// Arrange
		const int expected = 4;

		string[] codeLines = {
			"foo",
			"\t bar",
			"\t\t  \t",
			"\t\t  \t:",
			":\t\t  \t"
		};

		// Act
		int result = IDETextManipulation.CountCodeLines(codeLines);

		// Assert
		Assert.AreEqual(expected, result);
	}

	[Test]
	public void CountCodeLinesIgnoresCommentLines()
	{
		// Arrange
		const int expected = 3;

		string[] codeLines = {
			"foo#",
			"\t bar",
			"#",
			"\t\t  # some comment",
			"\t\t  !# not some comment"
		};

		// Act
		int result = IDETextManipulation.CountCodeLines(codeLines);

		// Assert
		Assert.AreEqual(expected, result);
	}
}
