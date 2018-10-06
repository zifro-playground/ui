using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

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
				Instance = this;
		}

		void Update()
		{
			secondsSpentOnCurrentLevel += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.Escape))
				SaveUserLevelProgress();
		}

		public void LoadUserGameProgress()
		{
			var request = new Request
			{
				Method = HttpMethod.GET,
				Endpoint = "/levels/load?gameId=" + Main.Instance.GameDefinition.gameId,
				OkResponsCallback = HandleOkGetResponse,
				ErrorResponsCallback = HandleErrorGetResponse
			};
			ApiHandler.Instance.AddRequestToQueue(request);
		}

		public void HandleOkGetResponse(string response)
		{
			var gameProgress = JsonConvert.DeserializeObject<GameProgress>(response);

			foreach (var levelProgress in gameProgress.levels)
			{
				LevelData[levelProgress.levelId] = new LevelData(levelProgress);
			}

			AddMissingLevelData();
			Main.Instance.StartGame();
		}

		public void HandleErrorGetResponse(string response)
		{
			AddMissingLevelData();
			Main.Instance.StartGame();
		}

		private void AddMissingLevelData()
		{
			foreach (var level in Main.Instance.GameDefinition.activeLevels)
			{
				if (!LevelData.ContainsKey(level.levelId))
					LevelData[level.levelId] = new LevelData(level.levelId);
			}
		}

		public void SaveUserLevelProgress()
		{
			SaveAndResetSecondsSpent();

			var userProgress = CollectUserProgress();
			var jsonData = JsonConvert.SerializeObject(userProgress);

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
			var levelData = LevelData[PMWrapper.CurrentLevel.id];

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
			var secondsSpent = (int)secondsSpentOnCurrentLevel;
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
				SaveUserLevelProgress();
		}
	}
}
