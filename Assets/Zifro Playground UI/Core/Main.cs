using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mellis.Core.Interfaces;
using Newtonsoft.Json;
using PM.Guide;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace PM
{
	[Serializable]
	public class Main : MonoBehaviour, IPMCompilerStopped, IPMLevelChanged, IPMCaseSwitched
	{
		private string loadedScene;

		[FormerlySerializedAs("GameDataFileName")]
		public string gameDataFileName;

		[FormerlySerializedAs("GameDefinition")]
		public GameDefinition gameDefinition;

		[FormerlySerializedAs("LevelDefinition")]
		public Level levelDefinition;

		[FormerlySerializedAs("LevelAnswer")]
		public LevelAnswer levelAnswer;

		[FormerlySerializedAs("CaseHandler")]
		public CaseHandler caseHandler;

		public static Main instance;

		private SceneSettings currentSceneSettings;
		private LevelSettings currentLevelSettings;

		static readonly Dictionary<string, IEmbeddedType> REGISTERED_NAMED_FUNCTIONS =
			new Dictionary<string, IEmbeddedType>();

		static Main()
		{
			RegisterFunction(new Answer());
		}

		// Everything should be placed in Awake() but there are some things that needs to be set in Awake() in some other script before the things currently in Start() is called
		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		private void Start()
		{
			LoadingScreen.instance.Show();

			gameDefinition = ParseJson();

			Progress.instance.LoadUserGameProgress();
		}

		private GameDefinition ParseJson()
		{
			TextAsset jsonAsset = Resources.Load<TextAsset>(gameDataFileName);

			if (jsonAsset == null)
			{
				throw new Exception("Could not find the file \"" + gameDataFileName +
				                    "\" that should contain game data in json format.");
			}

			string jsonString = jsonAsset.text;

			GameDefinition gameDefinition = JsonConvert.DeserializeObject<GameDefinition>(jsonString);

			return gameDefinition;
		}

		public void StartGame()
		{
			// Will create level navigation buttons
			PMWrapper.numOfLevels = gameDefinition.activeLevels.Count;

			StartLevel(0);

			LoadingScreen.instance.Hide();
		}

		public void StartLevel(int levelIndex)
		{
			string sceneName = gameDefinition.activeLevels[levelIndex].sceneName;
			LoadScene(sceneName);

			string levelId = gameDefinition.activeLevels[levelIndex].levelId;
			LoadLevel(levelId);

			foreach (IPMLevelChanged ev in UISingleton.FindInterfaces<IPMLevelChanged>())
			{
				ev.OnPMLevelChanged();
			}
		}

		private void LoadScene(string sceneName)
		{
			if (sceneName != loadedScene)
			{
				var scenes = gameDefinition.scenes.Where(x => x.name == sceneName).ToList();

				if (scenes.Count > 1)
				{
					throw new Exception("There are more than one scene with name " + sceneName);
				}

				if (!scenes.Any())
				{
					throw new Exception("There is no scene with name " + sceneName);
				}

				Scene scene = scenes.First();

				currentSceneSettings = scene.sceneSettings;

				if (loadedScene != null)
				{
					SceneManager.UnloadSceneAsync(loadedScene);
				}

				int sceneIndex = SceneUtility.GetBuildIndexByScenePath(scene.name);
				if (sceneIndex < 0)
				{
					throw new Exception("Scene with name " + scene.name + " exists but is not added to build settings");
				}

				SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
				loadedScene = sceneName;
			}
		}

		private void LoadLevel(string levelId)
		{
			var levels = gameDefinition.scenes.First(x => x.name == loadedScene).levels.Where(x => x.id == levelId)
				.ToList();

			if (levels.Count > 1)
			{
				throw new Exception("There are more than one level with id " + levelId);
			}

			if (!levels.Any())
			{
				throw new Exception("There is no level with id " + levelId);
			}

			levelDefinition = levels.First();

			currentLevelSettings = levelDefinition.levelSettings;

			LevelModeButtons.instance.CreateButtons();

			BuildGuides(levelDefinition.guideBubbles);
			BuildCases(levelDefinition.cases);

			if (levelDefinition.sandbox != null)
			{
				LevelModeController.instance.InitSandboxMode();
			}
			else
			{
				LevelModeController.instance.InitCaseMode();
			}
		}

		public void SetSettings()
		{
			ClearSettings();
			SetSceneSettings();
			SetLevelSettings();

			if (PMWrapper.levelMode == LevelMode.Sandbox)
			{
				SetSandboxSettings();
			}
			else if (PMWrapper.levelMode == LevelMode.Case)
			{
				SetCaseSettings();
			}

			LoadMainCode();
		}

		private void ClearSettings()
		{
			PMWrapper.SetTaskDescription("", "");
			PMWrapper.SetCompilerFunctions();
			PMWrapper.preCode = "";
		}

		private void SetSceneSettings()
		{
			if (currentSceneSettings.walkerStepTime > 0)
			{
				PMWrapper.walkerStepTime = currentSceneSettings.walkerStepTime;
			}

			if (currentSceneSettings.gameWindowUiLightTheme)
			{
				GameWindow.instance.SetGameWindowUiTheme(GameWindowUITheme.light);
			}
			else
			{
				GameWindow.instance.SetGameWindowUiTheme(GameWindowUITheme.dark);
			}

			if (currentSceneSettings.availableFunctions != null)
			{
				List<IEmbeddedType> availableFunctions =
					CreateFunctionsFromStrings(currentSceneSettings.availableFunctions);
				PMWrapper.SetCompilerFunctions(availableFunctions);
			}
		}

		private void SetLevelSettings()
		{
			if (currentLevelSettings == null)
			{
				return;
			}

			if (!string.IsNullOrEmpty(currentLevelSettings.precode))
			{
				PMWrapper.preCode = currentLevelSettings.precode;
			}

			if (currentLevelSettings.taskDescription != null)
			{
				PMWrapper.SetTaskDescription(currentLevelSettings.taskDescription.header,
					currentLevelSettings.taskDescription.body);
			}
			else
			{
				PMWrapper.SetTaskDescription("", "");
			}

			if (currentLevelSettings.rowLimit > 0)
			{
				PMWrapper.codeRowsLimit = currentLevelSettings.rowLimit;
			}

			if (currentLevelSettings.availableFunctions != null)
			{
				List<IEmbeddedType> availableFunctions =
					CreateFunctionsFromStrings(currentLevelSettings.availableFunctions);
				PMWrapper.AddCompilerFunctions(availableFunctions);
			}
		}

		private void SetCaseSettings()
		{
			if (levelDefinition.cases != null && levelDefinition.cases.Any())
			{
				CaseSettings caseSettings = levelDefinition.cases[PMWrapper.currentCase].caseSettings;

				if (caseSettings == null)
				{
					return;
				}

				if (!string.IsNullOrEmpty(caseSettings.precode))
				{
					PMWrapper.preCode = caseSettings.precode;
				}

				if (caseSettings.walkerStepTime > 0)
				{
					PMWrapper.walkerStepTime = caseSettings.walkerStepTime;
				}
			}
		}

		private void SetSandboxSettings()
		{
			if (levelDefinition.sandbox != null)
			{
				SandboxSettings sandboxSettings = levelDefinition.sandbox.sandboxSettings;

				if (sandboxSettings == null)
				{
					return;
				}

				if (!string.IsNullOrEmpty(sandboxSettings.precode))
				{
					PMWrapper.preCode = sandboxSettings.precode;
				}

				if (sandboxSettings.walkerStepTime > 0)
				{
					PMWrapper.walkerStepTime = sandboxSettings.walkerStepTime;
				}
			}
		}

		private static void BuildGuides(List<GuideBubble> guideBubbles)
		{
			if (guideBubbles != null && guideBubbles.Any())
			{
				var levelGuide = new LevelGuide();
				foreach (GuideBubble guideBubble in guideBubbles)
				{
					if (guideBubble.target == null || string.IsNullOrEmpty(guideBubble.text))
					{
						throw new Exception("A guide bubble for level with index " + PMWrapper.currentLevelIndex +
						                    " is missing target or text");
					}

					// Check if target is a number
					Match match = Regex.Match(guideBubble.target, @"^[0-9]+$");
					if (match.Success)
					{
						int.TryParse(guideBubble.target, out int lineNumber);
						levelGuide.guides.Add(new Guide.Guide(guideBubble.target, guideBubble.text, lineNumber));
					}
					else
					{
						levelGuide.guides.Add(new Guide.Guide(guideBubble.target, guideBubble.text));
					}
				}

				UISingleton.instance.guidePlayer.currentGuide = levelGuide;
			}
			else
			{
				UISingleton.instance.guideBubble.HideMessage();
				UISingleton.instance.guidePlayer.currentGuide = null;
			}
		}

		private void BuildCases(List<Case> cases)
		{
			if (cases != null && cases.Any())
			{
				caseHandler = new CaseHandler(cases.Count);
			}
			else
			{
				if (levelDefinition.sandbox == null)
				{
					caseHandler = new CaseHandler(1);
				}
			}
		}

		private void LoadMainCode()
		{
			if (Progress.instance.levelData[PMWrapper.currentLevel.id].isStarted)
			{
				PMWrapper.mainCode = Progress.instance.levelData[PMWrapper.currentLevel.id].mainCode;
			}
			else
			{
				if (currentLevelSettings?.startCode != null)
				{
					PMWrapper.AddCodeAtStart(currentLevelSettings.startCode);
				}
				else
				{
					PMWrapper.mainCode = string.Empty;
				}

				Progress.instance.levelData[PMWrapper.currentLevel.id].isStarted = true;
			}
		}

		public static void RegisterFunction(IEmbeddedType function)
		{
			REGISTERED_NAMED_FUNCTIONS[function.GetType().Name] = function;
			//REGISTERED_NAMED_FUNCTIONS[function.FunctionName] = function;
		}

		private static List<IEmbeddedType> CreateFunctionsFromStrings(ICollection<string> functionNames)
		{
			var functions = new List<IEmbeddedType>(functionNames.Count);

			foreach (string functionName in functionNames)
			{
				if (!REGISTERED_NAMED_FUNCTIONS.TryGetValue(functionName, out IEmbeddedType function))
				{
					throw new Exception(
						$"Unable to find function: \"{functionName}\". " +
						$"Perhaps you forgot to register it via {nameof(Main)}.{nameof(RegisterFunction)}()?");
				}

				functions.Add(function);
			}

			return functions;
		}

		public void OnPMCompilerStopped(StopStatus status)
		{
			if (levelAnswer != null)
			{
				levelAnswer.compilerHasBeenStopped = true;
			}

			if (status == StopStatus.Finished)
			{
				if (PMWrapper.levelShouldBeAnswered &&
				    UISingleton.instance.taskDescription.isActiveAndEnabled)
				{
					PMWrapper.RaiseTaskError("Fick inget svar");
				}
			}
		}

		public void OnPMLevelChanged()
		{
			PMWrapper.StopCompiler();
			StopAllCoroutines();
		}

		public void OnPMCaseSwitched(int caseNumber)
		{
			StopAllCoroutines();
			UISingleton.instance.answerBubble.HideMessage();
		}
	}
}