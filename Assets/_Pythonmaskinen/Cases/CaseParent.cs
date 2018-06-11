using System.Collections.Generic;
using UnityEngine;

public class CaseParent : MonoBehaviour
{
	public List<GameObject> CaseButtons;

	public static CaseParent Instance;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}
}
