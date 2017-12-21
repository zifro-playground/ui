using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PM {
	public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		
		public virtual Vector2 offset { get { return new Vector2(10, 10); } }
		protected readonly static Vector2 sizeIncrement = new Vector2(20,5);

		public string text = "Tooltip";
		public float fadeInAfter = 0.5f;
		public float fadeDuration = 0.1f;

		protected RectTransform tooltipRect;
		protected Text tooltipText;
		protected List<Graphic> tooltipGraphics;

		private Vector3 targetPos {
			get {
				var pos = UISingleton.instance.uiCamera.ScreenToWorldPoint(Input.mousePosition) + transform.TransformVector(offset);
				pos.z = -50;
				return pos;
			}
		}

		public virtual GameObject prefab { get { return UISingleton.instance.uiTooltipPrefab; } }

#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(transform.TransformPoint(new Vector3(offset.x, offset.y) + new Vector3(100, 25)), transform.TransformVector(new Vector3(200, 50)));
			Gizmos.DrawLine(transform.position, transform.TransformPoint(offset));
		}
#endif

		protected bool initialized = false;
		protected virtual void Init() {
			GameObject clone = Instantiate(prefab);
			clone.transform.SetParent(UISingleton.instance.tooltipParent, false);

			tooltipRect = clone.transform as RectTransform;
			FetchTextReferences();
			tooltipGraphics = new List<Graphic>(clone.GetComponentsInChildren<Graphic>());
			
			initialized = true;
			tooltipGraphics.ForEach(g => g.canvasRenderer.SetAlpha(0));
			
			ApplyTooltipTextChange();

		}

		protected virtual void FetchTextReferences() {
			tooltipText = tooltipRect.GetComponentInChildren<Text>();
		}

		public virtual void ApplyTooltipTextChange() {
			if (!initialized) Init();
			if (tooltipText) {
				tooltipText.text = text;

				// Reset size
				tooltipRect.position = targetPos;
				tooltipRect.sizeDelta = (prefab.transform as RectTransform).sizeDelta;

				ResizeToFit();
				LockInsideParent();
			}
		}

		protected virtual void ResizeToFit() {
			var parent = (tooltipRect.parent as RectTransform);

			while (GetPrefferedTextHeight() > tooltipText.rectTransform.rect.height) {
				tooltipRect.sizeDelta += sizeIncrement;
				tooltipRect.sizeDelta = new Vector2(
					Mathf.Min(tooltipRect.sizeDelta.x + sizeIncrement.x, parent.sizeDelta.x),
					Mathf.Min(tooltipRect.sizeDelta.y + sizeIncrement.y, parent.sizeDelta.y)
				);
				if (Vector2.Distance(tooltipRect.sizeDelta, parent.sizeDelta) <= Vector2.kEpsilon)
					break;
			}

			tooltipRect.sizeDelta = new Vector2(
				tooltipRect.sizeDelta.x,
				tooltipText.preferredHeight + 20
			);
		}

		protected void LockInsideParent() {
			// Assuming pivot of parent to be at 0.5
			// Also assuming pivot of tooltipRect to be (0,0)

			var halfSize = (tooltipRect.parent as RectTransform).sizeDelta * 0.5f;
			// Too much to the right
			if (tooltipRect.anchoredPosition.x + tooltipRect.sizeDelta.x > halfSize.x)
				tooltipRect.anchoredPosition = new Vector2(halfSize.x - tooltipRect.sizeDelta.x, tooltipRect.anchoredPosition.y);
			// Too much to the left
			if (tooltipRect.anchoredPosition.x < -halfSize.x)
				tooltipRect.anchoredPosition = new Vector2(-halfSize.x, tooltipRect.anchoredPosition.y);
			// Too much upwards
			if (tooltipRect.anchoredPosition.y + tooltipRect.sizeDelta.y > halfSize.y)
				tooltipRect.anchoredPosition = new Vector2(tooltipRect.anchoredPosition.x, halfSize.y - tooltipRect.sizeDelta.y);
			// Too much downwards
			if (tooltipRect.anchoredPosition.y < -halfSize.y)
				tooltipRect.anchoredPosition = new Vector2(tooltipRect.anchoredPosition.x, -halfSize.y);
		}

		private float GetPrefferedTextHeight() {
			Canvas.ForceUpdateCanvases();
			return tooltipText.preferredHeight;
		}

		private void Start() {
			// Spawn Tooltip
			if (!initialized) Init();
		}

		private void FixedUpdate() {
			if (tooltipGraphics.All(g => g.color.a > float.Epsilon) && initialized) {
				tooltipRect.position = targetPos;
				LockInsideParent();
			}
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			if (!initialized) Init();
			StartCoroutine(WaitBeforeAppearing());
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			if (!initialized) Init();
			StopAllCoroutines();

			tooltipGraphics.ForEach(g => g.CrossFadeAlpha(0, fadeDuration, false));
		}

		IEnumerator WaitBeforeAppearing() {
			yield return new WaitForSeconds(fadeInAfter);
			
			tooltipGraphics.ForEach(g => g.CrossFadeAlpha(1, fadeDuration, false));
		}

		private void OnDisable() {
			if (!initialized) Init();
			StopAllCoroutines();
			
			tooltipGraphics.ForEach(g => { if (g) g.CrossFadeAlpha(0, fadeDuration, false); });
		}

		private void OnDestroy() {
			Destroy(tooltipRect.gameObject);
			StopAllCoroutines();
		}

	}
}