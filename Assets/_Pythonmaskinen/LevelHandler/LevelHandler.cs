using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM {

	public class LevelHandler : MonoBehaviour {

		public void LoadLevel (int level) {
			PMWrapper.StopCompiler();

			// TODO Save mainCode to database
			UISingleton.instance.saveData.ClearPreAndMainCode ();

			// Call every implemented event
			foreach (var ev in UISingleton.FindInterfaces<IPMLevelChanged>())
				ev.OnPMLevelChanged();
		}
	}
}