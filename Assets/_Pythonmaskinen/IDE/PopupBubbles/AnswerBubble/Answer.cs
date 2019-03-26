using System.Collections;
using System.Collections.Generic;
using Mellis;
using Mellis.Core.Interfaces;
using PM;
using UnityEngine;


public class Answer : ClrFunction
{
	public Answer()
		: base("svara")
	{
	}

	public override IScriptType Invoke(params IScriptType[] arguments)
	{
		Main.instance.levelAnswer.CheckAnswer(arguments);

		return null;
	}
}
