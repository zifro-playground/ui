using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace PM.Manus {
	public class ManusBuildingException : Exception {

		private readonly string _message = null;
		private readonly int? _row = null;
		private readonly string _filename = null;
		private readonly string _stacktrace = null;

		public override string Message {
			get {
				if (_filename == null)
					return string.Format("Error while reading manus: {0}\nPlease check the docs for clearer instructions on manus writing.\n", _message);
				else {
					if (_row.HasValue)
						return string.Format("Error while reading manus: {0}\n(at {1}:{2})\nPlease check the docs for clearer instructions on manus writing.\n", _message, _filename, _row.Value);
					else
						return string.Format("Error while reading manus: {0}\n(at {1})\nPlease check the docs for clearer instructions on manus writing.\n", _message, _filename);
				}
			}
		}

		public override string StackTrace { get { return _stacktrace ?? base.StackTrace; } }

#if UNITY_EDITOR
		public override string HelpLink {
			get { return "https://github.com/HelloWorldSweden/PythonMaskinen-UI/blob/docs/README.md"; }
		}
#endif

		public ManusBuildingException(int rowNumber, string filename, ManusBuildingException innerException) {
			this._message = innerException._message;
			this._row = innerException._row.HasValue ? innerException._row : rowNumber;
			this._filename = innerException._filename != null ? innerException._filename : filename;
			this._stacktrace = innerException.StackTrace;
		}


		public ManusBuildingException(string message, int rowNumber, string filename) {
			this._message = message;
			this._row = rowNumber;
			this._filename = filename;
		}
		
		public ManusBuildingException(string message) {
			this._message = message;
		}
	}
}