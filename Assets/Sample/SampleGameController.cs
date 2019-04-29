using PM;
using UnityEngine;

namespace Sample
{
	public class TestScript : MonoBehaviour, IPMLevelChanged, IPMCaseSwitched, IPMWrongAnswer, IPMCorrectAnswer,
		IPMTimeToCorrectCase
	{
		static TestScript()
		{
			Main.RegisterFunction(new CustomFunction());
			Main.RegisterLevelDefinitionContract<CustomLevelDefinition>();
		}

		void Start()
		{
			PMWrapper.SetCaseAnswer(1);
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
		}

		public void OnPMCaseSwitched(int caseNumber)
		{
			//if (caseNumber == 0)
			//	PMWrapper.SetCaseAnswer(1);
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
