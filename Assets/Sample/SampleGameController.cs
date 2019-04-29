using PM;
using UnityEngine;

namespace Sample
{
	public class SampleGameController : MonoBehaviour, IPMLevelChanged, IPMCaseSwitched, IPMWrongAnswer, IPMCorrectAnswer,
		IPMTimeToCorrectCase
	{
		void OnEnable()
		{
			Main.RegisterFunction(new CustomFunction());
			Main.RegisterLevelDefinitionContract<CustomLevelDefinition>();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.U))
			{
				PMWrapper.RaiseTaskError("Nu glömde du att göra något...");
			}
		}

		public void OnPMLevelChanged()
		{
			Debug.Log("Hej värld!");
		}

		public void OnPMCaseSwitched(int caseNumber)
		{
			PMWrapper.preCode = $"case = {caseNumber + 1}";
			PMWrapper.SetCaseAnswer(caseNumber + 1);
		}

		public void OnPMWrongAnswer(string answer)
		{
			PMWrapper.RaiseTaskError("\"" + answer + "\" är inte rätt svar");
		}

		public void OnPMCorrectAnswer(string answer)
		{
			PMWrapper.SetCaseCompleted();
		}

		public void OnPMTimeToCorrectCase()
		{
			PMWrapper.SetCaseCompleted();
		}
	}
}
