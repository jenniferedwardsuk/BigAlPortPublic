using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class UnityUIManager : MonoBehaviour
{
	[System.Serializable]
	private class SpriteWithFilename
	{
		public string filename => sprite.name;
		public Sprite sprite;
	}

	[System.Serializable]
	private class Compass
	{
		[SerializeField] Button Nbutton;
		[SerializeField] Button NEbutton;
		[SerializeField] Button Ebutton;
		[SerializeField] Button SEbutton;
		[SerializeField] Button Sbutton;
		[SerializeField] Button SWbutton;
		[SerializeField] Button Wbutton;
		[SerializeField] Button NWbutton;
		[SerializeField] Button waitbutton;
	}

	[Header("Sprites")]
	[SerializeField] List<Sprite> locationSprites;
	[SerializeField] List<Sprite> alWeightSprites;
	[SerializeField] List<Sprite> alFilledSprites;
	private Dictionary<string, Sprite> locationSpriteDictionary;
	private Dictionary<string, Sprite> alWeightSpriteDictionary;
	private Dictionary<string, Sprite> alFilledSpriteDictionary;

	[Header("Right bar")]
	[SerializeField] Text labellevel;
	[SerializeField] Text labelscore;
	[SerializeField] Text labelweight;
	[SerializeField] Image picturealweight;
	[SerializeField] Text labelenergy;
	[SerializeField] Image picturealenergy;
	[SerializeField] Text labelfitness;
	[SerializeField] Image picturealfitness;

	[Header("Description")]
	[SerializeField] Image locationImage;
	[SerializeField] Text labeldesc;
	[SerializeField] Button eatEggButton;
	[SerializeField] Button attackDiploButton;
	[SerializeField] MatingUIController matingUIController;

	[Header("Compass")]
	[SerializeField] Compass compass;
	[SerializeField] Text labelHint;
	[SerializeField] Image mapKeyImage;

	[Header("Bottom bar")]
	[SerializeField] Button buttonpause;
	[SerializeField] Button buttonRestartLevel;
	[SerializeField] Button buttonRestartGame;
	[SerializeField] Button buttonFactFiles;
	[SerializeField] Button buttonQuit;

	[Header("Enemies")]
	[SerializeField] List<EnemyUIController> enemyUIs = new List<EnemyUIController>();

	[Header("Level up")]
	[SerializeField] GameObject levelChoiceGroup;
	[SerializeField] GameObject levelRadioGroup;
	[SerializeField] GameObject level2DropdownGroup;
	[SerializeField] GameObject level3DropdownGroup;
	[SerializeField] Text levelText;
	[SerializeField] Button buttonPlayGame;

	[Header("Map")]
	[SerializeField] List<MapSquareController> mapSquares = new List<MapSquareController>();
	[SerializeField] List<Sprite> mapSprites = new List<Sprite>();
	[SerializeField] Sprite mapSpriteBlank;

	[Header("Suspended state")]
	[SerializeField] GameObject compassGroup;
	[SerializeField] GameObject suspendedTextGroup;

	void Start()
    {
		locationSpriteDictionary = locationSprites.ToDictionary(x => x.name);
		alWeightSpriteDictionary = alWeightSprites.ToDictionary(x => x.name);
		alFilledSpriteDictionary = alFilledSprites.ToDictionary(x => x.name);
	}

	internal void ToggleSuspendedState(bool active)
	{
		compassGroup.SetActive(active == false);
		labeldesc.text = active ? "" : labeldesc.text;
		suspendedTextGroup.SetActive(active);
		foreach (var enemy in enemyUIs)
		{
			enemy.ToggleSuspendedState(active);
		}
	}

	internal void SetLocationImage(BigAl_pl game, string hTML)
	{
		locationImage.sprite = game.currentLocationImage == null ? null : GetSpriteWithFilename(locationSpriteDictionary, game.currentLocationImage);
	}

	internal static Sprite GetSpriteWithFilename(Dictionary<string, Sprite> sourceDictionary, string filename)
	{
		string filenameWithoutExtension = filename.Split('.')[0];
		if (sourceDictionary.ContainsKey(filenameWithoutExtension))
		{
			return sourceDictionary[filenameWithoutExtension];
		}
		foreach (var key in sourceDictionary.Keys) //fallback - e.g. forest1.jpg has varying capitalisation in the data
		{
			if (key.ToUpper() == filenameWithoutExtension.ToUpper())
			{
				return sourceDictionary[key];
			}
		}
		Logger.LogError("Unknown UI image filename: " + filename + " (" + filenameWithoutExtension + ")");
		return null;
	}

	internal void SetLocationDescription(BigAl_pl game, string hTML)
	{
		string desc = game.currentDescription;

		desc = desc.Replace(@"<FONT FACE='Arial,Helvetica,Helv' SIZE=2 style=""font-size: 8pt;"">", "");
		desc = desc.Replace(@"</FONT>", "");
		desc = AllowForEatEgg(game, desc);
		desc = AllowForDiploAttack(game, desc);

		const string matingDecisionIndicator = "document.forms[0].action.value = 'Mate:Female Allosaurus';";
		const string matingMovesIndicator = "move in front and show off eye crests";
		const string matingQuestionIndicator = "The female turns round and asks you a question";
		matingUIController.ToggleMatingDecisionUI(hTML.Contains(matingDecisionIndicator), hTML.Contains(matingMovesIndicator), hTML.Contains(matingQuestionIndicator));
		desc = matingUIController.ReplaceUnsupportedHTML(game, desc);

		labeldesc.text = desc;
	}

	private string AllowForDiploAttack(BigAl_pl game, string desc)
	{
		string attackDiploButtonHtml = $"<A OnMouseOver='self.status=\"Attack the Diplodocus\"; return true;' HREF='javascript:do_action(\"Attack:Diplodocus:{game.current_pack_size}\");'><IMG BORDER=0 ALIGN='BASELINE' SRC='images/level_3/3_m_attack.gif' ALT='Attack the Diplodocus' WIDTH='43' HEIGHT='18'></A>";

		bool available = desc.Contains(attackDiploButtonHtml);
		attackDiploButton.gameObject.SetActive(available);
		UIInputSender.instance.SetDiploAttack(available, game.current_pack_size);
		desc = desc.Replace(attackDiploButtonHtml, "");

		return desc;
	}

	private string AllowForEatEgg(BigAl_pl game, string desc)
	{
		int level = game.level > 0 ? game.level : 1;
		string eatEggsButtonHtml = $"<A OnMouseOver='self.status=\"Eat one of the eggs\"; return true;' HREF='javascript:do_action(\"EatEgg\");'><IMG BORDER=0 ALIGN='BASELINE' SRC='images/level_{level}/{level}_eategg.gif' ALT='Eat one of the eggs' WIDTH='' HEIGHT='18'></A>";
		string eatEggButtonHtml = $"<A OnMouseOver='self.status=\"Eat the egg\"; return true;' HREF='javascript:do_action(\"EatEgg\");'><IMG BORDER=0 ALIGN='BASELINE' SRC='images/level_{level}/{level}_eategg.gif' ALT='Eat the egg' WIDTH='' HEIGHT='18'></A>";

		if (desc.Contains(eatEggsButtonHtml) || desc.Contains(eatEggButtonHtml))
		{
			desc = desc.Replace(eatEggButtonHtml, "");
			desc = desc.Replace(eatEggsButtonHtml, "");
			eatEggButton.gameObject.SetActive(true);
		}
		else
		{
			eatEggButton.gameObject.SetActive(false);
		}

		return desc;
	}

	internal void SetLevelUI(bool active, BigAl_pl game, string hTML)
	{
		levelChoiceGroup.SetActive(active);
		levelRadioGroup.SetActive(false);
		level2DropdownGroup.SetActive(false);
		level3DropdownGroup.SetActive(false);
		if (active)
		{
			string levelString = "";
			switch (game.level)
			{
				case 1:
					levelString = FileContents.level1HtmlNoLinks;
					levelRadioGroup.SetActive(true);
					level2DropdownGroup.SetActive(false);
					level3DropdownGroup.SetActive(false);
					break;
				case 2:
					levelString = FileContents.level2HtmlNoLinks;
					levelRadioGroup.SetActive(false);
					level2DropdownGroup.SetActive(true);
					level3DropdownGroup.SetActive(false);
					break;
				case 3:
					levelString = FileContents.level3HtmlNoLinks;
					levelRadioGroup.SetActive(false);
					level2DropdownGroup.SetActive(false);
					level3DropdownGroup.SetActive(true);
					break;
				case 4:
					levelString = FileContents.level4HtmlNoLinks;
					levelRadioGroup.SetActive(false);
					level2DropdownGroup.SetActive(false);
					level3DropdownGroup.SetActive(false);
					break;
				default:
					Logger.LogError($"Unknown level up {game.level}");
					break;
			}

			//remove unsupported html
			levelString = levelString.Replace("<P>", "");
			levelString = levelString.Replace(@"<INPUT name=decision value=Yes type=radio>Yes &nbsp; <INPUT CHECKED name=decision value=No type=radio>No", "");
			levelString = levelString.Replace(@"&nbsp;
<SELECT NAME = ""decision"" >
<OPTION VALUE=0.2> Very Slowly
 </OPTION> <OPTION VALUE = 0.5 > Quite Slowly
 </OPTION> <OPTION VALUE = 1 > Medium </OPTION >
 <OPTION VALUE= 2 > Quite Fast</OPTION>
 <OPTION VALUE = 5 > Very Fast</OPTION>
 </SELECT>", "");
			levelString = levelString.Replace(@"<SELECT NAME = 'decision' >
<OPTION> 1 </OPTION>
<OPTION> 2 </OPTION>
<OPTION> 3 </OPTION>
<OPTION> 4 </OPTION>
<OPTION> 6 </OPTION>
<OPTION> 8 </OPTION>
</SELECT>", "");
			levelString = levelString.Replace("</P>    ", "");

			levelText.text = levelString;
		}
	}

	internal void SetMapVisibility(BigAl_pl game, string hTML)
	{
		ResetMap(game, hTML);
		for (int i = 0; i < game.visited.Length; i++)
		{
			for (int j = 0; j < game.visited[0].Length; j++)
			{
				if (game.visited[i][j] > 0)
				{
					int combinedIndex = i * game.visited[0].Length + j;
					int spriteIndex = j * game.visited.Length + i;
					mapSquares[combinedIndex].SetSprite(mapSprites[spriteIndex]);
					mapSquares[combinedIndex].SetAl(false);
					mapSquares[combinedIndex].SetMum(false);
				}
			}
		}
		int alIndex = game.y * game.visited[0].Length + game.x;
		mapSquares[alIndex].SetAl(true);
		if (game.have_mother)
		{
			int mumIndex = game.my * game.visited[0].Length + game.mx;
			mapSquares[mumIndex].SetMum(true);
		}
	}

	internal void ResetMap(BigAl_pl game, string hTML)
	{
		for (int j = 0; j < mapSquares.Count; j++)
		{
			mapSquares[j].SetSprite(mapSpriteBlank);
			mapSquares[j].SetAl(false);
			mapSquares[j].SetMum(false);
		}
	}

	internal void SetMapKey(string hTML)
	{
		bool showKey = false;
		if (hTML.Contains("<!--### Key inc starts ###-->"))
		{
			showKey = true;
		}
		mapKeyImage.gameObject.SetActive(showKey);
		labelHint.gameObject.SetActive(!showKey);
	}

	internal void SetHintText(BigAl_pl game, string hTML)
	{
		labelHint.text = game.currentHint;
	}

	internal void SetAlStats(BigAl_pl game, string hTML)
	{
		labellevel.text = "Level: " + game.level;
		labelscore.text = "Score: " + game.score;

		labelweight.text = "Weight: " + Math.Round(game.weight, 2) + "kg";
		picturealweight.sprite = GetSpriteWithFilename(alWeightSpriteDictionary, game.currentWeightSprite);

		labelenergy.text = "Energy: " + Math.Floor(game.energy) + "%";
		picturealenergy.sprite = GetSpriteWithFilename(alFilledSpriteDictionary, game.currentEnergySprite);

		labelfitness.text = "Fitness: " + Math.Floor(game.fitness) + "%";
		picturealfitness.sprite = GetSpriteWithFilename(alFilledSpriteDictionary, game.currentFitnessSprite);
	}

	internal void SetEnemies(BigAl_pl game, string hTML)
	{
		int index = 0;

		foreach (var enemy in game.species_present)
		{
			enemyUIs[index].SetEnemy(game, enemy.Key, BigAl_pl.species[enemy.Key], game.level == 4);
			index++;
		}

		for (int i = index; i < 4; i++)
		{
			enemyUIs[i].HideEnemy();
		}
	}
}
