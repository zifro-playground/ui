using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject Screen;

    public static LoadingScreen Instance;

    private void Awake()
    {
        if (Instance == null)
		{
			Instance = this;
		}
	}

    public void Show()
    {
        Screen.SetActive(true);
    }

    public void Hide()
    {
        Screen.SetActive(false);
    }
}
