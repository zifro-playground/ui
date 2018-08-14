using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Compiler;
using Newtonsoft.Json;
using PM.Guide;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PM
{
	[Serializable]
	public class Main : MonoBehaviour, IPMCompilerStopped, IPMLevelChanged, IPMCaseSwitched
	{
		private string loadedScene;
		public string GameDataFileName;

		public GameDefinition GameDefinition;

		public Level LevelData;
		public LevelAnswer LevelAnswer;

		public CaseHandler CaseHandler;

		public static Main Instance;

		private SceneSettings currentSceneSettings;

		// Everything should be placed in Awake() but there are some things that needs to be set in Awake() in some other script before the things currently in Start() is called
		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		private void Start ()
		{
			GameDefinition = ParseJson();

			// Will create level navigation buttons
			PMWrapper.numOfLevels = GameDefinition.activeLevels.Count;

			StartLevel(0); // TODO Load last level played from database
		}

		private GameDefinition ParseJson()
		{
			var jsonAsset = Resources.Load<TextAsset>(GameDataFileName);

			if (jsonAsset == null)
				throw new Exception("Could not find the file \"" + GameDataFileName + "\" that should contain game data in json format.");

			string jsonString = jsonAsset.text;

			var gameDefinition = JsonConvert.DeserializeObject<GameDefinition>(jsonString);

			return gameDefinition;
		}

		public void StartLevel(int levelIndex)
		{
			var sceneName = GameDefinition.activeLevels[levelIndex].sceneName;
			LoadScene(sceneName);

			var levelId = GameDefinition.activeLevels[levelIndex].levelId;
			LoadLevel(levelId);

			foreach (var ev in UISingleton.FindInterfaces<IPMLevelChanged>())
				ev.OnPMLevelChanged();
		}

		private void LoadScene(string sceneName)
		{
			if (sceneName != loadedScene)
			{
				var scenes = GameDefinition.scenes.Where(x => x.name == sceneName).ToList();

				if (scenes.Count > 1)
					throw new Exception("There are more than one scene with name " + sceneName);
				if (!scenes.Any())
					throw new Exception("There is no scene with name " + sceneName);

				var scene = scenes.First();

				currentSceneSettings = scene.sceneSettings;

				if (loadedScene != null)
					SceneManager.UnloadSceneAsync(loadedScene);

				var sceneIndex = SceneUtility.GetBuildIndexByScenePath(scene.name);
				if (sceneIndex < 0)
					throw new Exception("Scene with name " + scene.name + " exists but is not added to build settings");

				SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
				loadedScene = sceneName;
			}
		}
		private void LoadLevel(string levelId)
		{
			var levels = GameDefinition.scenes.First(x => x.name == loadedScene).levels.Where(x => x.id == levelId).ToList();

			if (levels.Count > 1)
				throw new Exception("There are more than one level with id " + levelId);
			if (!levels.Any())
				throw new Exception("There is no level with id " + levelId);

			LevelData = levels.First();

			ClearSettings();
			SetSceneSettings(currentSceneSettings);
			SetLevelSettings(LevelData.levelSettings);

			LevelModeButtons.Instance.CreateButtons();

			BuildGuides(LevelData.guideBubbles);
			BuildCases(LevelData.cases);

			if (LevelData.sandbox != null)
				LevelModeController.Instance.InitSandboxMode();
			else
				LevelModeController.Instance.InitCaseMode();
		}	

		private void ClearSettings()
		{
			PMWrapper.SetTaskDescription("", "");
			PMWrapper.SetCompilerFunctions(new List<Function>());
		}
		private void SetSceneSettings(SceneSettings sceneSettings)
		{
			currentSceneSettings = sceneSettings;

			if (sceneSettings.walkerStepTime > 0)
				PMWrapper.walkerStepTime = sceneSettings.walkerStepTime;

			if (sceneSettings.gameWindowUiLightTheme)
				GameWindow.Instance.SetGameWindowUiTheme(GameWindowUiTheme.light);
			else
				GameWindow.Instance.SetGameWindowUiTheme(GameWindowUiTheme.dark);

			if (currentSceneSettings.availableFunctions != null)
			{
				var availableFunctions = CreateFunctionsFromStrings(currentSceneSettings.availableFunctions);
				PMWrapper.SetCompilerFunctions(availableFunctions);
			}
		}
		private void SetLevelSettings(LevelSettings levelSettings)
		{
			UISingleton.instance.saveData.ClearPreAndMainCode();

			if (levelSettings == null)
				return;

			if (!String.IsNullOrEmpty(levelSettings.precode))
				PMWrapper.preCode = levelSettings.precode;

			if (!String.IsNullOrEmpty(levelSettings.startCode))
				PMWrapper.AddCodeAtStart(levelSettings.startCode);
			
			if (levelSettings.taskDescription != null)
                PMWrapper.SetTaskDescription(levelSettings.taskDescription.header,levelSettings.taskDescription.body);
			else
				PMWrapper.SetTaskDescription("", "");

			if (levelSettings.rowLimit > 0)
				PMWrapper.codeRowsLimit = levelSettings.rowLimit;

			if (levelSettings.availableFunctions != null)
			{
				var availableFunctions = CreateFunctionsFromStrings(levelSettings.availableFunctions);
				PMWrapper.AddCompilerFunctions(availableFunctions);
			}
		}

		private void BuildGuides(List<GuideBubble> guideBubbles)
		{
			if (guideBubbles != null && guideBubbles.Any())
			{
				var levelGuide = new LevelGuide();
				foreach (var guideBubble in guideBubbles)
				{
					if (guideBubble.target == null || String.IsNullOrEmpty(guideBubble.text))
						throw new Exception("A guide bubble for level with index " + PMWrapper.currentLevel + " is missing target or text");

					// Check if target is a number
					Match match = Regex.Match(guideBubble.target, @"^[0-9]+$");
					if (match.Success)
					{
						int lineNumber;
						int.TryParse(guideBubble.target, out lineNumber);
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
				CaseHandler = new CaseHandler(cases.Count);
			else
				CaseHandler = new CaseHandler(1);

            foreach (var ev in UISingleton.FindInterfaces<IPMCaseSwitched>())
                ev.OnPMCaseSwitched(0);
		}

		private List<Function> CreateFunctionsFromStrings(List<string> functionNames)
		{
			var functions = new List<Function>();
			// Use reflection to get an instance of compiler function class from string
			foreach (string functionName in functionNames)
			{
				Type type = Type.GetType(functionName);

				if (type == null)
					throw new Exception("Error when trying to read available functions. Function name: \"" + functionName + "\" could not be found.");

				Function function = (Function)Activator.CreateInstance(type);
				functions.Add(function);
			}

			return functions;
		}

		public void SetSandboxSettings()
		{
			if (LevelData.sandbox != null)
			{
				var sandboxSettings = LevelData.sandbox.sandboxSettings;

				if (sandboxSettings == null)
				{
					if (LevelData.levelSettings == null || string.IsNullOrEmpty(LevelData.levelSettings.precode))
						PMWrapper.preCode = "";
					return;
				}

				if (!String.IsNullOrEmpty(sandboxSettings.precode))
					PMWrapper.preCode = sandboxSettings.precode;

				if (sandboxSettings.walkerStepTime > 0)
					PMWrapper.walkerStepTime = sandboxSettings.walkerStepTime;
			}
			else
			{
				if (LevelData.levelSettings == null || string.IsNullOrEmpty(LevelData.levelSettings.precode))
					PMWrapper.preCode = "";
			}
		}



		public void OnPMCompilerStopped(HelloCompiler.StopStatus status)
		{
			if (LevelAnswer != null)
				LevelAnswer.compilerHasBeenStopped = true;

			if (status == HelloCompiler.StopStatus.Finished)
			{
				if (PMWrapper.levelShouldBeAnswered && UISingleton.instance.taskDescription.isActiveAndEnabled)
					PMWrapper.RaiseTaskError("Fick inget svar");
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
