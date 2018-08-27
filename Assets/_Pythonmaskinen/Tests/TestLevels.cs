using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class TestLevels : MonoBehaviour, IPMLevelCompleted, IPMCompilerStopped
    {
        private List<TestError> testErrors = new List<TestError>(); 

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
                print(UISingleton.instance.textField.text.text);

            if (Input.GetKeyDown(KeyCode.C))
            {
                TestLevel();
            }

        }


        public void OnPMLevelCompleted()
        {
            TestNextLevel();
        }


        public void TestLevel()
        {
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
                print(testErrors.Count);
                foreach(TestError te in testErrors)
                {
                    print(te.levelId + " " + te.errorType);
                }
            }
        }

        public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
        {
            if (status == HelloCompiler.StopStatus.RuntimeError || status == HelloCompiler.StopStatus.TaskError)
            {
                testErrors.Add(new TestError(Main.Instance.LevelData.id, status.ToString()));
                //print("Runtime Error on level: " + Main.Instance.LevelData.id);
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


