using System;
using System.Collections.Generic;
using System.Globalization;
using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace PM.GlobalFunctions
{
	public class LengthOf : ClrFunction
	{
		public LengthOf() : base("len")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			if (v is IScriptString s)
			{
				return Processor.Factory.Create(s.Value.Length);
			}

			PMWrapper.RaiseError($"Kan inte beräkna längden på värde av typen '{v.GetTypeName()}'.");
			return Processor.Factory.Null;
		}
	}

	public class AbsoluteValue : ClrFunction
	{
		public AbsoluteValue() : base("abs")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			switch (v)
			{
			case IScriptInteger i:
				return Processor.Factory.Create(i.Value < 0 ? -i.Value : i.Value);
			case IScriptDouble d:
				return Processor.Factory.Create(d.Value < 0 ? -d.Value : d.Value);
			default:
				PMWrapper.RaiseError($"Kan inte beräkna absoluta värdet av typen '{v.GetTypeName()}'.");
				return Processor.Factory.Null;
			}
		}
	}

	public class RoundedValue : ClrFunction
	{
		public RoundedValue() : base("round")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			switch (v)
			{
			case IScriptInteger i:
				return Processor.Factory.Create(i.Value);
			case IScriptDouble d:
				return Processor.Factory.Create(Math.Round(d.Value));
			default:
				PMWrapper.RaiseError($"Kan inte avrunda värde av typen '{v.GetTypeName()}'.");
				return Processor.Factory.Null;
			}
		}
	}

	public class ConvertToBinary : ClrFunction
	{
		public ConvertToBinary() : base("bin")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			if (v is IScriptInteger i)
			{
				if (i.Value < 0)
				{
					return Processor.Factory.Create("-0b" + Convert.ToString(-i.Value, 2));
				}

				return Processor.Factory.Create("0b" + Convert.ToString(i.Value, 2));
			}

			PMWrapper.RaiseError($"Kan inte konvertera typen '{v.GetTypeName()}' till binärt!");
			return Processor.Factory.Null;
		}
	}

	public class ConvertToHexadecimal : ClrFunction
	{
		public ConvertToHexadecimal()
			: base("hex")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			if (v is IScriptInteger i)
			{
				if (i.Value < 0)
				{
					return Processor.Factory.Create("-0x" + Convert.ToString(-i.Value, 16));
				}

				return Processor.Factory.Create("0x" + Convert.ToString(i.Value, 16));
			}

			PMWrapper.RaiseError($"Kan inte konvertera typen '{v.GetTypeName()}' till hexadecimalt!");
			return Processor.Factory.Null;
		}
	}

	public class ConvertToOctal : ClrFunction
	{
		public ConvertToOctal()
			: base("oct")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			IScriptType v = arguments[0];

			if (v is IScriptInteger i)
			{
				if (i.Value < 0)
				{
					return Processor.Factory.Create("-0o" + Convert.ToString(-i.Value, 8));
				}

				return Processor.Factory.Create("0o" + Convert.ToString(i.Value, 8));
			}

			PMWrapper.RaiseError($"Kan inte konvertera typen '{v.GetTypeName()}' till octaler!");
			return Processor.Factory.Null;
		}
	}

	public class MinimumValue : ClrFunction
	{
		public MinimumValue()
			: base("min")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			if (arguments.Length == 0)
			{
				PMWrapper.RaiseError("Kräver minst 1 värde till min().");
			}

			IScriptType min = null;

			foreach (IScriptType v in arguments)
			{
				if (min == null)
				{
					min = v;
					continue;
				}

				IScriptType result = v.CompareLessThan(min);

				if (result == null)
				{
					PMWrapper.RaiseError($"Kan inte jämföra datatypen '{v.GetTypeName()}'.");
				}
				else if (result.IsTruthy())
				{
					min = v;
				}
			}

			return min;
		}
	}

	public class MaximumValue : ClrFunction
	{
		public MaximumValue()
			: base("max")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			if (arguments.Length == 0)
			{
				PMWrapper.RaiseError("Kräver minst 1 värde till max().");
			}

			IScriptType max = null;

			foreach (IScriptType v in arguments)
			{
				if (max == null)
				{
					max = v;
					continue;
				}

				IScriptType result = v.CompareGreaterThan(max);

				if (result == null)
				{
					PMWrapper.RaiseError($"Kan inte jämföra datatypen '{v.GetTypeName()}'.");
				}
				else if (result.IsTruthy())
				{
					max = v;
				}
			}

			return max;
		}
	}

	public class GetTime : ClrFunction
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public GetTime()
			: base("time")
		{
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			return Processor.Factory.Create((int)(DateTime.UtcNow - EPOCH).TotalSeconds);
		}
	}
}
