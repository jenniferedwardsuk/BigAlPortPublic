using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
	[SerializeField] Button _openMenuButton;
	[SerializeField] Button _closeMenuButton;
	[SerializeField] GameObject _menuObject;
	[SerializeField] RectTransform _gameAreaTransform;

	private float[] gameSizes = new float[] { 1, 1.35f, 1.65f };

	// Start is called before the first frame update
	void Start()
	{
		ToggleMenu(false);
		_openMenuButton.onClick.AddListener(OpenMenu);
		_closeMenuButton.onClick.AddListener(CloseMenu);
	}

	internal void OnGameSizeSelected(int toggleIndex)
	{
		ResizeGame(gameSizes[toggleIndex]);
	}

	private void ResizeGame(float scale)
	{
		_gameAreaTransform.localScale = new Vector3(scale, scale, scale);
	}

	private void OpenMenu()
	{
		ToggleMenu(true);
	}

	private void CloseMenu()
	{
		ToggleMenu(false);
	}

	private void ToggleMenu(bool visible)
	{
		_menuObject.SetActive(visible);
	}
}
