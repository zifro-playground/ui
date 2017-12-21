using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace PM {

	/*public class OldExceptionHandler : MonoBehaviour {
		private void OnEnable() {
			Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
		}

		private void OnDisable() {
			Application.logMessageReceivedThreaded -= Application_logMessageReceivedThreaded;
		}

		private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type) {
			if (type != LogType.Exception) return;

			// Extract exception type and message from condition
			int index = condition.IndexOf(": ");
			string exceptionType = index == -1 ? "UNKNOWN" : condition.Substring(0, index);
			string exceptionMsg = index == -1 ? condition : condition.Substring(index + 2, condition.Length - index - 2);
			
			TrySendExceptionToDatabase(exceptionType, exceptionMsg);
		}

		private const string api_url = "http://jontebackis4.pythonanywhere.com/";
		public static string TrySendExceptionToDatabase(string exType, string exMsg) {
			
			try {
				if (Application.internetReachability == NetworkReachability.NotReachable) return "no internet";
				if (UISingleton.gameToken == null) return "no token";

				var form = new WWWForm();
				
#if UNITY_EDITOR
				form.AddField("platform", "editor");
#elif UNITY_WEBGL
				form.AddField("platform", "webgl");
#elif UNITY_STANDALONE_WIN
				form.AddField("platform", "windows");
#elif UNITY_STANDALONE_OSX
				form.AddField("platform", "osx");
#elif UNITY_STANDALONE_LINUX
				form.AddField("platform", "linux");
#elif UNITY_IOS
				form.AddField("platform", "ios");
#elif UNITY_ANDROID
				form.AddField("platform", "android");
#elif UNITY_WP_8_1
				form.AddField("platform", "wp");
#elif UNITY_WSA || UNITY_WSA_8_1 || UNITY_WSA_10_0
				form.AddField("platform", "wsa");
#else
				form.AddField("platform", "unknown");
#endif
				form.AddField("user", "<unknown_user>");
				form.AddField("game_token", UISingleton.gameToken);

				// Miscellaneous data
				form.AddField("current_level", PMWrapper.currentLevel);
				form.AddField("unlocked_level", PMWrapper.unlockedLevel);
				form.AddField("previous_level", PMWrapper.previousLevel);
				form.AddField("error_message", exMsg);
				form.AddField("error_type", exType);
				form.AddField("ui_version", PMWrapper.Version);

				form.AddField("main_code", PMWrapper.mainCode);
				form.AddField("pre_code", PMWrapper.preCode);
				form.AddField("error_line", IDELineMarker.lineNumber);

				var req = UnityWebRequest.Post(api_url, form);
				req.Send();
				return "sent";

			} catch {
				return "error";
			}
		}

	}*/

	public class PMRuntimeException : Exception {
		private string _rawMessage;
		public override string Message { get { return _rawMessage; } }

		public PMRuntimeException(string message) {
			this._rawMessage = message;
		}
	}

}