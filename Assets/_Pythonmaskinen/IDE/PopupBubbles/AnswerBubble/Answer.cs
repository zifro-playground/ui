using System.Collections;
using System.Collections.Generic;
using PM;
using UnityEngine;

public class Answer : Compiler.Function
{
	public Answer()
	{
		name = "svara";
		buttonText = "svara()";
		inputParameterAmount.Add(0);
		inputParameterAmount.Add(1);
		inputParameterAmount.Add(2);
		inputParameterAmount.Add(3);
		hasReturnVariable = false;
		pauseWalker = true;
	}

	#region implemented abstract members of Function

	public override Compiler.Variable runFunction(Compiler.Scope currentScope, Compiler.Variable[] inputParas, int lineNumber)
	{
		Main.Instance.LevelAnswer.CheckAnswer(inputParas, lineNumber);
		return new Compiler.Variable();
	}

	#endregion
}
