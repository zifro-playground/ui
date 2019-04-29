using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace Sample
{
	public class CustomFunction : ClrFunction
	{
		public CustomFunction() : base("AnpassadFunktion")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			Debug.Log("Hej! Nu kör jag den anpassade funktionen.");

			return Processor.Factory.Create(Random.value);
		}
	}
}
