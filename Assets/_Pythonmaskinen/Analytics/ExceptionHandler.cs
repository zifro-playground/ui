using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ExceptionHandler : MonoBehaviour {

	public AnalyticsTracker tracker;
	public string message;

	public void sendErrorToAnalytics (string errorMessage){
		message = errorMessage;
		//tracker.TriggerEvent ();
	}
}
