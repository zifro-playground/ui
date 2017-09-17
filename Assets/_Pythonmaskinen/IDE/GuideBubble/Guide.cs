using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class Guide {

		public Target target;
		public int lineNumber;
		public string message;

		public Guide (Target target, string message, int lineNumber) {
			this.target = target;
			this.message = message;
			this.lineNumber = lineNumber;
		}
	}
}