using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class TestLevels : MonoBehaviour, IPMLevelCompleted, IPMCompilerStopped
    {
        public bool IsTesting;
        public static TestLevels Instance;
        private List<TestError> testErrors = new List<TestError>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void OnPMLevelCompleted()
        {
            if(IsTesting)
                TestNextLevel();
        }
        
        public void TestLevel()
        {
            if(Main.Instance.LevelData.levelSettings != null && Main.Instance.LevelData.levelSettings.exampleSolutionCode != null)
                PMWrapper.mainCode = Main.Instance.LevelData.levelSettings.exampleSolutionCode;
            PMWrapper.speedMultiplier = 1;
            PMWrapper.RunCode();
        }

        public void TestNextLevel()
        {
            if (PMWrapper.currentLevel < PMWrapper.numOfLevels - 1)
            {
                PMWrapper.currentLevel++;
                TestLevel();
            }
            else
            {
                foreach (TestError te in testErrors)
                {
                    print("LevelID: " + te.levelId + " ErrorType: " + te.errorType + "\n");
                }
                print("Testing completed\n");
                IsTesting = false;
            }
        }

        public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
        {
            if(!IsTesting)
                return; 
            if (status == HelloCompiler.StopStatus.RuntimeError || status == HelloCompiler.StopStatus.TaskError)
            {
                testErrors.Add(new TestError(Main.Instance.LevelData.id, status.ToString()));
                TestNextLevel();
            }
        }

        private struct TestError
        {
            public string errorType;
            public string levelId;

            public TestError(string CaseLevelId, string caseErrorType)
            {
                levelId = CaseLevelId;
                errorType = caseErrorType;
            }
        }
    }
}


