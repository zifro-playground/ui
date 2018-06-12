using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
	public Image VariableWindowBackground;
	public Image UserIcon;

	public GameWindowUiTheme GameWindowUiTheme;

	public static GameWindow Instance;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	public void SetGameWindowUiTheme(GameWindowUiTheme theme)
	{
		if (theme == GameWindowUiTheme.light)
		{
			// TODO Set light
		}
		else if (theme == GameWindowUiTheme.dark)
		{
			// TODO Set dark
		}
	}
}

public enum GameWindowUiTheme
{
	dark,
	light
}