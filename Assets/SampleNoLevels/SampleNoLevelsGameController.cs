using Sample;
using UnityEngine;

namespace SampleNoLevels
{
	public class SampleNoLevelsGameController : MonoBehaviour, IPMCompilerStarted
	{
		[TextArea]
		public string codeAtStart = "x = AnpassadFunktion()";

		void OnEnable()
		{
			PMWrapper.mainCode = codeAtStart;
			PMWrapper.SetCompilerFunctions(
				new CustomFunction()
			);
		}

		public void OnPMCompilerStarted()
		{
			Debug.Log("Hej värld!");
		}
	}
}
