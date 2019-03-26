using UnityEngine;
using System;
using JetBrains.Annotations;
using Mellis.Core.Entities;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;

namespace PM
{
	public class CodeWalker : MonoBehaviour, IPMSpeedChanged
	{
		#region time based variables

		public float sleepTime = 3f;

		[Range(0, 1)]
		public float sleepHideLineAfterMultiplier = 0.9f;

		float sleepTimeLeft;
		float speedFactor = 1;

		#endregion

		public bool walkerRunning => compiledCode != null &&
		                             compiledCode.State != ProcessState.Ended &&
		                             compiledCode.State != ProcessState.Error;

		public bool isUserPaused { get; private set; }

		int lastLineNumber;

		public int currentLineNumber => compiledCode?.CurrentSource.IsFromClr == false
			? (lastLineNumber = compiledCode.CurrentSource.FromRow)
			: lastLineNumber;

		Action<HelloCompiler.StopStatus> stopCompiler;

		IProcessor compiledCode;

		public VariableWindow theVarWindow;

		//This Script needs to be added to an object in the scene
		//To start the compiler simply call "ActivateWalker" Method

		#region init

		/// <summary>
		/// Activates the walker by telling the compiler to compile code and links necessary methods.
		/// </summary>
		public IProcessor ActivateWalker(Action<HelloCompiler.StopStatus> stopCompilerMeth)
		{
			stopCompiler = stopCompilerMeth;
			enabled = true;
			isUserPaused = false;

			lastLineNumber = 0;

			theVarWindow.resetList();

			compiledCode = new PyCompiler().Compile(PMWrapper.fullCode);

			return compiledCode;
		}

		#endregion

		#region CodeWalker

		// Update method runs every frame, and check if it is time to parse a line.
		// if so is the case, then we call "Runtime.CodeWalker.parseLine()" while we handle any thrown runtime exceptions that the codeWalker finds.

		private void Update()
		{
			if (isUserPaused)
			{
				return;
			}

			if (compiledCode.State == ProcessState.Yielded)
			{
				return;
			}

			if (sleepTimeLeft > 0)
			{
				float before = sleepTimeLeft;
				float firstInterval = sleepTime * (1 - sleepHideLineAfterMultiplier);

				sleepTimeLeft -= Time.deltaTime;

				if (before > firstInterval && sleepTimeLeft <= firstInterval)
				{
					IDELineMarker.instance.SetState(IDELineMarker.State.Hidden);
				}

				return;
			}

			if (!walkerRunning)
			{
				enabled = false;
				stopCompiler.Invoke(HelloCompiler.StopStatus.Finished);
				return;
			}

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
					sleepTimeLeft = sleepTime * (1 - sleepHideLineAfterMultiplier);
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

				stopCompiler.Invoke(HelloCompiler.StopStatus.RuntimeError);
				PMWrapper.RaiseError(e.Message);
				throw;
			}
		}

		#endregion

		#region Compiler Methodes

		// TODO: Setup input link submitter
		//public static void LinkInputSubmitter(Action<string, Scope> submitInput, Scope currentScope)
		//{
		//	SubmitInput = submitInput;
		//	CurrentScope = currentScope;
		//}

		#endregion

		#region Public Methods

		public void ResumeWalker()
		{
			if (!walkerRunning)
			{
				return;
			}

			compiledCode.ResolveYield();
			sleepTimeLeft = 0;
		}

		public void ResumeWalker(IScriptType returnValue)
		{
			if (!walkerRunning)
			{
				return;
			}

			compiledCode.ResolveYield(returnValue);
			sleepTimeLeft = 0;
		}

		public void StopWalker()
		{
			SetWalkerUserPaused(false);
			compiledCode = null;
			enabled = false;
		}

		// Called by the RunCodeButton script
		public void SetWalkerUserPaused(bool paused)
		{
			if (paused == isUserPaused)
			{
				return;
			}

			isUserPaused = paused;

			if (isUserPaused)
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

		public void OnPMSpeedChanged(float speed)
		{
			speedFactor = 1 - speed;
		}

		#endregion
	}
}