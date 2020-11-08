using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CxnSpeedController : MonoBehaviour
{
	[SerializeField] List<Toggle> toggles;

	void Start()
	{
		for (int i = 0; i < toggles.Count; i++)
		{
			int j = i;
			toggles[i].onValueChanged.AddListener(isOn => { OnToggleSelect(j, isOn); });
		}
	}

	private void OnToggleSelect(int toggleIndex, bool isOn)
	{
		if (isOn)
		{
			UIInputSender.instance.SetCxnSpeedParam(toggleIndex);
		}
	}
}
