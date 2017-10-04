using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting {

	private string preCode;
	private string startCode;
	private int rowLimit;
	private string[] smartButtons;

	public LevelSetting (string pCode = "", string sCode = "", int rLimit = 100, string[] sBtns = null){
		preCode = pCode;
		startCode = sCode;
		rowLimit = rLimit;
		smartButtons = sBtns ?? new string[0];
	}

	public void UseSettings () {
		PMWrapper.preCode = preCode;
		PMWrapper.AddCodeAtStart (startCode);
		PMWrapper.codeRowsLimit = rowLimit;
		PMWrapper.SetSmartButtons (smartButtons);
	}
}
