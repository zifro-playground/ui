using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaseFlash : MonoBehaviour
{
	[HideInInspector]
	public static CaseFlash Instance;

	public Image NumberImage;
	public Sprite[] CaseImages;

	public float Duration;

	private Coroutine coroutine;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowNewCaseFlash(int caseNumber, bool startCompilerWhenFinished = false)
	{
		if (coroutine != null)
			StopCoroutine(coroutine);

		if (caseNumber < 0 || caseNumber > PMWrapper.numOfLevels-1)
			throw new ArgumentOutOfRangeException();

		NumberImage.sprite = CaseImages[caseNumber];

		coroutine = StartCoroutine(ShowFlash(startCompilerWhenFinished));
	}

	private IEnumerator ShowFlash(bool startCompilerWhenFinished)
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(Duration);

		HideFlash();

		if (startCompilerWhenFinished)
			PMWrapper.StartCompiler();
	}

	public void HideFlash()
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}
	}
}
