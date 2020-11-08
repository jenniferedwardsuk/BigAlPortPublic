using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityLevelColourManager : MonoBehaviour
{
	int currentLevel;

	[SerializeField] Image descriptionLabel;
	[SerializeField] List<Sprite> descriptionLabelSprites;

	[SerializeField] Image eateggButton;
	[SerializeField] List<Sprite> eateggButtonSprites;

	[SerializeField] Image factfileButton;
	[SerializeField] List<Sprite> factfileButtonSprites;

	[SerializeField] List<Image> enemyAttackButtons;
	[SerializeField] List<Sprite> enemyAttackButtonSprites;

	[SerializeField] List<Image> enemyFactFileButtons;
	[SerializeField] List<Sprite> enemyFactFileButtonSprites;

	[SerializeField] List<Image> enemyMateButtons;
	[SerializeField] List<Sprite> enemyMateButtonSprites;

	[SerializeField] Image menuLabel;
	[SerializeField] List<Sprite> menuLabelSprites;

	[SerializeField] Image movementLabel;
	[SerializeField] List<Sprite> movementLabelSprites;

	[SerializeField] Image pauseButton;
	[SerializeField] List<Sprite> pauseButtonSprites;

	[SerializeField] Image playLevelButton;
	[SerializeField] List<Sprite> playLevelButtonSprites;

	[SerializeField] Image quitButton;
	[SerializeField] List<Sprite> quitButtonSprites;

	[SerializeField] Image restartGameButton;
	[SerializeField] List<Sprite> restartGameButtonSprites;

	[SerializeField] Image restartLevelButton;
	[SerializeField] List<Sprite> restartLevelButtonSprites;

	[SerializeField] Image resumeButton;
	[SerializeField] List<Sprite> resumeButtonSprites;

	[SerializeField] Image background;
	[SerializeField] List<Sprite> backgroundSprites;

	[SerializeField] List<Image> furniture1Images;
	[SerializeField] List<Sprite> furniture1Sprites;

	[SerializeField] List<Image> furniture2Images;
	[SerializeField] List<Sprite> furniture2Sprites;

	[SerializeField] List<Image> furniture3Images;
	[SerializeField] List<Sprite> furniture3Sprites;

	[SerializeField] List<Image> directColorImages;
	[SerializeField] List<Color> levelColors;

	internal void SetColours(int level)
	{
		if (level == currentLevel)
			return;

		currentLevel = level;
		level = level <= 0 ? 0 : level - 1;

		descriptionLabel.sprite = descriptionLabelSprites[level];
		eateggButton.sprite = eateggButtonSprites[level];
		factfileButton.sprite = factfileButtonSprites[level];

		foreach (var item in enemyAttackButtons)
		{
			item.sprite = enemyAttackButtonSprites[level];
		}

		foreach (var item in enemyFactFileButtons)
		{
			item.sprite = enemyFactFileButtonSprites[level];
		}

		foreach (var item in enemyMateButtons)
		{
			item.sprite = enemyMateButtonSprites[level];
		}

		menuLabel.sprite = menuLabelSprites[level];
		movementLabel.sprite = movementLabelSprites[level];
		pauseButton.sprite = pauseButtonSprites[level];
		playLevelButton.sprite = playLevelButtonSprites[level];
		quitButton.sprite = quitButtonSprites[level];
		restartGameButton.sprite = restartGameButtonSprites[level];
		restartLevelButton.sprite = restartLevelButtonSprites[level];
		resumeButton.sprite = resumeButtonSprites[level];
		background.sprite = backgroundSprites[level];
		
		foreach (var item in furniture1Images)
		{
			item.sprite = furniture1Sprites[level];
		}

		foreach (var item in furniture2Images)
		{
			item.sprite = furniture2Sprites[level];
		}

		foreach (var item in furniture3Images)
		{
			item.sprite = furniture3Sprites[level];
		}

		foreach (var item in directColorImages)
		{
			item.color = levelColors[level];
		}
	}
}
