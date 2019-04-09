using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
	[FormerlySerializedAs("VariableWindowBackground")]
	public Image variableWindowBackground;

	[FormerlySerializedAs("UserIcon")]
	public Image userIcon;

	[FormerlySerializedAs("GameWindowUiTheme")]
	public GameWindowUITheme gameWindowUITheme;

	public static GameWindow instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void SetGameWindowUiTheme(GameWindowUITheme theme)
	{
		if (theme == GameWindowUITheme.light)
		{
			// TODO Set light
		}
		else if (theme == GameWindowUITheme.dark)
		{
			// TODO Set dark
		}
	}
}

public enum GameWindowUITheme
{
	dark,
	light
}