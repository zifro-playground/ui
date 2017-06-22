using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM {

	[ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class MatchViewportToGameRect : MonoBehaviour {

		private Camera cam;

		[HideInInspector]
		public RectTransform theRect;
		private RectTransform rect {
			get {
				if (theRect == null)
#if UNITY_EDITOR
					if (Application.isPlaying)
						theRect = UISingleton.instance.gameCameraRect;
					else {
						var ui = FindObjectOfType<UISingleton>();
						if (ui) theRect = ui.gameCameraRect;
					}
#else
				theRect = UISingleton.instance.gameCameraRect;
#endif
				return theRect;
			}
		}

		public bool isValid { get { return theRect != null; } }

		private void Awake() {
			cam = GetComponent<Camera>();
		}

#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if (rect == null) return;

			Vector3[] corners = new Vector3[4];
			rect.GetWorldCorners(corners);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(corners[0], corners[1]);
			Gizmos.DrawLine(corners[1], corners[2]);
			Gizmos.DrawLine(corners[2], corners[3]);
			Gizmos.DrawLine(corners[3], corners[0]);

			Gizmos.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
			Gizmos.DrawLine(transform.position, corners[0]);
			Gizmos.DrawLine(transform.position, corners[1]);
			Gizmos.DrawLine(transform.position, corners[2]);
			Gizmos.DrawLine(transform.position, corners[3]);
		}

		public bool runInEditor = false;
		private void LateUpdate() {
			if (!runInEditor && !Application.isPlaying) return;
			if (cam == null) cam = GetComponent<Camera>();
			if (cam == null) return;
#else
	private void LateUpdate() {
#endif

			// Make sure we have all objects
			if (rect == null) return;
			Canvas canvas = rect.GetComponentInParent<Canvas>();
			if (!canvas) return;

			// Different calculations for different modes
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null) {

				// Get worldspace corners of recttransform
				Vector3[] corners = new Vector3[4];
				rect.GetWorldCorners(corners);

				// Transform into viewport
				Vector3[] newCorners = new Vector3[] {
				canvas.worldCamera.WorldToViewportPoint(corners[0]),
				canvas.worldCamera.WorldToViewportPoint(corners[1]),
				canvas.worldCamera.WorldToViewportPoint(corners[2]),
				canvas.worldCamera.WorldToViewportPoint(corners[3])
			};

				// Turn into Rect
				float xMin = Mathf.Clamp01(newCorners.Min(c => c.x));
				float xMax = Mathf.Clamp01(newCorners.Max(c => c.x));
				float yMin = Mathf.Clamp01(newCorners.Min(c => c.y));
				float yMax = Mathf.Clamp01(newCorners.Max(c => c.y));

				Rect viewport = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

				// ...
				// Profit!
				cam.rect = viewport;

			} /* else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {

			// Get worldspace corners of recttransform
			Vector3[] corners = new Vector3[4];
			rect.GetWorldCorners(corners);

			string tmp = "";
			for (int i = 0; i < corners.Length; i ++) {
				tmp += i + ": " + corners[i] + "\n";
			}
			print(tmp);
		}
		*/
		}
	}

}