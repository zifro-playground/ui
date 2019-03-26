using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PM
{
	public class CaseFlash : MonoBehaviour
	{
		public static CaseFlash instance;

		[FormerlySerializedAs("NumberImage")]
		public Image numberImage;

		[FormerlySerializedAs("CaseImages")]
		public Sprite[] caseImages;

		[FormerlySerializedAs("Duration")]
		public float duration;

		private Coroutine coroutine;

		private void Awake()
		{
			instance = this;
		}

		public void ShowNewCaseFlash(int caseNumber, bool startCompilerWhenFinished = false)
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}

			if (caseNumber < 0 || caseNumber > Main.instance.caseHandler.numberOfCases - 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			numberImage.sprite = caseImages[caseNumber];

			coroutine = StartCoroutine(ShowFlash(startCompilerWhenFinished));
		}

		private IEnumerator ShowFlash(bool startCompilerWhenFinished)
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
			}

			yield return new WaitForSeconds(duration);

			HideFlash();

			if (startCompilerWhenFinished)
			{
				PMWrapper.StartCompiler();
			}
		}

		public void HideFlash()
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}
	}
}