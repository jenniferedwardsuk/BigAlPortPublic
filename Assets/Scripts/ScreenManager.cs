using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
	[SerializeField] GameObject introScreen;
	[SerializeField] GameObject howToPlayScreen;
	[SerializeField] GameObject gameOverScreen;
	[SerializeField] PopupUIController popupScreen;
	[SerializeField] Text gameOverText;

	public static ScreenManager instance;

	public void Start()
	{
		instance = this;

		introScreen.SetActive(true);
	}

	public void ShowPopup(string url, string name, string propertiesData)
	{
		popupScreen.ShowFactFile(url);
		popupScreen.gameObject.SetActive(true);
	}

	public void OnQuitButtonClick()
	{
		introScreen.SetActive(true);
		gameOverScreen.SetActive(false);
	}

	public void OnHowToPlayButtonClick()
	{
		howToPlayScreen.SetActive(true);
		introScreen.SetActive(false);
	}

	public void OnHowToPlayPlayGameButtonClick()
	{
		introScreen.SetActive(true);
		howToPlayScreen.SetActive(false);
	}

	public void OnPlayGameButtonClick()
	{
		howToPlayScreen.SetActive(false);
		introScreen.SetActive(false);
	}

	public void OnGameOver(string gameoverText)
	{
		gameOverScreen.SetActive(true);
		if (gameoverText.Contains("<!--"))
		{
			Debug.LogError(gameoverText);
			gameoverText = gameoverText.Substring(0, gameoverText.IndexOf("<!--"));
		}
		gameOverText.text = gameoverText;
	}

	public void OnGameOverButtonClick()
	{
		gameOverScreen.SetActive(false);
	}
}
