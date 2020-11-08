using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupController : MonoBehaviour
{
	[SerializeField] List<Toggle> toggles;
	
	void Start()
    {
		for (int i = 0; i < toggles.Count; i++)
		{
			int j = i;
			toggles[i].onValueChanged.AddListener(isOn => { ToggleToggle(j, isOn); });
		}
	}

	bool toggling = false;
	private void ToggleToggle(int toggleIndex, bool isOn)
	{
		if (isOn == false)
		{
			if (WasOnlyTrueToggle(toggleIndex))
			{
				toggles[toggleIndex].isOn = true;
			}
			return;
		}
		
		for (int i = 0; i < toggles.Count; i++)
		{
			if (i != toggleIndex)
			{
				toggles[i].isOn = false;
			}
		}
	}

	private bool WasOnlyTrueToggle(int toggleIndex)
	{
		int otherTrueToggles = 0;
		for (int i = 0; i < toggles.Count; i++)
		{
			if (i != toggleIndex && toggles[i].isOn)
			{
				otherTrueToggles++;
			}
		}
		return otherTrueToggles == 0;
	}
}
