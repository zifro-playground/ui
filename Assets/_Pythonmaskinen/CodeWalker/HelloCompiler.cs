using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

using B83.ExpressionParser;
using Compiler;
using System.Collections.ObjectModel;

namespace PM {

	public class HelloCompiler : MonoBehaviour {

		public bool isRunning { get; private set; }
		
		public CodeWalker theCodeWalker;
		public VariableWindow theVarWindow;

		[NonSerialized]
		public List<Compiler.Function> addedFunctions = new List<Compiler.Function> ();

		public readonly ReadOnlyCollection<Function> globalFunctions = new ReadOnlyCollection<Function>(new Function[] {
			new GlobalFunctions.AbsoluteValue(),
			new GlobalFunctions.ConvertToBinary(),
			new GlobalFunctions.ConvertToBoolean(),
			new GlobalFunctions.ConvertToFloat(),
			new GlobalFunctions.ConvertToHexadecimal(),
			new GlobalFunctions.ConvertToInt(name:"int"),
			new GlobalFunctions.ConvertToInt(name:"long"),
			new GlobalFunctions.ConvertToString(),
			new GlobalFunctions.LengthOf(),
			new GlobalFunctions.PrintFunction(),
			new GlobalFunctions.RoundedValue(),
			new GlobalFunctions.MinimumValue(),
			new GlobalFunctions.MaximumValue(),
			new GlobalFunctions.GetTime(),
		});

		public List<Function> allAddedFunctions {
			get {
				List<Function> allFunctions = new List<Function>(globalFunctions);
				allFunctions.AddRange(addedFunctions);
				return allFunctions;
			}
		}

		void Start() {
			Runtime.Print.printFunction = prettyPrint;
		}

		//Called from compile button
		public void compileCode() {
			if (isRunning) return;
			else isRunning = true;

			foreach (var ev in UISingleton.FindInterfaces<IPMCompilerStarted>())
				ev.OnPMCompilerStarted();

			try {
				Runtime.VariableWindow.setVariableWindowFunctions(theVarWindow.addVariable, theVarWindow.resetList);
				ErrorHandler.ErrorMessage.setLanguage();
				ErrorHandler.ErrorMessage.setErrorMethod(PMWrapper.RaiseError);
				
				Compiler.GameFunctions.setGameFunctions(allAddedFunctions);

				theCodeWalker.activateWalker(stopCompiler);
			} catch {
				stopCompiler(StopStatus.RuntimeError);
				throw;
			}
		}

		public void prettyPrint(string dasMessage) {
			if (UISingleton.instance.textField.devBuild)
				print(dasMessage);
		}

		#region stop methods
		public void stopCompilerButton() {
			stopCompiler(StopStatus.Forced);
		}

		public void stopCompiler(StopStatus status = StopStatus.Forced) {
			//if (!isRunning) return;
			/*else*/ isRunning = false;
			
			theCodeWalker.stopWalker();

			// Call stop events
			foreach (var ev in UISingleton.FindInterfaces<IPMCompilerStopped>())
				ev.OnPMCompilerStopped(status);
		}
		#endregion

		public enum StopStatus {
			/// <summary>
			/// The compiler was force-stopped mid execution. Example via pressing the stop button.
			/// </summary>
			Forced,
			/// <summary>
			/// The compiler finished successfully.
			/// </summary>
			Finished,
			/// <summary>
			/// The compiler had an error during runtime. For example some missing variable or syntax error.
			/// </summary>
			RuntimeError,
		}
	}

}