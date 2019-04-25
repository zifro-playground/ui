using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using PM;
using UnityEngine;

namespace ZifroPlaygroundTests
{
	public static class PlaygroundTestHelper
	{
		/// <summary>
		/// Runs the code in the code window and asserts a successful outcome for the case.
		/// <para>Note: Do not yield the direct return value of this method.
		/// Save it to a variable and iterate it manually and yield the current value <see cref="IEnumerator.Current"/>
		/// on the <see cref="IEnumerator"/></para>
		/// <example>
		/// IEnumerator coroutine = PlaygroundTestHelper.RunCaseAndAssert(data);
		///	while (coroutine.MoveNext())
		/// {
		///		yield return coroutine.Current;
		/// }
		/// </example>
		/// </summary>
		public static IEnumerator RunCaseAndAssert(LevelTestData data, int timeoutMilliseconds = 20_000)
		{
			int caseIndex = PMWrapper.currentCase;
			Assert.AreEqual(LevelCaseState.Active, PMWrapper.caseStates[caseIndex],
				"Case {0} was not marked as Active in {1}", caseIndex + 1, data);

			PMWrapper.StartCompiler();
			
			var watch = Stopwatch.StartNew();

			while (PMWrapper.isCompilerRunning)
			{
				if (watch.ElapsedMilliseconds > timeoutMilliseconds)
				{
					Assert.Fail("Compiler execution timeout! Compiler took too long to complete case {0} in {1}.",
						PMWrapper.currentCase + 1, data);
				}

				yield return null;
			}

			Assert.AreEqual(LevelCaseState.Completed, PMWrapper.caseStates[caseIndex],
				"Case {0} did not get marked Completed in {1}", caseIndex + 1, data);
			yield return null;
		}

		/// <summary>
		/// Loads the json save file asset and returns the active levels.
		/// </summary>
		/// <param name="gameFile">Path to save file asset, relative to Resources folder, without extension. If placed in {project root}/Assets/Resources/game.json, then string "game" will suffice.</param>
		public static LevelTestData[] GetActiveLevels(string gameFile)
		{
			GameDefinition definition = Main.ParseJson(gameFile);
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

		/// <summary>
		/// Loads the json save file asset and returns a flattened array of the cases from the active levels.
		/// </summary>
		/// <param name="gameFile">Path to save file asset, relative to Resources folder, without extension. If placed in {project root}/Assets/Resources/game.json, then string "game" will suffice.</param>
		public static CaseTestData[] GetActiveCases(string gameFile)
		{
			LevelTestData[] levels = GetActiveLevels(gameFile);
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
