using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PM
{
	public class TestLevels : MonoBehaviour, IPMLevelCompleted, IPMCompilerStopped
	{
		[FormerlySerializedAs("IsTesting")]
		public bool isTesting;

		private readonly List<TestError> testErrors = new List<TestError>();
		public static TestLevels instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		public void OnPMLevelCompleted()
		{
			if (isTesting)
			{
				TestNextLevel();
			}
		}

		public void TestLevel()
		{
			if (Main.instance.levelDefinition.levelSettings != null &&
			    Main.instance.levelDefinition.levelSettings.exampleSolutionCode != null)
			{
				PMWrapper.mainCode = Main.instance.levelDefinition.levelSettings.exampleSolutionCode;
			}

			PMWrapper.speedMultiplier = 1;
			PMWrapper.RunCode();
		}

		public void TestNextLevel()
		{
			if (PMWrapper.currentLevelIndex < PMWrapper.numOfLevels - 1)
			{
				PMWrapper.currentLevelIndex++;
				TestLevel();
			}
			else
			{
				foreach (TestError te in testErrors)
				{
					print("LevelID: " + te.levelId + " ErrorType: " + te.errorType + "\n");
				}

				print("Testing completed\n");
				isTesting = false;
			}
		}

		public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			if (!isTesting)
			{
				return;
			}

			if (status == HelloCompiler.StopStatus.RuntimeError || status == HelloCompiler.StopStatus.TaskError)
			{
				testErrors.Add(new TestError(Main.instance.levelDefinition.id, status.ToString()));
				TestNextLevel();
			}
		}

		private struct TestError
		{
			public readonly string errorType;
			public readonly string levelId;

			public TestError(string caseLevelId, string caseErrorType)
			{
				levelId = caseLevelId;
				errorType = caseErrorType;
			}
		}
	}
}