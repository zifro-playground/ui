using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting {

	private string preCode;
	private string startCode;
	private int rowLimit;
	private string[] smartButtons;
	private int caseCount;

	public LevelSetting (string pCode = "", string sCode = "", int rLimit = 100, string[] sBtns = null, int numberOfCases = 1){
		preCode = pCode;
		startCode = sCode;
		rowLimit = rLimit;
		smartButtons = sBtns ?? new string[0];
		caseCount = numberOfCases;
	}

	public void UseSettings () {
		PMWrapper.preCode = preCode;
		PMWrapper.AddCodeAtStart (startCode);
		PMWrapper.codeRowsLimit = rowLimit;
		PMWrapper.SetSmartButtons (smartButtons);
	}
}
