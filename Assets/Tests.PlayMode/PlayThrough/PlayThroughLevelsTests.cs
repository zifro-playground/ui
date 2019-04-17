using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using PM;
using PM.Guide;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityScene = UnityEngine.SceneManagement.Scene;
using Object = UnityEngine.Object;

namespace Tests.PlayMode.PlayThrough
{
	[TestFixture]
	[Parallelizable(ParallelScope.None)]
	public class PlayThroughLevelsTests
	{
		const string MAIN_SCENE_PATH = "Assets/Tests.PlayMode/PlayThrough/MainSceneForTesting.unity";
		UnityScene loadedScene;

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			FileAssert.Exists(MAIN_SCENE_PATH);
			loadedScene = EditorSceneManager.LoadSceneInPlayMode(MAIN_SCENE_PATH,
				new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
			Assert.True(loadedScene.IsValid());
			yield return null;

			if (Main.instance == null)
			{
				Assert.Fail("Class Main.cs did not initialize.");
			}

			if (Main.instance.ignoreLoadingGameProgress == false)
			{
				Assert.Fail(
					$"Class {nameof(Main)}.cs loaded game progress. This should not be enabled during testing.\n" +
					$"Fix this by setting '{nameof(Main.ignoreLoadingGameProgress)}' to 'true' in {MAIN_SCENE_PATH}");
			}
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			yield return SceneManager.UnloadSceneAsync(loadedScene);
			loadedScene = default;
		}

		[UnityTest]
		public IEnumerator PlayThroughGuides([ValueSource(nameof(GetActiveLevels))] LevelTestData data)
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

			yield return new WaitForSeconds(0.5f);

			while (UISingleton.instance.guidePlayer.currentGuide?.hasNext == true ||
			       UISingleton.instance.guideBubble.isShowing)
			{
				yield return new WaitForSeconds(0.5f);
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

		[UnityTest]
		public IEnumerator PlayThroughCase([ValueSource(nameof(GetActiveCases))] CaseTestData data)
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

			PMWrapper.speedMultiplier = 1;

			// Act
			if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings.startCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.startCode;
			}
			else if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings.exampleSolutionCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
			}
			else
			{
				Assert.Inconclusive($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
			}
			
			yield return RunCompilerAndAssert(data);

			// Asserting is done by assuming no exceptions & no error logs
		}

		[UnityTest]
		public IEnumerator PlayThroughLevel([ValueSource(nameof(GetActiveLevels))] LevelTestData data)
		{
			// Arrange
			Main.instance.ignorePlayingGuides = true;
			Main.instance.StartGame(data.levelIndex);
			yield return null;

			PMWrapper.speedMultiplier = 1;

			// Act
			if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.startCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.startCode;
			}
			else if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.exampleSolutionCode))
			{
				PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
			}
			else
			{
				Assert.Inconclusive($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
			}

			float start = Time.time;
			const float limit = 60; // seconds
			do
			{
				yield return RunCompilerAndAssert(data);

				Assert.IsTrue(Time.time - start < limit,
					$"Compiler execution timeout! Compiler took too long to complete ALL cases in {data}.");
			} while (!Main.instance.caseHandler.allCasesCompleted);

			// Asserting is done by assuming no exceptions & no error logs
		}

		[UnityTest]
		public IEnumerator PlayThroughGame()
		{
			// Arrange
			LevelTestData[] levels = GetActiveLevels();

			Main.instance.ignorePlayingGuides = true;
			Main.instance.StartGame();
			yield return null;

			PMWrapper.speedMultiplier = 1;

			// Act
			var inconclusive = new List<string>();
			foreach (LevelTestData data in levels)
			{
				Main.instance.StartLevel(data.levelIndex);

				if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.startCode))
				{
					PMWrapper.mainCode = data.levelData.levelSettings.startCode;
				}
				else if (!string.IsNullOrWhiteSpace(data.levelData.levelSettings?.exampleSolutionCode))
				{
					PMWrapper.mainCode = data.levelData.levelSettings.exampleSolutionCode;
				}
				else
				{
					Debug.LogWarning($"Level {data.levelData.id} has no example solution");
					inconclusive.Add($"Level '{data.levelData.id}' ({data.scene.name}) has no example solution.");
				}

				float start = Time.time;
				const float limit = 60; // seconds
				do
				{
					yield return RunCompilerAndAssert(data);

					Assert.IsTrue(Time.time - start < limit,
						$"Compiler execution timeout! Compiler took too long to complete ALL cases in {data}.");

				} while (!Main.instance.caseHandler.allCasesCompleted);
			}

			// Assert
			if (inconclusive.Count > 0)
			{
				Assert.Inconclusive(string.Join("\n", inconclusive));
			}
			// Asserting is done by assuming no exceptions & no error logs
		}
		
		private static IEnumerator RunCompilerAndAssert(LevelTestData data)
		{
			PMWrapper.StartCompiler();
			for (int i = 0; i < 20; i++)
			{
				yield return new WaitForSeconds(1);
				if (!PMWrapper.isCompilerRunning)
				{
					break;
				}
			}

			if (PMWrapper.isCompilerRunning)
			{
				Assert.Fail(
					$"Compiler execution timeout! Compiler took too long to complete case {PMWrapper.currentCase + 1} in {data}.");
			}
		}

		public class LevelTestData
		{
			public int levelIndex;
			public Level levelData;
			public Scene scene;

			public override string ToString()
			{
				return $"level '{levelData?.id}' (scene {scene?.name}, index {levelIndex}))";
			}
		}

		public class CaseTestData : LevelTestData
		{
			public int caseIndex;
			public Case caseData;

			public override string ToString()
			{
				return
					$"case {(caseData is null ? "<?>" : (caseIndex + 1).ToString())}, level '{levelData?.id}' (scene {scene?.name}, index {levelIndex})";
			}
		}

		static LevelTestData[] GetActiveLevels()
		{
			GameDefinition definition = Main.ParseJson("game");
			var list = new List<LevelTestData>();

			foreach (Scene scene in definition.scenes)
			{
				IEnumerable<Level> activeLevels = scene.levels
					.Where(s => definition.activeLevels
						.Select(a => a.levelId)
						.Contains(s.id));

				list.AddRange(activeLevels.Select((level, index) => new LevelTestData {
					levelData = level,
					levelIndex = index,
					scene = scene
				}));
			}

			return list.ToArray();
		}

		static CaseTestData[] GetActiveCases()
		{
			LevelTestData[] levels = GetActiveLevels();
			var list = new List<CaseTestData>();

			foreach (LevelTestData data in levels)
			{
				if (data.levelData?.cases == null)
				{
					list.Add(new CaseTestData {
						levelData = data.levelData,
						levelIndex = data.levelIndex,
						scene = data.scene,
						caseData = null,
						caseIndex = -1
					});
					continue;
				}

				list.AddRange(data.levelData.cases.Select((caseData, index) => new CaseTestData {
					levelData = data.levelData,
					levelIndex = data.levelIndex,
					scene = data.scene,
					caseData = caseData,
					caseIndex = index
				}));
			}

			return list.ToArray();
		}
	}
}
