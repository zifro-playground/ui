using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting {

	private string preCode;
	private string startCode;
	private int rowLimit;
	private string[] smartButtons;
	private int caseCount;

	public LevelSetting (string preCode = "", string startCode = "", int rowLimit = 100, string[] smartButtons = null, int numberOfCases = 1){
		this.preCode = preCode;
		this.startCode = startCode;
		this.rowLimit = rowLimit;
		this.smartButtons = smartButtons ?? new string[0];
		this.caseCount = numberOfCases;
	}

	public void UseSettings () {
		PMWrapper.preCode = preCode;
		PMWrapper.AddCodeAtStart (startCode);
		PMWrapper.codeRowsLimit = rowLimit;
		PMWrapper.SetSmartButtons (smartButtons);
	}
}
