using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWindowSizeController : MonoBehaviour
{
	[SerializeField] List<Toggle> toggles;
	[SerializeField] GameSettingsManager gameSettingsManager;

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
			gameSettingsManager.OnGameSizeSelected(toggleIndex);
		}
	}
}
