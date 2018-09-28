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
		private float SecondsSpentOnCurrentLevel;

		public Dictionary<int, LevelData> LevelData = new Dictionary<int, LevelData>();

		public static Progress Instance;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		void Update()
		{
			SecondsSpentOnCurrentLevel += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.Escape))
				SaveUserLevelProgress();
		}

		public void SaveUserLevelProgress()
		{
			SaveAndResetSecondsSpent();

			var userProgress = CollectUserProgress();
			var rawBody = CreateRawRequestBody(userProgress);
			StartCoroutine(SendRequest("/levels/save", rawBody));
		}
		private Dictionary<string, string> CollectUserProgress()
		{
			var levelData = LevelData[PMWrapper.CurrentLevelIndex];

			var userProgress = new Dictionary<string, string>
			{
				{"levelId", levelData.Id},
				{"isCompleted", levelData.IsCompleted.ToString()},
				{"mainCode", levelData.MainCode},
				{"codeLineCount", levelData.CodeLineCount.ToString()},
				{"secondsSpent", levelData.SecondsSpent.ToString()}
			};

			return userProgress;
		}
		private byte[] CreateRawRequestBody(Dictionary<string, string> formData)
		{
			var jsonData = JsonConvert.SerializeObject(formData);
			print("Save is triggered!\n" + jsonData);
			byte[] rawBody = Encoding.UTF8.GetBytes(jsonData);

			return rawBody;
		}
		private IEnumerator SendRequest(string endpoint, byte[] rawBody)
		{
			var url = GetBaseUrl() + endpoint;
			print(url);

			var request = new UnityWebRequest(url, "POST");
			request.uploadHandler = new UploadHandlerRaw(rawBody);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.isNetworkError || request.isHttpError)
				Debug.Log(request.error);
			Debug.Log(request.downloadHandler.text);
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

		public void LoadMainCode()
		{
			if (LevelData.ContainsKey(PMWrapper.CurrentLevelIndex))
				PMWrapper.mainCode = LevelData[PMWrapper.CurrentLevelIndex].MainCode;
		}

		private int SaveAndResetSecondsSpent()
		{
			var secondsSpent = (int)SecondsSpentOnCurrentLevel;
			LevelData[PMWrapper.CurrentLevelIndex].SecondsSpent += secondsSpent;

			SecondsSpentOnCurrentLevel = 0;

			return secondsSpent;
		}

		public void OnPMLevelCompleted()
		{
			LevelData[PMWrapper.CurrentLevelIndex].IsCompleted = true;
			SaveUserLevelProgress();
		}

		public void OnPMUnloadLevel()
		{
			if (LevelData.ContainsKey(PMWrapper.CurrentLevelIndex))
			{
				SaveUserLevelProgress();
			}
		}
	}
}
