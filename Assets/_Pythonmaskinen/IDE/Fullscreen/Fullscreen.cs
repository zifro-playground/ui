using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fullscreen : MonoBehaviour, IPointerDownHandler {

	public void OnPointerDown(PointerEventData eventData) {
		Screen.fullScreen = !Screen.fullScreen;
	}
}
