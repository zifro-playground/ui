using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PM;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace ZifroPlaygroundTests.PlayMode
{
	[TestFixture]
	[Parallelizable(ParallelScope.None)]
	public abstract class PlayThroughLevelsTests
	{
		protected UnityScene loadedScene;

		protected abstract string testingScenePath { get; }

		[UnitySetUp]
		public virtual IEnumerator UnitySetUp()
		{
			string mainScenePath = testingScenePath;

			FileAssert.Exists(mainScenePath);
			loadedScene = EditorSceneManager.LoadSceneInPlayMode(mainScenePath,
				new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
			Assert.True(loadedScene.IsValid());
			yield return null;
			Assert.True(SceneManager.SetActiveScene(loadedScene));

			if (Main.instance == null)
			{
				Assert.Fail("Class Main.cs did not initialize.");
			}

			if (Main.instance.ignoreLoadingGameProgress == false)
			{
				Assert.Fail(
					$"Class {nameof(Main)}.cs loaded game progress. This should not be enabled during testing.\n" +
					$"Fix this by setting '{nameof(Main.ignoreLoadingGameProgress)}' to 'true' in {mainScenePath}");
			}
		}

		[UnityTearDown]
		public virtual IEnumerator UnityTearDown()
		{
			yield return SceneManager.UnloadSceneAsync(loadedScene);
			loadedScene = default;
		}

		/// <summary>
		/// Loads the scene, loads the level, and then tries to walk through all the guides.
		/// Useful for catching guides with invalid targets.
		/// <para>Argument <paramref name="data"/> can be fed using the <see cref="ValueSourceAttribute"/> in combination
		/// with using <see cref="PlaygroundTestHelper.GetActiveLevels"/> from <see cref="PlaygroundTestHelper"/>.</para>
		/// <para>Note: Needs attribute <see cref="UnityTestAttribute"/> to be applied on the overridden method
		/// in your derived class for Unity Test Runner to find the method.</para>
		/// </summary>
		/// <param name="data">The level data. Use <see cref="PlaygroundTestHelper.GetActiveLevels"/>
		/// from <see cref="PlaygroundTestHelper"/> to feed automatically.</param>
		public virtual IEnumerator TestPlayGuidesInLevel(LevelTestData data)
		{
			// Arrange
			Main.instance.StartGame(data.levelIndex);
			int expectedCount = data.levelData.guideBubbles?.Count ?? 0;
			int actualCount = 0;
			yield return null;

			// Act
			if (!string.IsNullOrEmpty(data.levelData.levelSettings?.taskDescription?.header))
			{
				Assert.IsTrue(UISingleton.instance.taskDescription.bigTaskDescription.activeSelf,
					"Did not mark {0} as active for {1}", nameof(TaskDescriptionController.bigTaskDescription), data);
				Assert.IsTrue(Progress.instance.levelData[data.levelData.id].hasShownDescription,
					"Did not mark {0} as true for {1}", nameof(LevelData.hasShownDescription), data);
				UISingleton.instance.taskDescription.bigTaskDescription.SetActive(false);
			}
			else
			{
				Assert.IsFalse(UISingleton.instance.taskDescription.bigTaskDescription.activeSelf,
					"Game object {0} was active while it should not have been for {1}",
					nameof(TaskDescriptionController.bigTaskDescription), data);
				Assert.IsFalse(Progress.instance.levelData[data.levelData.id].hasShownDescription,
					"Field {0} was true while it should not have been for {1}", nameof(LevelData.hasShownDescription),
					data);
			}

			yield return new WaitForSeconds(0.1f);

			while (UISingleton.instance.guidePlayer.currentGuide?.hasNext == true ||
				   UISingleton.instance.guideBubble.isShowing)
			{
				yield return new WaitForSeconds(0.1f);
				if (UISingleton.instance.guideBubble.isShowing)
				{
					UISingleton.instance.guideBubble.HideMessage();
					actualCount++;
				}

				yield return null;
			}

			// Assert
			Assert.AreEqual(expectedCount, actualCount, "Number of guides shown did not match.");

			// Asserting is done by assuming no exceptions & no error logs
		}

		/// <summary>
		/// Loads the scene, opens the level, sets case, runs the case, then unloads the scene.
		/// Guarantees a fresh game instance between each case.
		/// Useful to try catch unwanted state between cases.
		/// <para>Argument <paramref name="data"/> can be fed using the <see cref="ValueSourceAttribute"/> in combination
		/// with using <see cref="PlaygroundTestHelper.GetActiveCases"/> from <see cref="PlaygroundTestHelper"/>.</para>
		/// <para>Note: Needs attribute <see cref="UnityTestAttribute"/> to be applied on the overridden method
		/// in your derived class for Unity Test Runner to find the method.</para>
		/// </summary>
		/// <param name="data">The level data. Use <see cref="PlaygroundTestHelper.GetActiveCases"/>
		/// from <see cref="PlaygroundTestHelper"/> to feed automatically using <see cref="ValueSourceAttribute"/>.</param>
		public virtual IEnumerator TestPlayCase(CaseTestData data)
		{
			if (data.caseData == null)
			{
				Assert.Inconclusive($"Level '{data.levelData.id}' ({data.scene.name}) has not a single case.");
			}

			// Arrange
			Main.instance.ignorePlayingGuides = true;
			Main.instance.ignoreNextCase = true;
			Main.instance.StartGame(data.levelIndex);
			yield return null;
			PMWrapper.SwitchCase(data.caseIndex);
			yield return null;
			var postSceneLoad = PostSceneLoad();
			while (postSceneLoad != null && postSceneLoad.MoveNext())
			{
				yield return postSceneLoad.Current;
			}
			yield return null;

			PMWrapper.speedMultiplier = 1;

			// Act
			if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings.exampleSolutionCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
			}
			else
			{
				Assert.Inconclusive($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
			}

			IEnumerator coroutine = PlaygroundTestHelper.RunCaseAndAssert(data);
			while (coroutine.MoveNext())
			{
				yield return coroutine.Current;
			}

			// Asserting is done by assuming no exceptions & no error logs
		}

		/// <summary>
		/// Opens the level and runs all cases in order using the same scene instance.
		/// Guarantees a fresh scene instance between each level.
		/// Useful to try catch unwanted state between levels.
		/// <para>Argument <paramref name="data"/> can be fed using the <see cref="ValueSourceAttribute"/> in combination
		/// with using <see cref="PlaygroundTestHelper.GetActiveLevels"/> from <see cref="PlaygroundTestHelper"/>.</para>
		/// <para>Note: Needs attribute <see cref="UnityTestAttribute"/> to be applied on the overridden method
		/// in your derived class for Unity Test Runner to find the method.</para>
		/// </summary>
		/// <param name="data">The level data. Use <see cref="PlaygroundTestHelper.GetActiveLevels"/>
		/// from <see cref="PlaygroundTestHelper"/> to feed automatically.</param>
		[Timeout(60_000)] // ms to complete ALL cases for level
		public virtual IEnumerator TestPlayLevel(LevelTestData data)
		{
			// Arrange
			const float caseTimeLimit = 10; // seconds
			Main.instance.ignorePlayingGuides = true;
			Main.instance.StartGame(data.levelIndex);
			yield return null;
			PMWrapper.speedMultiplier = 1;

			var postSceneLoad = PostSceneLoad();
			while (postSceneLoad != null && postSceneLoad.MoveNext())
			{
				yield return postSceneLoad.Current;
			}
			yield return null;


			// Act
			if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.exampleSolutionCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
			}
			else
			{
				Assert.Inconclusive($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
			}

			float start = Time.time;
			do
			{
				IEnumerator coroutine = PlaygroundTestHelper.RunCaseAndAssert(data);
				while (coroutine.MoveNext())
				{
					yield return coroutine.Current;
				}

				Assert.IsTrue(Time.time - start < caseTimeLimit,
					$"Compiler execution timeout! Compiler took too long to complete ALL cases in {data}. Waited {caseTimeLimit} seconds.");
			} while (!Main.instance.caseHandler.allCasesCompleted);

			// Asserting is done by assuming no exceptions & no error logs
		}

		[Timeout(120_000)] // ms to complete whole game
		public abstract IEnumerator TestPlayWholeGame();

		/// <summary>
		/// Opens the scene and runs all levels and their cases in order using the same scene instance.
		/// Useful to try catch bugs of unexpected state carry-over.
		/// <para>Argument <paramref name="game"/> can be fed using <see cref="PlaygroundTestHelper.GetActiveLevels"/>
		/// from <see cref="PlaygroundTestHelper"/>.</para>
		/// <para>Note: Needs attribute <see cref="UnityTestAttribute"/> to be applied on the overridden method
		/// in your derived class for Unity Test Runner to find the method.</para>
		/// </summary>
		/// <param name="game">The level data. Use <see cref="PlaygroundTestHelper.GetActiveLevels"/>
		/// from <see cref="PlaygroundTestHelper"/> to feed automatically.</param>
		[Timeout(120_000)] // ms to complete whole game
		public IEnumerator TestPlayWholeGame(LevelTestData[] game)
		{
			// Arrange
			const float caseTimeLimit = 10; // seconds

			Main.instance.ignorePlayingGuides = true;
			Main.instance.StartGame();
			yield return null;

			PMWrapper.speedMultiplier = 1;

			// Act
			var inconclusive = new List<string>();
			foreach (LevelTestData data in game)
			{
				Main.instance.StartLevel(data.levelIndex);
				PMWrapper.currentLevelIndex = data.levelIndex;

				yield return null;
				var postSceneLoad = PostSceneLoad();
				while (postSceneLoad != null && postSceneLoad.MoveNext())
				{
					yield return postSceneLoad.Current;
				}
				yield return null;

				if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.exampleSolutionCode))
				{
					PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
				}
				else
				{
					Debug.LogWarning($"Level {data.levelData.id} has no example solution");
					inconclusive.Add($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
				}

				float start = Time.time;
				do
				{
					IEnumerator coroutine = PlaygroundTestHelper.RunCaseAndAssert(data);
					while (coroutine.MoveNext())
					{
						yield return coroutine.Current;
					}

					Assert.IsTrue(Time.time - start < caseTimeLimit,
						"Compiler execution timeout! Compiler took too long to complete ALL cases in {0}.", data);
				} while (!Main.instance.caseHandler.allCasesCompleted);
			}

			// Assert
			if (inconclusive.Count > 0)
			{
				Assert.Inconclusive(string.Join("\n", inconclusive));
			}

			// Asserting is done by assuming no exceptions & no error logs
		}

		/// <summary>
		/// Is called just after each scene is loaded in each test among
		/// <see cref="TestPlayCase"/>, <see cref="TestPlayLevel"/>, <see cref="TestPlayGuidesInLevel"/>, and <see cref="TestPlayWholeGame()"/>
		/// </summary>
		protected virtual IEnumerator PostSceneLoad()
		{
			return null;
		}

	}
}
