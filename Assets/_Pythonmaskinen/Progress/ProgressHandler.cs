using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace PM
{
	public class ProgressHandler : MonoBehaviour
	{
		private const string saveUrl = "http://localhost:51419/umbraco/api/levels/save";

		public static ProgressHandler Instance;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
		}

		public void SaveUserProgress()
		{
			var userProgress = CollectUserProgress();
			var rawBody = CreateRawRequestBody(userProgress);
			//StartCoroutine(SendRequest(rawBody));
		}

		private Dictionary<string, string> CollectUserProgress()
		{
			var userProgress = new Dictionary<string, string>();
			userProgress.Add("levelId", Main.Instance.LevelData.id);
			userProgress.Add("isCompleted", "false");
			userProgress.Add("code", PMWrapper.fullCode);
			userProgress.Add("codeLineCount", UISingleton.instance.textField.codeLineCount.ToString());
			userProgress.Add("secondsSpent", "60");

			return userProgress;
		}

		private byte[] CreateRawRequestBody(Dictionary<string, string> formData)
		{
			var jsonData = JsonConvert.SerializeObject(formData);
			print(jsonData);
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
	}
}
