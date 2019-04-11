using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace PM
{
	public class Progress : MonoBehaviour, IPMLevelCompleted, IPMUnloadLevel
	{
		private float secondsSpentOnCurrentLevel;

		[FormerlySerializedAs("LevelData")]
		public Dictionary<string, LevelData> levelData = new Dictionary<string, LevelData>();

		public static Progress instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		void Update()
		{
			secondsSpentOnCurrentLevel += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.Escape) && Screen.fullScreen)
			{
				SaveUserLevelProgress();
			}
		}

		public void LoadUserGameProgress()
		{
			var request = new Request {
				method = HttpMethod.GET,
				endpoint = "/levels/load?gameId=" + Main.instance.gameDefinition.gameId,
				okResponsCallback = HandleOkGetResponse,
				errorResponsCallback = HandleErrorGetResponse
			};
			ApiHandler.instance.AddRequestToQueue(request);
		}

		public void HandleOkGetResponse(string response)
		{
			GameProgress gameProgress = JsonConvert.DeserializeObject<GameProgress>(response);

			foreach (LevelProgress levelProgress in gameProgress.levels)
			{
				levelData[levelProgress.levelId] = new LevelData(levelProgress);
			}

			AddMissingLevelData();
			Main.instance.StartGame();
		}

		public void HandleErrorGetResponse(string response)
		{
			AddMissingLevelData();
			Main.instance.StartGame();
		}

		private void AddMissingLevelData()
		{
			foreach (ActiveLevel level in Main.instance.gameDefinition.activeLevels)
			{
				if (!levelData.ContainsKey(level.levelId))
				{
					levelData[level.levelId] = new LevelData(level.levelId);
				}
			}
		}

		public void SaveUserLevelProgress()
		{
			SaveAndResetSecondsSpent();

			LevelProgress userProgress = CollectUserProgress();
			string jsonData = JsonConvert.SerializeObject(userProgress);

			var request = new Request {
				method = HttpMethod.POST,
				endpoint = "/levels/save",
				jsonString = jsonData
			};
			ApiHandler.instance.AddRequestToQueue(request);
		}

		private LevelProgress CollectUserProgress()
		{
			LevelData levelData = this.levelData[PMWrapper.currentLevel.id];

			var userProgress = new LevelProgress() {
				levelId = levelData.id,
				isCompleted = levelData.isCompleted,
				mainCode = levelData.mainCode,
				codeLineCount = levelData.codeLineCount,
				secondsSpent = levelData.secondsSpent
			};

			return userProgress;
		}

		private int SaveAndResetSecondsSpent()
		{
			int secondsSpent = (int)secondsSpentOnCurrentLevel;
			levelData[PMWrapper.currentLevel.id].secondsSpent = secondsSpent;

			secondsSpentOnCurrentLevel = 0;

			return secondsSpent;
		}

		public void OnPMLevelCompleted()
		{
			levelData[PMWrapper.currentLevel.id].isCompleted = true;
			SaveUserLevelProgress();
		}

		public void OnPMUnloadLevel()
		{
			if (levelData.ContainsKey(PMWrapper.currentLevel.id))
			{
				SaveUserLevelProgress();
			}
		}
	}
}