using System.Collections;
using System.Collections.Generic;
using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

public class CustomFunction : ClrFunction
{
	public CustomFunction()
		: base("AnpassadFunktion")
	{
	}

	public override IScriptType Invoke(params IScriptType[] arguments)
	{
		Debug.Log("Hej! Nu kör jag den anpassade funktionen.");

		return Processor.Factory.Null;
	}
}
