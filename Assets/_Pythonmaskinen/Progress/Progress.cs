using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace PM
{
	public class Progress : MonoBehaviour, IPMLevelCompleted, IPMLevelChanged
	{
		private const string saveUrl = "http://localhost:51419/umbraco/api/levels/save";

		private float SecondsSpentOnCurrentLevel;

		public Dictionary<int, LevelData> LevelData = new Dictionary<int, LevelData>();

		public static Progress Instance;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
		}

		void Update()
		{
			SecondsSpentOnCurrentLevel += Time.deltaTime;
		}

		public void SaveUserProgress()
		{
			var userProgress = CollectUserProgress();
			var rawBody = CreateRawRequestBody(userProgress);
			//StartCoroutine(SendRequest(rawBody));
		}
		private Dictionary<string, string> CollectUserProgress()
		{
			var levelData = LevelData[PMWrapper.currentLevel];

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
		private IEnumerator SendRequest(byte[] rawBody)
		{
			var request = new UnityWebRequest(saveUrl, "POST");
			request.uploadHandler = new UploadHandlerRaw(rawBody);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.isNetworkError || request.isHttpError)
				Debug.Log(request.error);
			else
				Debug.Log(request.downloadHandler.text);
		}

		public void ClearCodeWindow()
		{
			PMWrapper.preCode = string.Empty;
			PMWrapper.mainCode = string.Empty;
		}
		public void LoadMainCode()
		{
			if (LevelData.ContainsKey(PMWrapper.currentLevel))
				PMWrapper.mainCode = LevelData[PMWrapper.currentLevel].MainCode;
		}

		public void OnPMLevelCompleted()
		{
			LevelData[PMWrapper.currentLevel].IsCompleted = true;

			SaveUserProgress();
		}

		public void OnPMLevelChanged()
		{
			LevelData[PMWrapper.currentLevel].SecondsSpent += (int)SecondsSpentOnCurrentLevel;
			SecondsSpentOnCurrentLevel = 0;
		}
	}
}
