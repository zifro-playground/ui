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

		public IEnumerator LoadUserGameProgress(string endpoint)
		{
			var url = GetBaseUrl() + endpoint;
			print(url);

			using (var request = new UnityWebRequest(url, "GET"))
			{
				request.downloadHandler = new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");

				yield return request.SendWebRequest();

				if (request.isNetworkError || request.isHttpError)
				{
					Debug.Log(request.error);
					Debug.Log(request.downloadHandler.text);
					Main.Instance.StartGame();
				}
				else
				{
					HandlePositiveLevelProgressResponse(request.downloadHandler.text);
				}
			}
		}
		private void HandlePositiveLevelProgressResponse(string response)
		{
			var gameProgress = JsonConvert.DeserializeObject<GameProgress>(response);

			foreach (var levelProgress in gameProgress.levels)
			{
				LevelData[levelProgress.levelId] = new LevelData(levelProgress);
			}

			foreach (var level in Main.Instance.GameDefinition.activeLevels)
			{
				if (!LevelData.ContainsKey(level.levelId))
					LevelData[level.levelId] = new LevelData(level.levelId);
			}
			Main.Instance.StartGame();
		}

		public void SaveUserLevelProgress()
		{
			SaveAndResetSecondsSpent();

			var userProgress = CollectUserProgress();
			var jsonData = JsonConvert.SerializeObject(userProgress);
			byte[] rawBody = Encoding.UTF8.GetBytes(jsonData);

			StartCoroutine(SendPostRequest("/levels/save", rawBody));
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
		private IEnumerator SendPostRequest(string endpoint, byte[] rawBody)
		{
			var url = GetBaseUrl() + endpoint;
			print(url);

			using (var request = new UnityWebRequest(url, "POST"))
			{
				request.uploadHandler = new UploadHandlerRaw(rawBody);
				request.downloadHandler = new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");

				yield return request.SendWebRequest();

				if (request.isNetworkError || request.isHttpError)
					Debug.Log(request.error);
				Debug.Log(request.downloadHandler.text);
			}
		}

		private string GetBaseUrl()
		{
			string baseUrl;
#if UNITY_EDITOR
			baseUrl = "http://localhost:51419";
#elif UNITY_WEBGL
			var uri = new Uri(Application.absoluteURL);
			baseUrl = uri.Scheme + "://" + uri.Authority;
#endif
			return baseUrl + "/umbraco/api";
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
