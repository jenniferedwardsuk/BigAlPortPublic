using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInputSender : MonoBehaviour
{
	[SerializeField] BigAlHTMLToUnityTranslator translator = null;

	[SerializeField] Button NWButton = null;
	[SerializeField] Button NButton = null;
	[SerializeField] Button NEButton = null;
	[SerializeField] Button WButton = null;
	[SerializeField] Button WaitButton = null;
	[SerializeField] Button EButton = null;
	[SerializeField] Button SWButton = null;
	[SerializeField] Button SButton = null;
	[SerializeField] Button SEButton = null;
	[SerializeField] Button PauseButton = null;
	[SerializeField] Button ResumeButton = null;
	[SerializeField] Button ReincarnateButton = null;
	[SerializeField] Button BeginButton = null;
	[SerializeField] Button PopupIndexButton = null;
	[SerializeField] Button CloseButton = null;
	[SerializeField] Button StartLevelButton = null;

	[SerializeField] Button GameOverReincarnateButton = null;
	[SerializeField] Button GameOverBeginButton = null;
	[SerializeField] Button GameOverCloseButton = null;

	[SerializeField] Button CheatRechargeButton = null;
	[SerializeField] Button CheatScoreButton = null;
	[SerializeField] Button CheatLevelButton = null;

	[SerializeField] Button EatEggButton = null;
	[SerializeField] Button AttackDiploButton = null;

	public static UIInputSender instance;
	internal BigAl_pl _game = null;

	// Start is called before the first frame update
	void Start()
	{
		instance = this;

		NWButton.onClick.AddListener(() => { SendInput("Move:NW"); });
		NButton.onClick.AddListener(() => { SendInput("Move:N"); });
		NEButton.onClick.AddListener(() => { SendInput("Move:NE"); });
		WButton.onClick.AddListener(() => { SendInput("Move:W"); });
		WaitButton.onClick.AddListener(() => { SendInput("Wait"); });
		EButton.onClick.AddListener(() => { SendInput("Move:E"); });
		SWButton.onClick.AddListener(() => { SendInput("Move:SW"); });
		SButton.onClick.AddListener(() => { SendInput("Move:S"); });
		SEButton.onClick.AddListener(() => { SendInput("Move:SE"); });
		PauseButton.onClick.AddListener(() => { SendInput("PAUSE"); });
		ResumeButton.onClick.AddListener(() => { SendInput("Resume"); });
		ReincarnateButton.onClick.AddListener(() => { SendInput("'REINCARNATE'"); });
		BeginButton.onClick.AddListener(() => { SendInput("BEGIN"); });
		PopupIndexButton.onClick.AddListener(() => { SendInput("Popup:index.shtml"); });
		CloseButton.onClick.AddListener(() => { SendInput("BEGIN"); });
		StartLevelButton.onClick.AddListener(() => { SendInput("StartLevel"); });

		GameOverReincarnateButton.onClick.AddListener(() => { SendInput("'REINCARNATE'"); });
		GameOverBeginButton.onClick.AddListener(() => { SendInput("BEGIN"); });
		GameOverCloseButton.onClick.AddListener(() => { SendInput("BEGIN"); });

		EatEggButton.onClick.AddListener(() => { SendInput("EatEgg"); });
		AttackDiploButton.onClick.AddListener(() => { SendInput("Attack:Diplodocus:{game.current_pack_size}"); });

		CheatRechargeButton.onClick.AddListener(() => { SendInput("Cheat:Recharge"); });
		CheatScoreButton.onClick.AddListener(() => { SendInput("Cheat:Score"); });
		CheatLevelButton.onClick.AddListener(() => { SendInput("Cheat:Level:10"); });
	}

	private float keyCooldown = 0.25f;
	private float currentKeyCooldown = 0f;
	private void Update()
	{
		if (currentKeyCooldown > 0)
		{
			currentKeyCooldown -= Time.deltaTime;
		}
		else
		{
			HandleKeyboardInput();
		}
	}

	private void HandleKeyboardInput()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			SendInput("PAUSE");
		}
		else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Keypad5))
		{
			SendInput("Wait");
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			SendInput("Move:N");
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			SendInput("Move:S");
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			SendInput("Move:W");
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			SendInput("Move:E");
		}
		else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Keypad7))
		{
			SendInput("Move:NW");
		}
		else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Keypad9))
		{
			SendInput("Move:NE");
		}
		else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			SendInput("Move:SW");
		}
		else if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			SendInput("Move:SE");
		}
	}

	internal void SetCxnSpeedParam(int chosenToggle)
	{
		translator.cxn_speed(chosenToggle);
	}

	internal void SetNumberParam(string matingQNumber)
	{
		translator.number(matingQNumber);
	}

	internal void SetDiploAttack(bool active, int current_pack_size)
	{
		AttackDiploButton.onClick.RemoveAllListeners();
		if (active)
		{
			AttackDiploButton.onClick.AddListener(() => { SendInput($"Attack:Diplodocus:{current_pack_size}"); });
		}
	}

	internal bool levellingUp = false;
	private List<string> permittedInputsDuringLevelup = new List<string>(){ "StartLevel", "BEGIN", "'REINCARNATE'" };
	private List<string> permittedInputsDuringSuspend = new List<string>() { "Resume", "BEGIN", "'REINCARNATE'", "Popup:" };
	public void SendInput(string value)
	{
		if (_game.levellingUp && !permittedInputsDuringLevelup.Contains(value))
			return;

		if (_game.suspended && permittedInputsDuringSuspend.Find(x => value.StartsWith(x)) == null)
			return;

		currentKeyCooldown = keyCooldown;

		Logger.Log("Sending input: " + value);
		translator.SendActionToGame(value);
	}

	public void SendMatingDecision(string value)
	{
		if (_game.levellingUp && !permittedInputsDuringLevelup.Contains(value))
			return;

		Logger.Log("Sending decision: " + value);
		translator.SendMatingDecisionToGame(value);
	}
}
