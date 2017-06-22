using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent, ExecuteInEditMode]
public class BorrowLayoutElement : MonoBehaviour, ILayoutElement {

	public Object other;
	public float paddingTop = 0;
	public float paddingBottom = 0;
	public float paddingLeft = 0;
	public float paddingRight = 0;

#if UNITY_EDITOR
	private void OnValidate() {
		if (!(other is ILayoutElement)) other = null;
	}
#endif

	private ILayoutElement layout {
		get { return other != null && other is ILayoutElement ? other as ILayoutElement : null; }
	}

	public float flexibleHeight {
		get {
			return layout != null ? layout.flexibleHeight : 1;
		}
	}

	public float flexibleWidth {
		get {
			return layout != null ? layout.flexibleWidth : 1;
		}
	}

	public int layoutPriority {
		get {
			return layout != null ? layout.layoutPriority : 1;
		}
	}

	public float minHeight {
		get {
			return (layout != null ? layout.minHeight : 10) + paddingTop + paddingBottom;
		}
	}

	public float minWidth {
		get {
			return (layout != null ? layout.minWidth : 10) + paddingLeft + paddingRight;
		}
	}

	public float preferredHeight {
		get {
			return (layout != null ? layout.preferredHeight : 10) + paddingTop + paddingBottom;
		}
	}

	public float preferredWidth {
		get {
			return (layout != null ? layout.preferredWidth : 10) + paddingLeft + paddingRight;
		}
	}

	public void CalculateLayoutInputHorizontal() {
		if (layout != null)
			layout.CalculateLayoutInputVertical();
	}

	public void CalculateLayoutInputVertical() {
		if (layout != null)
			layout.CalculateLayoutInputVertical();
	}
}
