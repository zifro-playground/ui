using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Guide {

	public class Guide {

		public string target;
		public int lineNumber;
		public string message;

		public Guide (string target, string message){
			this.target = target;
			this.message = message;
			lineNumber = -1;
		}

		public Guide (string target, string message, int lineNumber) {
			this.target = target;
			this.message = message;
			this.lineNumber = lineNumber;
		}
	}
}