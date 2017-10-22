using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswereFunction : Compiler.Function {

	public AnswereFunction () {
		this.name = "svara";
		this.inputParameterAmount.Add (0);
		this.inputParameterAmount.Add (1);
		this.inputParameterAmount.Add (2);
		this.inputParameterAmount.Add (3);
		this.hasReturnVariable = false;
		this.pauseWalker = false;
	}

	#region implemented abstract members of Function

	public override Compiler.Variable runFunction (Compiler.Scope currentScope, Compiler.Variable[] inputParas, int lineNumber)
	{
		PM.UISingleton.instance.levelHandler.currentLevel.answere.CheckAnswere (inputParas, lineNumber);
		return new Compiler.Variable ();
	}

	#endregion
}
