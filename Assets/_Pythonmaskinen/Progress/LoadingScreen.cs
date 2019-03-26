using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadingScreen : MonoBehaviour
{
	[FormerlySerializedAs("Screen")]
	public GameObject screen;

	public static LoadingScreen instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void Show()
	{
		screen.SetActive(true);
	}

	public void Hide()
	{
		screen.SetActive(false);
	}
}
