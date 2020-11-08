using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSquareController : MonoBehaviour
{
	[SerializeField] Image mapImage;
	[SerializeField] Image alImage;
	[SerializeField] Image mumImage;

	internal void SetSprite(Sprite newImage)
	{
		mapImage.sprite = newImage;
	}

	internal void SetAl(bool present)
	{
		alImage.enabled = present;
	}

	internal void SetMum(bool present)
	{
		mumImage.enabled = present;
	}
}
