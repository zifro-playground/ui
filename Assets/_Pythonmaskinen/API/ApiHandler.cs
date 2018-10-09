using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PM
{
	public class ApiHandler : MonoBehaviour
	{
		public static ApiHandler Instance;

		private Queue<Request> requestQueue = new Queue<Request>();

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		public void AddRequestToQueue(Request request)
		{
			requestQueue.Enqueue(request);
			if (requestQueue.Count == 1)
				StartCoroutine(SendRequest(request));
		}
		
		private IEnumerator SendRequest(Request request)
		{
			var url = GetBaseUrl() + request.Endpoint;

			using (var unityWebRequest = new UnityWebRequest(url, request.Method.ToString()))
			{
				if (request.Method == HttpMethod.POST)
				{
					byte[] rawBody = Encoding.UTF8.GetBytes(request.JsonString);
					unityWebRequest.uploadHandler = new UploadHandlerRaw(rawBody);
				}
				unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
				unityWebRequest.SetRequestHeader("Content-Type", "application/json");

				yield return unityWebRequest.SendWebRequest();

				if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
				{
					Debug.Log(unityWebRequest.error);
					Debug.Log(unityWebRequest.downloadHandler.text);
					if (request.ErrorResponsCallback != null)
						request.ErrorResponsCallback.Invoke(unityWebRequest.downloadHandler.text);
				}
				else
				{
					Debug.Log(unityWebRequest.downloadHandler.text);
					if (request.OkResponsCallback != null)
						request.OkResponsCallback.Invoke(unityWebRequest.downloadHandler.text);
				}

				requestQueue.Dequeue();

				if (requestQueue.Count > 0)
				{
					var nextRequest = requestQueue.Peek();
					StartCoroutine(SendRequest(nextRequest));
				}
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
	}

	public struct Request
	{
		public HttpMethod Method;
		public string Endpoint;
		public string JsonString;
		public Action<string> OkResponsCallback;
		public Action<string> ErrorResponsCallback;
	}

	public enum HttpMethod
	{
		GET,
		POST
	}
}