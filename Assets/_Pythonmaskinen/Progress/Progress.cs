using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PM
{
	public class Progress : MonoBehaviour, IPMLevelCompleted, IPMUnloadLevel
	{
		private float secondsSpentOnCurrentLevel;

		public Dictionary<string, LevelData> LevelData = new Dictionary<string, LevelData>();

		public static Progress Instance;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
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
			var request = new Request
			{
				Method = HttpMethod.GET,
				Endpoint = "/levels/load?gameId=" + Main.instance.gameDefinition.gameId,
				OkResponsCallback = HandleOkGetResponse,
				ErrorResponsCallback = HandleErrorGetResponse
			};
			ApiHandler.Instance.AddRequestToQueue(request);
		}

		public void HandleOkGetResponse(string response)
		{
			GameProgress gameProgress = JsonConvert.DeserializeObject<GameProgress>(response);

			foreach (LevelProgress levelProgress in gameProgress.levels)
			{
				LevelData[levelProgress.levelId] = new LevelData(levelProgress);
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
				if (!LevelData.ContainsKey(level.levelId))
				{
					LevelData[level.levelId] = new LevelData(level.levelId);
				}
			}
		}

		public void SaveUserLevelProgress()
		{
			SaveAndResetSecondsSpent();

			LevelProgress userProgress = CollectUserProgress();
			string jsonData = JsonConvert.SerializeObject(userProgress);

			var request = new Request
			{
				Method = HttpMethod.POST,
				Endpoint = "/levels/save",
				JsonString = jsonData
			};
			ApiHandler.Instance.AddRequestToQueue(request);
		}

		private LevelProgress CollectUserProgress()
		{
			LevelData levelData = LevelData[PMWrapper.CurrentLevel.id];

			var userProgress = new LevelProgress()
			{
				levelId = levelData.Id,
				isCompleted = levelData.IsCompleted,
				mainCode = levelData.MainCode,
				codeLineCount = levelData.CodeLineCount,
				secondsSpent = levelData.SecondsSpent
			};

			return userProgress;
		}

		private int SaveAndResetSecondsSpent()
		{
			int secondsSpent = (int)secondsSpentOnCurrentLevel;
			LevelData[PMWrapper.CurrentLevel.id].SecondsSpent = secondsSpent;

			secondsSpentOnCurrentLevel = 0;

			return secondsSpent;
		}

		public void OnPMLevelCompleted()
		{
			LevelData[PMWrapper.CurrentLevel.id].IsCompleted = true;
			SaveUserLevelProgress();
		}
		public void OnPMUnloadLevel()
		{
			if (LevelData.ContainsKey(PMWrapper.CurrentLevel.id))
			{
				SaveUserLevelProgress();
			}
		}
	}
}
