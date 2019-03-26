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
		public static ApiHandler instance;

		private readonly Queue<Request> requestQueue = new Queue<Request>();

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		public void AddRequestToQueue(Request request)
		{
			requestQueue.Enqueue(request);
			if (requestQueue.Count == 1)
			{
				StartCoroutine(SendRequest(request));
			}
		}
		
		private IEnumerator SendRequest(Request request)
		{
			string url = GetBaseUrl() + request.endpoint;

			using (var unityWebRequest = new UnityWebRequest(url, request.method.ToString()))
			{
				if (request.method == HttpMethod.POST)
				{
					byte[] rawBody = Encoding.UTF8.GetBytes(request.jsonString);
					unityWebRequest.uploadHandler = new UploadHandlerRaw(rawBody);
				}
				unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
				unityWebRequest.SetRequestHeader("Content-Type", "application/json");

				yield return unityWebRequest.SendWebRequest();

				if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
				{
					Debug.Log(unityWebRequest.error);
					Debug.Log(unityWebRequest.downloadHandler.text);
					if (request.errorResponsCallback != null)
					{
						request.errorResponsCallback.Invoke(unityWebRequest.downloadHandler.text);
					}
				}
				else
				{
					Debug.Log(unityWebRequest.downloadHandler.text);
					if (request.okResponsCallback != null)
					{
						request.okResponsCallback.Invoke(unityWebRequest.downloadHandler.text);
					}
				}

				requestQueue.Dequeue();

				if (requestQueue.Count > 0)
				{
					Request nextRequest = requestQueue.Peek();
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
		public HttpMethod method;
		public string endpoint;
		public string jsonString;
		public Action<string> okResponsCallback;
		public Action<string> errorResponsCallback;
	}

	public enum HttpMethod
	{
		GET,
		POST
	}
}