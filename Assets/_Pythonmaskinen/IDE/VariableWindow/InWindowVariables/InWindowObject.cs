using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Compiler;

namespace PM {

	public abstract class InWindowObject : MonoBehaviour {
		public float width;

		#region remove object
		public void remove() {
			StartCoroutine(removeAtEndOfFrame());
		}

		private IEnumerator removeAtEndOfFrame() {
			yield return new WaitForEndOfFrame();
			Destroy(gameObject);
		}
		#endregion
	}

}