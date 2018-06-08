using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IPMLevelChanged, IPMCaseSwitched, IPMWrongAnswer, IPMCorrectAnswer
{
	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
			PMWrapper.RaiseTaskError("Nu glömde du att göra något...");
	}

	public void OnPMLevelChanged()
	{
	}

	public void OnPMCaseSwitched(int caseNumber)
	{
		if (caseNumber == 0)
			PMWrapper.SetCaseAnswer(1);
	}

	public void OnPMWrongAnswer(string answer)
	{
		PMWrapper.RaiseTaskError("\"" + answer + "\" är inte rätt svar");
	}

	public void OnPMCorrectAnswer(string answer)
	{
		PMWrapper.SetCaseCompleted();
	}
}
