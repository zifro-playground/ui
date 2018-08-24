using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    public class TestLevels : MonoBehaviour, IPMLevelCompleted, IPMCompilerStopped, IPMWrongAnswer
    {

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
        }

        public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
        {
            if (status == HelloCompiler.StopStatus.RuntimeError)
            {
                print("Runtime Error on level: " + Main.Instance.LevelData.id);
                TestNextLevel();
            }
        }

        public void OnPMWrongAnswer(string answer)
        {
            print("Wrong Answer on level: " + Main.Instance.LevelData.id);
            TestNextLevel();
        }
    }
}


