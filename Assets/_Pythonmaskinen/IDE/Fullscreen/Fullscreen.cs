using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fullscreen : MonoBehaviour, IPointerDownHandler {

	// Switch to fullscreen when you press down the button. This is used because browsers only allow fullscreen to be set on user input events and a normal button pressed event is not classified as one. 
	// Now fullscreen is switched when browser gets user event PointerUp since we do not look at it.
	public void OnPointerDown(PointerEventData eventData) {
		Screen.fullScreen = !Screen.fullScreen;
	}
}
