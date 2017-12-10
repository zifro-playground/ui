using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PM {

	public class CodeWalker : MonoBehaviour, IPMSpeedChanged {

		#region time based variables
		private float baseWalkerWaitTime = 3f;
		public float BaseWalkerWaitTime
		{
			get { return baseWalkerWaitTime; }
			set {
				baseWalkerWaitTime = Mathf.Clamp(value, 0.01f, 1000);
				OnPMSpeedChanged(PMWrapper.speedMultiplier);
			}
		}

		float sleepTime = 3f;
		private float sleepTimer;
		#endregion

		public static bool manusPlayerSaysICanContinue = true;
		public static bool isSleeping = false;
		public static bool walkerRunning = true;
		private static bool doEndWalker = false;
		private static Action<HelloCompiler.StopStatus> stopCompiler;
		private VariableWindow theVarWindow;
		public bool isUserPaused { get; private set; }

		//This Script needs to be added to an object in the scene
		//To start the compiler simply call "ActivateWalker" Method
		#region init
		void Start() {
			enabled = false;
			walkerRunning = false;
		}

		/// <summary>
		/// Activates the walker by telling the compiler to compile code and links necessary methods.
		/// </summary>
		public void activateWalker(Action<HelloCompiler.StopStatus> stopCompilerMeth, VariableWindow theVarWindow) {
			Compiler.SyntaxCheck.CompileCode(PMWrapper.fullCode, endWalker, pauseWalker, IDELineMarker.activateFunctionCall, IDELineMarker.SetWalkerPosition);
			enabled = true;
			walkerRunning = true;
			doEndWalker = false;
			stopCompiler = stopCompilerMeth;
			this.theVarWindow = theVarWindow;
			isUserPaused = false;
			manusPlayerSaysICanContinue = true;
		}
		#endregion


		#region CodeWalker
		// Update method runs every frame, and check if it is time to parse a line.
		// if so is the case, then we call "Runtime.CodeWalker.parseLine()" while we handle any thrown runtime exceptions that the codeWalker finds.

		void Update() {
			if (isUserPaused) return;

			if (walkerRunning && !isSleeping) {
				if (doEndWalker) {
					stopCompiler.Invoke(HelloCompiler.StopStatus.Finished);
					return;
				}
				
				if (PMWrapper.isDemoingLevel && !manusPlayerSaysICanContinue)
					return;
				
				try {

					Runtime.CodeWalker.parseLine();
					theVarWindow.updateWindow();

					if (PMWrapper.isDemoingLevel) {
						manusPlayerSaysICanContinue = false;
					}
				} catch {
					stopCompiler.Invoke(HelloCompiler.StopStatus.RuntimeError);
					throw;
				} finally {
					isSleeping = true;
				}
			}

			runSleepTimer();
		}

		private void runSleepTimer() {
			sleepTimer += Time.deltaTime;
			float firstInterval = sleepTime - sleepTime / 20;
			if (sleepTimer > firstInterval) {
				IDELineMarker.instance.SetState(IDELineMarker.State.Hidden);
				if (sleepTimer > sleepTime) {
					sleepTimer = 0;
					isSleeping = false;
				}
			}
		}
		#endregion


		#region Compiler Methodes
		//Methods that the CC should be able to call.
		//We link this to the CC by passing them into the "Runtime.CodeWalker.initCodeWalker" method
		public static void endWalker() {
			doEndWalker = true;
		}

		public static void pauseWalker() {
			walkerRunning = false;
		}
		#endregion


		#region Public Unity Methods

		public void resumeWalker() {
			sleepTimer = 0;
			isSleeping = true;
			walkerRunning = true;
		}

		public void stopWalker() {
			SetWalkerUserPaused(false);
			walkerRunning = false;
			enabled = false;
		}

		// Called by the RunCodeButton script
		public void SetWalkerUserPaused(bool paused) {
			if (paused == isUserPaused) return;

			isUserPaused = paused;

			if (isUserPaused) {
				foreach (var ev in UISingleton.FindInterfaces<IPMCompilerUserPaused>())
					ev.OnPMCompilerUserPaused();
			} else {
				foreach (var ev in UISingleton.FindInterfaces<IPMCompilerUserUnpaused>())
					ev.OnPMCompilerUserUnpaused();
			}
		}

		public void OnPMSpeedChanged(float speed) {
			float speedFactor = 1 - speed;
			sleepTime = baseWalkerWaitTime * speedFactor;
		}
		#endregion
	}

}