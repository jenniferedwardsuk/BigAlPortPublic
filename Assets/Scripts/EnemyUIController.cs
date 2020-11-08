using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour
{
	[SerializeField] int enemyNum;

	[SerializeField] Image enemyImage;
	[SerializeField] Text enemyNameText;

	[SerializeField] RectTransform energybar;
	[SerializeField] Image energybarImage;
	[SerializeField] Text enemyEnergyText;

	[SerializeField] RectTransform agilitybar;
	[SerializeField] Image agilitybarImage;
	[SerializeField] Text enemyAgilityText;

	[SerializeField] RectTransform fiercebar;
	[SerializeField] Image fiercebarImage;
	[SerializeField] Text enemyFiercenessText;

	[SerializeField] Button attackbutton;
	[SerializeField] Button matebutton;
	[SerializeField] Button FFbutton;

	[SerializeField] Sprite greenSprite;
	[SerializeField] Sprite yellowSprite;
	[SerializeField] Sprite redSprite;

	[SerializeField] List<Sprite> enemySprites;
	private Dictionary<string, Sprite> enemySpriteDictionary;
	private BigAl_pl _game;

	private KeyCode attackKeycode = KeyCode.Alpha1;
	private string attackCommand;

	private void Start()
	{
		attackKeycode = 
			  enemyNum == 0 ? KeyCode.Alpha1
			: enemyNum == 1 ? KeyCode.Alpha2
			: enemyNum == 2 ? KeyCode.Alpha3
			: enemyNum == 3 ? KeyCode.Alpha4 
			: KeyCode.Escape;
	}

	private void Update()
	{
		if (attackKeycode != KeyCode.Escape && Input.GetKeyDown(attackKeycode))
		{
			UIInputSender.instance.SendInput(attackCommand);
		}
	}

	internal void SetEnemy(BigAl_pl game, string enemyKey, PerlInterface.speciesObj enemy, bool mateAvailable)
	{
		_game = game;

		if (enemySpriteDictionary == null)
		{
			enemySpriteDictionary = enemySprites.ToDictionary(x => x.name);
		}

		attackbutton.onClick.RemoveAllListeners();
		matebutton.onClick.RemoveAllListeners();
		FFbutton.onClick.RemoveAllListeners();

		enemyImage.sprite = UnityUIManager.GetSpriteWithFilename(enemySpriteDictionary, enemy.Image);
		enemyNameText.text = enemyKey;
		
		for (int i = 0; i < 3; i++)
		{
			try
			{
				string[] values = game.enemyBars[enemyNum][i].Split('|');
				if (values.Length != 3)
				{
					Logger.LogError($"Enemy {enemyNum} has invalid bar string: {game.enemyBars[enemyNum][i]}");
				}
				else
				{
					switch (i)
					{
						case 0: SetBar(fiercebar, fiercebarImage, enemyFiercenessText, values); break;
						case 1: SetBar(agilitybar, agilitybarImage, enemyAgilityText, values); break;
						case 2: SetBar(energybar, energybarImage, enemyEnergyText, values); break;
					}
				}
			}
			catch
			{
				Logger.LogError($"Enemy bar data missing for enemy {enemyNum}");
				break;
			}
		}

		attackCommand = $"Attack:{enemyKey}:{game.current_pack_size}";
		string popupCommand = $"Popup:{enemy.URL}";
		string mateCommand = $"Mate:{enemyKey}";

		attackbutton.onClick.AddListener(() => { UIInputSender.instance.SendInput(attackCommand); });
		FFbutton.onClick.AddListener(() => { UIInputSender.instance.SendInput(popupCommand); });
		matebutton.gameObject.SetActive(mateAvailable);
		if (mateAvailable)
		{
			matebutton.onClick.AddListener(() => { UIInputSender.instance.SendInput(mateCommand); });
		}

		this.gameObject.SetActive(true);
	}

	private Sprite GetSpriteWithFilename(Dictionary<string, Sprite> sourceDictionary, string filename)
	{
		string filenameWithoutExtension = filename.Split('.')[0];
		if (sourceDictionary.ContainsKey(filenameWithoutExtension))
		{
			return sourceDictionary[filenameWithoutExtension];
		}
		Logger.LogError("Unknown UI image filename: " + filename + " (" + filenameWithoutExtension + ")");
		return null;
	}

	private void SetBar(RectTransform bar, Image img, Text txt, string[] values)
	{
		Sprite colorSprite =
			values[0] == "green" ? greenSprite
			: values[0] == "red" ? redSprite
			: values[0] == "yellow" ? yellowSprite
			: null;
		img.sprite = colorSprite;

		float width = 0;
		try { float.TryParse(values[1], out width); } catch { Logger.LogError("Failed to parse bar width from value " + values[1]); }
		if (width == -1) width = 12;
		bar.sizeDelta = new Vector2(width, bar.sizeDelta.y);
		
		txt.text = values[2];
	}

	internal void ToggleSuspendedState(bool active)
	{
		attackbutton.gameObject.SetActive(active == false);
		matebutton.gameObject.SetActive(active == false && _game != null && _game.level == 4);
	}

	internal void HideEnemy()
	{
		this.gameObject.SetActive(false);
	}
}
