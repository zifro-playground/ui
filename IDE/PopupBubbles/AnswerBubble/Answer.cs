using System.Collections;
using System.Collections.Generic;
using Mellis;
using Mellis.Core.Interfaces;
using PM;
using UnityEngine;

public class Answer : ClrYieldingFunction
{
	public Answer()
		: base("svara")
	{
	}

	public override void InvokeEnter(params IScriptType[] arguments)
	{
		Main.instance.levelAnswer.CheckAnswer(arguments);
	}
}