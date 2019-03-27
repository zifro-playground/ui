using System;
using System.Collections.Generic;
using System.Linq;
using Mellis.Core.Entities;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;
using PM.GlobalFunctions;
using UnityEngine;

namespace PM
{
	public class CodeWalker : MonoBehaviour
	{
		[Header("Settings")]
		[Range(0, 1)]
		public float hideLineWhenTimeLeftFactor = 0.1f;

		public float sleepTime = 3f;

		public VariableWindow theVarWindow;

		IProcessor compiledCode;
		int lastLineNumber;
		float sleepTimeLeft;

		public int currentLineNumber => compiledCode?.CurrentSource.IsFromClr == false
			? lastLineNumber = compiledCode.CurrentSource.FromRow
			: lastLineNumber;

		public bool isWalkerRunning => compiledCode != null &&
		                               compiledCode.State != ProcessState.Ended &&
		                               compiledCode.State != ProcessState.Error;
		public bool isUserPaused { get; private set; }
		public bool isYielded => compiledCode?.State == ProcessState.Yielded;

		public readonly List<IEmbeddedType> addedFunctions = new List<IEmbeddedType>();

		static readonly IReadOnlyCollection<IEmbeddedType> BUILTIN_FUNCTIONS = new IClrFunction[]{
			new AbsoluteValue(),
			new ConvertToBinary(),
			new ConvertToHexadecimal(),
			new LengthOf(),
			new RoundedValue(),
			new MinimumValue(),
			new MaximumValue(),
			new GetTime()
		};

		static IProcessor CreateProcessor()
		{
			return new PyCompiler().Compile(PMWrapper.fullCode);
		}

		public void CompileFullCode()
		{
			enabled = true;

			lastLineNumber = 0;

			theVarWindow.ResetList();

			foreach (IPMCompilerStarted ev in UISingleton.FindInterfaces<IPMCompilerStarted>())
			{
				ev.OnPMCompilerStarted();
			}

			try
			{
				compiledCode = CreateProcessor();
				compiledCode.AddBuiltin(BUILTIN_FUNCTIONS.ToArray());
				compiledCode.AddBuiltin(addedFunctions.ToArray());
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

		public void ResolveYield(IScriptType returnValue = null)
		{
			if (!isWalkerRunning)
			{
				return;
			}

			compiledCode.ResolveYield(returnValue);
			sleepTimeLeft = 0;
		}

		// Called by the RunCodeButton script
		public void SetWalkerUserPaused(bool paused)
		{
			if (!isWalkerRunning)
			{
				return;
			}

			enabled = !paused;
			isUserPaused = paused;

			if (paused)
			{
				foreach (IPMCompilerUserPaused ev in UISingleton.FindInterfaces<IPMCompilerUserPaused>())
				{
					ev.OnPMCompilerUserPaused();
				}
			}
			else
			{
				foreach (IPMCompilerUserUnpaused ev in UISingleton.FindInterfaces<IPMCompilerUserUnpaused>())
				{
					ev.OnPMCompilerUserUnpaused();
				}
			}
		}

		public void EvaluateEnabled()
		{
			enabled = isWalkerRunning && !isUserPaused;
		}

		// UnityEvent button
		public void StopCompilerButton()
		{
			StopCompiler(StopStatus.UserForced);
		}

		public void StopCompiler(StopStatus status = StopStatus.CodeForced)
		{
			SetWalkerUserPaused(false);
			compiledCode = null;
			EvaluateEnabled();

			// Call stop events
			foreach (IPMCompilerStopped ev in UISingleton.FindInterfaces<IPMCompilerStopped>())
			{
				ev.OnPMCompilerStopped(status);
			}
		}

		void Update()
		{
			if (compiledCode.State == ProcessState.Yielded)
			{
				return;
			}

			if (sleepTimeLeft > 0)
			{
				float before = sleepTimeLeft;
				float firstInterval = sleepTime * (1 - hideLineWhenTimeLeftFactor);

				sleepTimeLeft -= Time.deltaTime;

				if (before > firstInterval && sleepTimeLeft <= firstInterval)
				{
					IDELineMarker.instance.SetState(IDELineMarker.State.Hidden);
				}

				return;
			}

			if (!isWalkerRunning)
			{
				StopCompiler(StopStatus.Finished);
				return;
			}

			WalkLine();
		}

		void WalkLine()
		{
			try
			{
				IDELineMarker.SetWalkerPosition(currentLineNumber);

				compiledCode.WalkLine();

				if (!compiledCode.CurrentSource.IsFromClr)
				{
					lastLineNumber = compiledCode.CurrentSource.FromRow;
				}

				theVarWindow.UpdateList(compiledCode);

				if (compiledCode.State == ProcessState.Yielded)
				{
					sleepTimeLeft = sleepTime * (1 - hideLineWhenTimeLeftFactor);
				}
				else
				{
					sleepTimeLeft = Mathf.Clamp(sleepTime * (1 - PMWrapper.speedMultiplier), 0.01f, 1000);
				}
			}
			catch (Exception e)
			{
				if (!compiledCode.CurrentSource.IsFromClr)
				{
					lastLineNumber = compiledCode.CurrentSource.FromRow;
					IDELineMarker.SetWalkerPosition(currentLineNumber);
				}

				StopCompiler(StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.Message);
				throw;
			}
		}
	}
}