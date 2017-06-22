using System;
using System.Collections;
using System.Collections.Generic;
using Compiler;
using UnityEngine;

namespace PM.Demo {

	public class MyScript : MonoBehaviour,
							IPMCompilerStopped, 
							IPMCompilerStarted, 
							IPMSpeedChanged, 
							IPMLevelChanged {

		public bool showErrror;
		[Multiline]
		public string[] precodes = new string[] { "world = 42\nhello = 2 * world\nhelloworld = \"hello world\"" };

		public void OnPMSpeedChanged(float speed) {
			//print("Speed change to " + PMWrapper.speedMultiplier);
		}

		public void OnPMCompilerStarted() {
			//print("It started");
		}

		public void OnPMCompilerStopped(PM.HelloCompiler.StopStatus status) {
			//print("It stopped with the status \"" + status.ToString() + "\"");
			//if (status == HelloCompiler.StopStatus.RuntimeError) PMWrapper.UnpauseWalker();
		}

		public void OnPMLevelChanged() {
			if (!PMWrapper.isDemoLevel)
				PMWrapper.preCode = PMWrapper.currentLevel < precodes.Length ? precodes[PMWrapper.currentLevel] : string.Empty;
		}

		private void Update() {
			if (showErrror) {
				showErrror = false;

				PMWrapper.RaiseError(transform.position, "Testing world position");
			}
		}

		private void Start() {
			showErrror = false;

			PMWrapper.SetCompilerFunctions(
				new MyCheatyFunction()
			);

			PMWrapper.SetSmartButtons(
				"foo()",
				//"longsmartbutton()",
				//"longsmartbutton()",
				//"longsmartbutton()",
				//"longsmartbutton()",
				"longsmartbutton()"
			);
			
		}

	}

	public class MyCheatyFunction : Compiler.Function {
		public MyCheatyFunction() {
			this.hasReturnVariable = false;
			this.inputParameterAmount.Add(0);
			this.pauseWalker = false;
			this.name = "foo";
		}

		public override Variable runFunction(Scope currentScope, Variable[] inputParas, int lineNumber) {

			PMWrapper.SetLevelCompleted();
			//PMWrapper.RaiseError(lineNumber, "BAR");

			return new Variable(null);
		}
	}

}