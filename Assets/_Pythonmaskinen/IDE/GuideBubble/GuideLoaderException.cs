using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class GuideLoaderException : Exception {
		private readonly string _message = null;

		public GuideLoaderException(string message) {
			this._message = message;
		}
	}
}