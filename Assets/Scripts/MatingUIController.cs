using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MatingUIController : MonoBehaviour
{
	[SerializeField] GameObject decisionGroupObject;
	[SerializeField] GameObject movesTextObject;
	[SerializeField] GameObject callsTextObject;
	[SerializeField] Text questionText;
	[SerializeField] Text bulletsText;

	[SerializeField] Button decision1Button;
	[SerializeField] Button decision2Button;
	[SerializeField] Button decision3Button;
	[SerializeField] Button decision4Button;
	[SerializeField] Button decision5Button;

	public void ToggleMatingDecisionUI(bool active, bool isMoves, bool isQuestion)
	{
		decisionGroupObject.SetActive(active);
		if (!active)
			return;
		
		movesTextObject.SetActive(isMoves);
		callsTextObject.SetActive(isMoves == false && isQuestion == false);
		questionText.gameObject.SetActive(isQuestion);

		SetupButton(decision1Button, isMoves ? "Display" : "Rumble");
		SetupButton(decision2Button, isMoves ? "Circle" : "Squeak");
		SetupButton(decision3Button, isMoves ? "Side" : "Wait");
		SetupButton(decision4Button, isMoves ? "Wait" : "Mount");
		SetupButton(decision5Button, isMoves ? "Mount" : null);
	}

	private void SetupButton(Button decisionButton, string command)
	{
		decisionButton.enabled = command != null;
		decisionButton.onClick.RemoveAllListeners();
		decisionButton.onClick.AddListener(() => { UIInputSender.instance.SendMatingDecision(command); });
	}

	internal string ReplaceUnsupportedHTML(BigAl_pl game, string desc)
	{
		string reaction = FemaleReactionInDesc(desc);
		if (!string.IsNullOrEmpty(reaction))
		{
			desc = reaction + " What now?";
		}

		if (desc.Contains("The female turns round and asks you a question"))
		{
			int qStartIndex = desc.IndexOf("<INPUT TYPE = HIDDEN NAME=decision>") + "<INPUT TYPE = HIDDEN NAME=decision>".Length;
			int qEndIndex = desc.IndexOf("<LI><A HREF=");

			desc = "The female turns round and asks you a question:" + "\n" + game.matingQuestion;
			SetMatingQuestionAnswers(game);
		}
		else
		{
			desc = desc.Replace("<P>", "");
		}

		return desc;
	}

	private string[] femaleReactions = new string[]
	{
		"The female leans forward and lifts her tail.",
		"The female eyes you cautiously.",
		"The female moves in for a closer look.",
		"The female starts to lose interest.",
		"The female returns your display.",
		"The female starts to lose interest."
	};

	private string FemaleReactionInDesc(string desc)
	{
		string reaction = null;
		foreach (var reac in femaleReactions)
		{
			if (desc.Contains(reac))
			{
				reaction = reac;
			}
		}
		return reaction;
	}

	private void SetMatingQuestionAnswers(BigAl_pl game)
	{
		string[] answers = game.matingAnswers;
		UIInputSender.instance.SetNumberParam(game.matingQNumber);
		Button[] buttons = new Button[] { decision1Button, decision2Button, decision3Button, decision4Button, decision5Button };
		string[] answerCommands = new string[] { "A", "B", "C", "D", "E" };
		string questionString = "";
		bulletsText.text = "";
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].enabled = false;
		}
		for (int i = 0; i < answers.Length; i++)
		{
			buttons[i].enabled = true;
			SetupButton(buttons[i], answers.Length > i ? answerCommands[i] : null);
			questionString += "<color=white>•</color> " + answers[i] + "\n";
		}
		questionText.text = questionString;
	}
}
