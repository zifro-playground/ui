using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM {

	[RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
	public abstract class OldAbstractPopupBubble : MonoBehaviour {

		public bool isShowing { get; private set; }

		[Header("Object references")]
		public RectTransform pointerInside;
		public RectTransform pointerOutside;
		public RectTransform bubbleRect;

		[Header("Settings")]
		public float fadeTime = 0.3f;
		public Vector2 sizeIncrement = new Vector2(10,5);
		public Vector2 maxSize = new Vector2(900, 825);
		public float distanceFromTarget = 70;

		[System.NonSerialized] public CanvasGroup canvasGroup;
		[System.NonSerialized] public Canvas canvas;
		[System.NonSerialized] protected object target;
		private Coroutine fadeRoutine = null;
		public bool isFading { get { return fadeRoutine != null; } }

		public int? targetRow { get { return target as int?; } }
		public Vector2? targetCanvasPos { get { return target as Vector2?; } }
		public Vector3? targetWorldPos { get { return target as Vector3?; } }
		public Selectable targetSelectable { get { return target as Selectable; } }
		public RectTransform targetRectTransform { get { return target as RectTransform; } }

		protected abstract void OnShowMessage();
		protected abstract void OnHideMessage();

		private void _ShowMessage(object target, bool instant) {
			if (isShowing) HideMessageInstantly();

			if (instant) {
				StopFading();
				canvasGroup.alpha = 1;
			} else
				FadeTowards(1, fadeTime);

			this.target = target;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			isShowing = true;

			foreach (Transform t in transform)
				t.gameObject.SetActive(true);

			OnShowMessage();
		}

		public void ShowMessage(RectTransform target, bool instant = false) {
			FocusOnRectTransform(target);
			_ShowMessage(target, instant);
		}

		public void ShowMessage(RectTransform target, Vector2 newPivot, bool instant = false) {
			FocusOnRectTransform(target, newPivot);
			_ShowMessage(target, instant);
		}

		public void ShowMessage(Selectable target, bool instant = false) {
			FocusOnRectTransform(target.transform as RectTransform);
			_ShowMessage(target, instant);
		}

		public void ShowMessage(Selectable target, Vector2 newPivot, bool instant = false) {
			FocusOnRectTransform(target.transform as RectTransform, newPivot);
			_ShowMessage(target, instant);
		}

		public void ShowMessage(Vector2 canvasPosition, bool instant = false) {
			FocusOnCanvasPosition(canvasPosition);
			_ShowMessage(canvasPosition, instant);
		}

		public void ShowMessage(Vector3 worldPosition, bool instant = false) {
			if (Camera.main == null) throw new System.Exception("The game camera must be marked with the \"Main Camera\" tag for world positioned errors to work.\n");

			// Try to get position from game camera
			Vector2 pos = Camera.main.WorldToViewportPoint(worldPosition);
			RectTransform canvasRect = (canvas.transform as RectTransform);

			Rect rect = Camera.main.rect;
			pos = new Vector2(
				Mathf.Lerp(rect.xMin, rect.xMax, pos.x),
				Mathf.Lerp(rect.yMin, rect.yMax, pos.y)
			);

			pos.Scale(canvasRect.sizeDelta);
			pos -= Vector2.Scale(canvasRect.sizeDelta, canvasRect.pivot);

			FocusOnCanvasPosition(pos);
			_ShowMessage(worldPosition, instant);
		}

		public void ShowMessage(int codeRow, bool instant = false) {

			RectTransform textfieldRect = UISingleton.instance.textField.theInputField.textComponent.rectTransform;

			Vector2 localPos = new Vector2(
				textfieldRect.rect.width * (1 - textfieldRect.pivot.x), // To get rightmost pos
				// codeRow is the compilers code row, which starts counting at 1
				UISingleton.instance.textField.DetermineYOffset(codeRow - 1) - 5
			);

			Vector2 canvasPos = canvas.transform.InverseTransformPoint(textfieldRect.TransformPoint(localPos));

			FocusOnCanvasPosition(canvasPos, new Vector2(-3.5f,-1));
			_ShowMessage(codeRow, instant);
		}

		public void HideMessageInstantly() {
			StopFading();
			canvasGroup.alpha = 0;
			isShowing = false;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			foreach (Transform t in transform)
				t.gameObject.SetActive(false);

			OnHideMessage();
		}

		public void HideMessage() {
			if (!isShowing) return;

			FadeTowards(0, fadeTime);
			isShowing = false;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			OnHideMessage();
		}

		private void Awake() {
			canvasGroup = GetComponent<CanvasGroup>();
			canvas = GetComponentInParent<Canvas>();
		}

		protected virtual void Start() {
			if (StartIsEnabledCheck())
				_ShowMessage(null, instant: true);
			else if (!StartIsDisabledCheck())
				HideMessageInstantly();
		}

		private bool StartIsEnabledCheck() {
			foreach (Transform t in transform)
				if (!t.gameObject.activeSelf)
					return false;
			return canvasGroup.interactable && canvasGroup.blocksRaycasts && Mathf.Approximately(canvasGroup.alpha, 1f);
		}
		private bool StartIsDisabledCheck() {
			foreach (Transform t in transform)
				if (t.gameObject.activeSelf)
					return false;
			return !canvasGroup.interactable && !canvasGroup.blocksRaycasts && Mathf.Approximately(canvasGroup.alpha, 0f);
		}

		protected void FocusOnRectTransform(RectTransform target) {
			FocusOnCanvasPosition(canvas.transform.InverseTransformPoint(target.position));
		}

		protected void FocusOnRectTransform(RectTransform target, Vector2 newPivot) {
			Vector2 size = target.rect.size;
			// Move to center
			Vector2 offset = Vector2.Scale(new Vector2(0.5f - target.pivot.x, 0.5f - target.pivot.y), size);
			// Move to new pivot
			offset += Vector2.Scale(new Vector2(newPivot.x - 0.5f, newPivot.y - 0.5f), size);

			FocusOnCanvasPosition(canvas.transform.InverseTransformPoint(target.TransformPoint(offset)));
		}

		protected void FocusOnCanvasPosition(Vector2 target) {
			FocusOnCanvasPosition(target, target);
		}

		protected void FocusOnCanvasPosition(Vector2 target, Vector2 direction) {

			// Calculate positions
			var squarified = SquareifyVector2(direction);
			var pos = target - squarified.normalized * distanceFromTarget;
			var parent = (bubbleRect.parent as RectTransform).rect;

			// Clamp the target position
			pos.x = Mathf.Clamp(pos.x, parent.xMin, parent.xMax);
			pos.y = Mathf.Clamp(pos.y, parent.yMin, parent.yMax);

			// Apply the position
			bubbleRect.pivot = Vector2.one * 0.5f + squarified * 0.5f;
			bubbleRect.anchoredPosition = pos;

			// Move the pointer
			pointerInside.anchoredPosition =
			pointerOutside.anchoredPosition =
				Vector2.Scale(bubbleRect.sizeDelta * 0.5f, squarified);

			pointerInside.localEulerAngles =
			pointerOutside.localEulerAngles =
				Vector3.forward * (Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg + 90);
		}

		IEnumerator FadeRoutine(float target, float time) {
			target = Mathf.Clamp01(target);

			while (!Mathf.Approximately(canvasGroup.alpha, target)) {
				canvasGroup.alpha = Mathf.MoveTowards(Mathf.Clamp01(canvasGroup.alpha), target, Time.deltaTime / time);
				// Wait 1 timestep
				yield return null;
			}
			
			if (Mathf.Approximately(target, 0))
				foreach (Transform t in transform)
					t.gameObject.SetActive(false);

			canvasGroup.alpha = target;
			fadeRoutine = null;
		}

		public void FadeTowards(float target, float time) {
			StopFading();
			fadeRoutine = StartCoroutine(FadeRoutine(target, time));
		}

		private void StopFading() {
			if (fadeRoutine != null)
				StopCoroutine(fadeRoutine);
			fadeRoutine = null;
		}

		public static Vector2 SquareifyVector2(Vector2 vec) {
			if (vec == Vector2.zero) return Vector2.right;

			float absX = Mathf.Abs(vec.x);
			float absY = Mathf.Abs(vec.y);

			if (absX > absY)
				return new Vector2(Mathf.Sign(vec.x), vec.y / absX);
			else
				return new Vector2(vec.x / absY, Mathf.Sign(vec.y));
		}

		public void ResizeToFit(ILayoutElement judger, RectTransform resizeThis) {

			RectTransform judgerRect = (judger as Component).transform as RectTransform;

			// Adjust size to element, through trial and error
			while (GetPrefferedElementHeight(judger) > judgerRect.rect.height) {
				Vector2 size = resizeThis.sizeDelta;
				resizeThis.sizeDelta = new Vector2(
					Mathf.Min(size.x + sizeIncrement.x, maxSize.x),
					Mathf.Min(size.y + sizeIncrement.y, maxSize.y)
				);
				// Message too long...
				if (Mathf.Approximately(resizeThis.sizeDelta.x, maxSize.x) && Mathf.Approximately(resizeThis.sizeDelta.y, maxSize.y))
					break;
			}
		}

		private static float GetPrefferedElementHeight(ILayoutElement element) {
			Canvas.ForceUpdateCanvases();
			return element.preferredHeight;
		}

	}

}