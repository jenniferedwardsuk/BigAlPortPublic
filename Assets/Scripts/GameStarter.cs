using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
	[SerializeField] BigAlHTMLToUnityTranslator htmlTranslator = null;
	[SerializeField] ScreenManager screenManager = null;
	[SerializeField] GameObject cheatButtons;
	BigAl_pl game;
	[SerializeField] private int DEBUG = 0;

	// Start is called before the first frame update
	void Start()
	{
		game = new BigAl_pl();
		PerlInterface.htmlTranslator = htmlTranslator;
		PerlInterface.screenManager = screenManager;
	}

    // Update is called once per frame
    void Update()
    {

	}

	public void StartGame()
	{
#if UNITY_EDITOR
		game.DEBUG = DEBUG;
		cheatButtons.SetActive(DEBUG != 0);
#else
		game.DEBUG = 0;
		cheatButtons.SetActive(false);
#endif
		game.BEGIN();
	}

	public void PlayLevel()
	{
		game.main();
	}
}
