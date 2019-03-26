using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;

namespace PM
{
	public class HelloCompiler : MonoBehaviour
	{
		public bool isRunning { get; private set; }

		public CodeWalker theCodeWalker;

		[NonSerialized]
		public readonly List<IEmbeddedType> addedFunctions = new List<IEmbeddedType>();

		readonly IReadOnlyCollection<IEmbeddedType> globalFunctions = new IEmbeddedType[] {
			new GlobalFunctions.AbsoluteValue(),
			new GlobalFunctions.ConvertToBinary(),
			new GlobalFunctions.ConvertToHexadecimal(),
			new GlobalFunctions.LengthOf(),
			new GlobalFunctions.RoundedValue(),
			new GlobalFunctions.MinimumValue(),
			new GlobalFunctions.MaximumValue(),
			new GlobalFunctions.GetTime(),
		};

		IEnumerable<IEmbeddedType> allAddedFunctions => globalFunctions.Concat(addedFunctions);

		public void compileCode()
		{
			if (isRunning) return;

			isRunning = true;

			foreach (var ev in UISingleton.FindInterfaces<IPMCompilerStarted>())
				ev.OnPMCompilerStarted();

			try
			{
				IProcessor compiled = theCodeWalker.ActivateWalker(stopCompiler);
				compiled.AddBuiltin(allAddedFunctions.ToArray());
			}
			catch (SyntaxException e) when (!e.SourceReference.IsFromClr)
			{
				stopCompiler(StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.SourceReference.FromRow, e.Message);
			}
			catch (Exception e)
			{
				stopCompiler(StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.Message);
			}
		}

		public void prettyPrint(string dasMessage)
		{
			if (UISingleton.instance.textField.devBuild)
				print(dasMessage);
		}

		#region stop methods
		public void stopCompilerButton()
		{
			stopCompiler(StopStatus.UserForced);
		}

		public void stopCompiler(StopStatus status = StopStatus.CodeForced)
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

		public enum StopStatus
		{
			/// <summary>
			/// The compiler was stopped by user via pressing the stop button.
			/// </summary>
			UserForced,
			/// <summary>
			/// The compiler was stopped by code via e.g. PMWrapper.
			/// </summary>
			CodeForced,
			/// <summary>
			/// The compiler finished successfully.
			/// </summary>
			Finished,
			/// <summary>
			/// The compiler had an error during runtime. For example some missing variable or syntax error.
			/// </summary>
			RuntimeError,
			/// <summary>
			/// The compiler was stopped due to task error. For example user submitted wrong answer or uncompleted task. 
			/// </summary>
			TaskError,
		}
	}

}