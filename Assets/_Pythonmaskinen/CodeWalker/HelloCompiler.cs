using System;
using System.Collections.Generic;
using System.Linq;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;
using PM.GlobalFunctions;
using UnityEngine;

namespace PM
{
	public class HelloCompiler : MonoBehaviour
	{
		public enum StopStatus
		{
			/// <summary>
			///     The compiler was stopped by user via pressing the stop button.
			/// </summary>
			UserForced,

			/// <summary>
			///     The compiler was stopped by code via e.g. PMWrapper.
			/// </summary>
			CodeForced,

			/// <summary>
			///     The compiler finished successfully.
			/// </summary>
			Finished,

			/// <summary>
			///     The compiler had an error during runtime. For example some missing variable or syntax error.
			/// </summary>
			RuntimeError,

			/// <summary>
			///     The compiler was stopped due to task error. For example user submitted wrong answer or uncompleted task.
			/// </summary>
			TaskError
		}

		[NonSerialized]
		public readonly List<IEmbeddedType> addedFunctions = new List<IEmbeddedType>();

		readonly IReadOnlyCollection<IEmbeddedType> globalFunctions = new IEmbeddedType[] {
			new AbsoluteValue(),
			new ConvertToBinary(),
			new ConvertToHexadecimal(),
			new LengthOf(),
			new RoundedValue(),
			new MinimumValue(),
			new MaximumValue(),
			new GetTime()
		};

		public CodeWalker theCodeWalker;
		public bool isRunning { get; private set; }

		IEnumerable<IEmbeddedType> allAddedFunctions => globalFunctions.Concat(addedFunctions);

		public void CompileCode()
		{
			if (isRunning)
			{
				return;
			}

			isRunning = true;

			foreach (IPMCompilerStarted ev in UISingleton.FindInterfaces<IPMCompilerStarted>())
			{
				ev.OnPMCompilerStarted();
			}

			try
			{
				IProcessor compiled = theCodeWalker.ActivateWalker(StopCompiler);
				compiled.AddBuiltin(allAddedFunctions.ToArray());
			}
			catch (SyntaxException e) when (!e.SourceReference.IsFromClr)
			{
				StopCompiler(StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.SourceReference.FromRow, e.Message);
			}
			catch (Exception e)
			{
				StopCompiler(StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.Message);
			}
		}

		#region stop methods

		public void StopCompilerButton()
		{
			StopCompiler(StopStatus.UserForced);
		}

		public void StopCompiler(StopStatus status = StopStatus.CodeForced)
		{
			isRunning = false;

			theCodeWalker.StopWalker();

			// Call stop events
			foreach (IPMCompilerStopped ev in UISingleton.FindInterfaces<IPMCompilerStopped>())
			{
				ev.OnPMCompilerStopped(status);
			}
		}

		#endregion
	}
}