using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaseFlash : MonoBehaviour
{
	[HideInInspector]
	public static CaseFlash Instance;

	public Image NumberImage;
	public float Duration;

	private Coroutine coroutine;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowNewCaseFlash(Sprite sprite, bool startCompilerWhenFinished = false)
	{
		if (coroutine != null)
			StopCoroutine(coroutine);

		NumberImage.sprite = sprite;

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
