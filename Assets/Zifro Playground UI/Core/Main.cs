using System;
using System.Collections.Generic;
using System.IO;
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

		/// <summary>
		/// If enabled, will not activate progress class <see cref="Progress"/> to load data
		/// from <seealso cref="PlayerPrefs"/> or server-saved progress.
		/// <para>Note: is used at <see cref="Start"/>, so needs to be assigned before initialization.
		/// Such as in the scene.</para>
		/// </summary>
		[Header("Unit testing settings")]
		public bool ignoreLoadingGameProgress;

		/// <summary>
		/// If enabled, will not play guides. This includes ignoring the big task description <see cref="TaskDescription"/> class.
		/// </summary>
		[NonSerialized]
		public bool ignorePlayingGuides;

		/// <summary>
		/// If enabled, will not automatically play through next case once one case is completed.
		/// </summary>
		[NonSerialized]
		public bool ignoreNextCase;

		public static Main instance;

		private SceneSettings currentSceneSettings;
		private LevelSettings currentLevelSettings;

		static readonly Dictionary<string, IEmbeddedType> REGISTERED_NAMED_FUNCTIONS =
			new Dictionary<string, IEmbeddedType>();

		static JsonConverter jsonCaseConverter = new OverridingJsonReader<CaseDefinition, CaseDefinition>();
		static JsonConverter jsonSandboxConverter = new OverridingJsonReader<SandboxDefinition, SandboxDefinition>();
		static JsonConverter jsonLevelConverter = new OverridingJsonReader<LevelDefinition, LevelDefinition>();

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

			gameDefinition = ParseJson(gameDataFileName);

			if (!ignoreLoadingGameProgress)
			{
				Progress.instance.LoadUserGameProgressThenStartGame();
			}
			else
			{
				Debug.Log("Ignored user game progress. Note: StartGame() is not called.", this);
				Progress.instance.AddMissingLevelData();
			}
		}

		void OnDestroy()
		{
			if (loadedScene != null)
			{
				SceneManager.UnloadSceneAsync(loadedScene);
			}
		}

		public static GameDefinition ParseJson(string fileName)
		{
			TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);

			if (jsonAsset == null)
			{
				throw new FileNotFoundException("Could not find the file \"" + fileName +
				                    "\" that should contain game data in json format.", fileName);
			}

			string jsonString = jsonAsset.text;

			GameDefinition definition = JsonConvert.DeserializeObject<GameDefinition>(
				/* input */ jsonString,
				/* converters */ jsonCaseConverter, jsonLevelConverter, jsonSandboxConverter
			);

			return definition;
		}

		public void StartGame(int firstLevelIndex = 0)
		{
			// Will create level navigation buttons
			PMWrapper.numOfLevels = gameDefinition.activeLevels.Count;

			StartLevel(firstLevelIndex);

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

		void LoadScene(string sceneName)
		{
			if (sceneName != loadedScene)
			{
				var scenes = gameDefinition.scenes.Where(x => x.name == sceneName).ToList();

				if (scenes.Count > 1)
				{
					throw new InvalidOperationException("There are more than one scene with name " + sceneName);
				}

				if (!scenes.Any())
				{
					throw new InvalidOperationException("There is no scene with name " + sceneName);
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
					throw new InvalidOperationException("Scene with name " + scene.name + " exists but is not added to build settings");
				}

				SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
				loadedScene = sceneName;
			}
		}

		void LoadLevel(string levelId)
		{
			var levels = gameDefinition.scenes.First(x => x.name == loadedScene).levels.Where(x => x.id == levelId)
				.ToList();

			if (levels.Count > 1)
			{
				throw new InvalidOperationException("There are more than one level with id " + levelId);
			}

			if (!levels.Any())
			{
				throw new InvalidOperationException("There is no level with id " + levelId);
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

			if (currentLevelSettings.taskDescription != null && !ignorePlayingGuides)
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

		private void BuildGuides(List<GuideBubble> guideBubbles)
		{
			if (guideBubbles != null && guideBubbles.Any() && !ignorePlayingGuides)
			{
				var levelGuide = new LevelGuide();
				foreach (GuideBubble guideBubble in guideBubbles)
				{
					if (guideBubble.target == null || string.IsNullOrEmpty(guideBubble.text))
					{
						throw new InvalidOperationException("A guide bubble for level with index " + PMWrapper.currentLevelIndex +
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
				caseHandler = new CaseHandler(cases.Count) {
					autoContinueTest = !ignoreNextCase
				};
			}
			else
			{
				if (levelDefinition.sandbox == null)
				{
					caseHandler = new CaseHandler(1) {
						autoContinueTest = !ignoreNextCase
					};
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

		/// <summary>
		/// Overrides what type will be used when reading <see cref="LevelDefinition"/> from the game definition json.
		/// Can later be accessed by casting the <see cref="Level"/> objects <see cref="Level.levelDefinition"/> parameter to your type.
		/// </summary>
		public static void RegisterLevelDefinitionContract<TLevelDefinition>()
			where TLevelDefinition : LevelDefinition, new()
		{
			jsonLevelConverter = new OverridingJsonReader<LevelDefinition, TLevelDefinition>();
		}

		/// <summary>
		/// Overrides what type will be used when reading <see cref="SandboxDefinition"/> from the game definition json.
		/// Can later be accessed by casting the <see cref="Sandbox"/> objects <see cref="Sandbox.sandboxDefinition"/> parameter to your type.
		/// </summary>
		public static void RegisterSandboxDefinitionContract<TSandboxDefinition>()
			where TSandboxDefinition : SandboxDefinition, new()
		{
			jsonSandboxConverter = new OverridingJsonReader<SandboxDefinition, TSandboxDefinition>();
		}

		/// <summary>
		/// Overrides what type will be used when reading <see cref="CaseDefinition"/> from the game definition json.
		/// Can later be accessed by casting the <see cref="Case"/> objects <see cref="Case.caseDefinition"/> parameter to your type.
		/// </summary>
		public static void RegisterCaseDefinitionContract<TCaseDefinition>()
			where TCaseDefinition : CaseDefinition, new()
		{
			jsonCaseConverter = new OverridingJsonReader<CaseDefinition, TCaseDefinition>();
		}

		public static void RegisterFunction(IEmbeddedType function)
		{
			REGISTERED_NAMED_FUNCTIONS[function.GetType().Name] = function;
		}

		private static List<IEmbeddedType> CreateFunctionsFromStrings(ICollection<string> functionNames)
		{
			var functions = new List<IEmbeddedType>(functionNames.Count);

			foreach (string functionName in functionNames)
			{
				if (!REGISTERED_NAMED_FUNCTIONS.TryGetValue(functionName, out IEmbeddedType function))
				{
					throw new InvalidOperationException(
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
