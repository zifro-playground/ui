using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Manus {
	
	public class Manus {
		public bool allStepsDone { get { return allSteps.Count == 0 && currentStep == null; } }
		
		public List<Step.IStep> allSteps = new List<Step.IStep>();
		public Step.IStep currentStep { get; private set; }
		
		public Manus() {
			allSteps.Add(Step.WaitSeconds.CheatyCreate(2));
		}

		public void NextStep() {
			currentStep = DequeueStep();
			if (currentStep != null)
				currentStep.StartStep();
		}

		public void FastForwardAllSteps() {
			while ((currentStep = DequeueStep()) != null) {
				currentStep.StartStep();
			}
		}

		/// <summary>
		/// Automatically steps via <see cref="NextStep"/> when the current step is done.
		/// </summary>
		public void UpdateSteps() {
			// Uses <while> so instant steps don't hault the execution
			while (currentStep != null && currentStep.IsStepDone())
				NextStep();
		}

		private Step.IStep DequeueStep() {
			if (allSteps.Count == 0) return null;
			Step.IStep step = allSteps[0];
			allSteps.RemoveAt(0);
			return step;
		}
		
		public Manus Copy() {
			var clone = new Manus();
			clone.allSteps = new List<Step.IStep>(this.allSteps);
			return clone;
		}
	}

}