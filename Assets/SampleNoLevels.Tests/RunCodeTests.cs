using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ZifroPlaygroundTests;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace SampleNoLevels.Tests
{
	public class RunCodeTests
	{
		private UnityScene loadedScene;

		[UnitySetUp]
		public IEnumerator UnitySetUp()
		{
			const string mainScenePath = "Assets/SampleNoLevels.Tests/MainSceneNoLevelsForTesting.unity";

			FileAssert.Exists(mainScenePath);
			loadedScene = EditorSceneManager.LoadSceneInPlayMode(mainScenePath,
				new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));

			Assert.True(loadedScene.IsValid());
			yield return null;
			Assert.True(SceneManager.SetActiveScene(loadedScene));
		}

		[UnityTearDown]
		public IEnumerator UnityTearDown()
		{
			yield return SceneManager.UnloadSceneAsync(loadedScene);
			loadedScene = default;
		}

		[UnityTest]
		public IEnumerator RunCodePasses()
		{
			// Arrange
			const int timeoutMilliseconds = 10_000;

			PMWrapper.mainCode = "AnpassadFunktion()";
			PMWrapper.speedMultiplier = 1;

			// Act
			return PlaygroundTestHelper.RunCompilerWithTimeout(timeoutMilliseconds);

			// Assert
			// Asserting is done by assuming no exceptions & no error logs
		}

		[UnityTest]
		public IEnumerator RunCodeErrorsAsExpected()
		{
			// Arrange
			const int timeoutMilliseconds = 10_000;

			PMWrapper.mainCode = "IckeDefinieradFunktion()";
			PMWrapper.speedMultiplier = 1;

			// Act
			var coroutine = PlaygroundTestHelper.RunCompilerWithTimeout(timeoutMilliseconds);
			while (coroutine.MoveNext())
			{
				yield return coroutine.Current;
			}

			// Assert
			LogAssert.Expect(LogType.Exception, new Regex(".*RuntimeVariableNotDefinedException.*"));
			LogAssert.Expect(LogType.Exception, new Regex(".*PMRuntimeException.*"));
		}
	}
}
