using System;
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

			if (v.TryConvert(out string s))
			{
				return Processor.Factory.Create(s.Length);
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

			if (v.TryConvert(out int i))
			{
				return Processor.Factory.Create(i < 0 ? -i : i);
			}

			if (v.TryConvert(out double d))
			{
				return Processor.Factory.Create(d < 0 ? -d : d);
			}

			if (v.TryConvert(out bool b))
			{
				return Processor.Factory.Create(b ? 1 : 0);
			}

			PMWrapper.RaiseError($"Kan inte beräkna absoluta värdet av typen '{v.GetTypeName()}'.");
			return Processor.Factory.Null;
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

			if (v.TryConvert(out int i))
			{
				return Processor.Factory.Create(i);
			}

			if (v.TryConvert(out double d))
			{
				return Processor.Factory.Create(Math.Round(d));
			}

			if (v.TryConvert(out bool b))
			{
				return Processor.Factory.Create(b ? 1 : 0);
			}

			PMWrapper.RaiseError($"Kan inte avrunda värde av typen '{v.GetTypeName()}'.");
			return Processor.Factory.Null;
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

			if (v.TryConvert(out int i))
			{
				if (i < 0)
				{
					return Processor.Factory.Create("-0b" + Convert.ToString(-i, 2));
				}

				return Processor.Factory.Create("0b" + Convert.ToString(i, 2));
			}

			if (v.TryConvert(out bool b))
			{
				return b
					? Processor.Factory.Create("0b1")
					: Processor.Factory.Create("0b0");
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

			if (v.TryConvert(out int i))
			{
				if (i < 0)
				{
					return Processor.Factory.Create("-0x" + Convert.ToString(-i, 16));
				}

				return Processor.Factory.Create("0x" + Convert.ToString(i, 16));
			}

			if (v.TryConvert(out bool b))
			{
				return b
					? Processor.Factory.Create("0x1")
					: Processor.Factory.Create("0x0");
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

			if (v.TryConvert(out int i))
			{
				if (i < 0)
				{
					return Processor.Factory.Create("-0o" + Convert.ToString(-i, 8));
				}

				return Processor.Factory.Create("0o" + Convert.ToString(i, 8));
			}

			if (v.TryConvert(out bool b))
			{
				return b
					? Processor.Factory.Create("0o1")
					: Processor.Factory.Create("0o0");
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
			object max = null;

			foreach (IScriptType v in arguments)
			{
				if (v.TryConvert(out int i))
				{
					switch (max)
					{
					case null:
					case double md when i < md:
					case int mi when i < mi:
					case bool mb when i < (mb ? 1 : 0):
						max = i;
						break;
					}
				}
				else if (v.TryConvert(out double d))
				{
					switch (max)
					{
					case null:
					case double md when d < md:
					case int mi when d < mi:
					case bool mb when d < (mb ? 1 : 0):
						max = d;
						break;
					}
				}
				else if (v.TryConvert(out string s))
				{
					switch (max)
					{
					case null:
					case string m when string.Compare(s, m, StringComparison.Ordinal) < 0:
						max = s;
						break;
					}
				}
				else if (v.TryConvert(out bool b))
				{
					switch (max)
					{
					case null:
					case bool mb when mb && b:
					case double md when (b ? 1 : 0) < md:
					case int mi when (b ? 1 : 0) < mi:
						max = b;
						break;
					}
				}
				else
				{
					PMWrapper.RaiseError($"Kan inte jämföra datatypen '{v.GetTypeName()}'.");
				}
			}

			switch (max)
			{
			case string s:
				return Processor.Factory.Create(s);
			case double d:
				return Processor.Factory.Create(d);
			case int i:
				return Processor.Factory.Create(i);
			case bool b:
				return Processor.Factory.Create(b);
			default:
				PMWrapper.RaiseError("Kräver minst 1 värde till min().");
				return Processor.Factory.Null;
			}
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
			object max = null;

			foreach (IScriptType v in arguments)
			{
				if (v.TryConvert(out int i))
				{
					switch (max)
					{
					case null:
					case double md when i > md:
					case int mi when i > mi:
					case bool mb when i > (mb ? 1 : 0):
						max = i;
						break;
					}
				}
				else if (v.TryConvert(out double d))
				{
					switch (max)
					{
					case null:
					case double md when d > md:
					case int mi when d > mi:
					case bool mb when d > (mb ? 1 : 0):
						max = d;
						break;
					}
				}
				else if (v.TryConvert(out string s))
				{
					switch (max)
					{
					case null:
					case string m when string.Compare(s, m, StringComparison.Ordinal) < 0:
						max = s;
						break;
					}
				}
				else if (v.TryConvert(out bool b))
				{
					switch (max)
					{
					case null:
					case bool mb when mb || b:
					case double md when (b ? 1 : 0) > md:
					case int mi when (b ? 1 : 0) > mi:
						max = b;
						break;
					}
				}
				else
				{
					PMWrapper.RaiseError($"Kan inte jämföra datatypen '{v.GetTypeName()}'.");
				}
			}

			switch (max)
			{
			case string s:
				return Processor.Factory.Create(s);
			case double d:
				return Processor.Factory.Create(d);
			case int i:
				return Processor.Factory.Create(i);
			case bool b:
				return Processor.Factory.Create(b);
			default:
				PMWrapper.RaiseError("Kräver minst 1 värde till max().");
				return Processor.Factory.Null;
			}
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