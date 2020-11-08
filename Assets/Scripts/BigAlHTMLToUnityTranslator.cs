using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PerlInterface;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class BigAlHTMLToUnityTranslator : MonoBehaviour
{
	const string actionInputString = "<INPUT TYPE=HIDDEN NAME=action VALUE='";
	const string movenumberInputString = "<INPUT TYPE=HIDDEN NAME=move_number VALUE='";
	const string testHtmlpath = @"D:\Unity\Projects\BigAlPort\big al test html\test.html";

	private static BigAl_pl _game = null;

	[SerializeField] UnityUIManager UIManager;
	[SerializeField] UIInputSender inputSender;
	[SerializeField] UnityLevelColourManager colourManager;

	[SerializeField] Toggle decisionYesToggle;
	[SerializeField] Dropdown decisionLevel2Dropdown;
	[SerializeField] Dropdown decisionLevel3Dropdown;

	[SerializeField] bool openHTML;

	public void TranslateHTML(BigAl_pl game, string HTML)
	{
#if UNITY_EDITOR
		Logger.Log(HTML);
		System.IO.File.WriteAllText(testHtmlpath, HTML);
		if (openHTML)
		{
			StartCoroutine(ShowHTMLInBrowser());
		}
#endif

		_game = game;
		inputSender._game = game;

		string HTMLafterActionSetup = HTML.Substring(HTML.IndexOf(actionInputString) + actionInputString.Length);
		int actionLength = HTMLafterActionSetup.IndexOf("'");
		string actionstring = HTML.Substring(HTML.IndexOf(actionInputString) + actionInputString.Length, actionLength);
		
		string HTMLafterMoveNumSetup = HTML.Substring(HTML.IndexOf(movenumberInputString) + movenumberInputString.Length);
		int movenumberLength = HTMLafterMoveNumSetup.IndexOf("'");
		string movenumberstring = HTML.Substring(HTML.IndexOf(movenumberInputString) + movenumberInputString.Length, movenumberLength);
		int movenumberint;
		int.TryParse(movenumberstring, out movenumberint);

		document = new JsDocument() { forms = new JsForm[] { new JsForm()
		{
			action = actionstring,
			move_number = movenumberint
		} } };

		UpdateUI(game, HTML);
	}

#if UNITY_EDITOR
	private IEnumerator ShowHTMLInBrowser()
	{
		System.Diagnostics.Process.Start(testHtmlpath);

		Assembly assembly = typeof(EditorWindow).Assembly;
		Type type = assembly.GetType("UnityEditor.GameView");
		EditorWindow gameWindow = EditorWindow.GetWindow(type);
		for (int i = 0; i < 10; i++) //keep focus on unity while browser is opening
		{
			gameWindow.Focus();
			yield return null;
		}
	}
#endif

	public void UpdateUI(BigAl_pl game, string HTML)
	{
		if (game.popupURL != null)
		{
			popwin_ff(game.popupURL, "_blank");
		}

		colourManager.SetColours(game.level);

		UIManager.SetLevelUI(game.levellingUp, game, HTML);
		UIManager.SetLocationImage(game, HTML);
		UIManager.SetLocationDescription(game, HTML);
		UIManager.SetMapVisibility(game, HTML);
		UIManager.SetHintText(game, HTML);
		UIManager.SetMapKey(HTML);
		UIManager.SetAlStats(game, HTML);
		UIManager.SetEnemies(game, HTML);
		UIManager.ToggleSuspendedState(game.suspended);

		UpdateTimer(game.realtime, game.realtime > 0 ? game.timeExpiredAction : "");
	}
	
	private void UpdateTimer(int realtime, string timeExpiredAction)
	{
		StopAllCoroutines();
		if (realtime > 0)
		{
			StartCoroutine(RunTimer(realtime, timeExpiredAction));
		}
	}

	private IEnumerator RunTimer(float realtime, string timeExpiredAction)
	{
		while (realtime > 0)
		{
			realtime -= 1;
			yield return new WaitForSeconds(1);
		}
		yield return new WaitForSeconds(realtime);

		Logger.Log("Timer ended: " + timeExpiredAction);
		do_action(timeExpiredAction);
	}

	public void SendActionToGame(string val)
	{
		Logger.Log("SendActionToGame: " + val);
		do_action(val);
	}

	public void SendMatingDecisionToGame(string val)
	{
		Logger.Log("SendDecisionToGame: " + val);
		decision(val);
	}

	private JsDocument document;
	private JsPopup window = new JsPopup();
	//private JsForm form = null;

	private class JsForm
	{
		internal string action { get { return PerlWebForm.action; } set { PerlWebForm.action = value; } }
		internal int move_number { get { return PerlWebForm.move_number; } set { PerlWebForm.move_number = value; } }

		internal bool BEGIN { get { return PerlWebForm.BEGIN; } set { PerlWebForm.BEGIN = value; } }
		internal string decision { get { return PerlWebForm.decision; } set { PerlWebForm.decision = value; } } //level questions
		internal string cxn_speed { get { return PerlWebForm.cxn_speed; } set { PerlWebForm.cxn_speed = value; } }
		internal int number { get { return PerlWebForm.number; } set { PerlWebForm.number = value; } } //chosen mating question answer

		//debug variables
		internal string level_data { get { return PerlWebForm.level_data; } set { PerlWebForm.level_data = value; } }
		internal int level { get { return PerlWebForm.level; } set { PerlWebForm.level = value; } }
		internal double weight { get { return PerlWebForm.weight; } set { PerlWebForm.weight = value; } }

		internal void submit()
		{
			Logger.LogWarning("JsForm submit ............................................................................................");
			if (_game != null)
			{
				_game.main();
			}
			else
			{
				Logger.LogError("no _game yet");
			}
		}
	}

	private class JsDocument
	{
		public Dictionary<string, JsImage> images = null;
		public List<JsImage> imgarray;
		public JsForm[] forms;
	}

	private class JsPopup
	{
		internal string name;

		internal void open(string url, string name, string propertiesData)
		{
			ScreenManager.instance.ShowPopup(url, name, propertiesData);
		}
	}

	private class JsImage
	{
		public string src;
	}

	#region js functions
	void popwin_ff(string url, string name)
	{
		window.open(url, name, "status=no,scrollbars=yes,resizable=yes,width=500,height=400");
		window.name = "dino";
	}

	void decision(string val)
	{
		document.forms[0].decision = val;
		document.forms[0].action = "Mate:Female Allosaurus";
		document.forms[0].submit();
	}

	internal void cxn_speed(int radioChoice)
	{
		switch (radioChoice)
		{
			case 0:
				PerlWebForm.cxn_speed = ""; //fast
				break;
			case 1:
				PerlWebForm.cxn_speed = "M"; //average
				break;
			case 2:
				PerlWebForm.cxn_speed = "S"; //slow
				break;
			default:
				break;
		}
	}

	internal void number(string matingQNumber)
	{
		int.TryParse(matingQNumber, out int qNumber);
		document.forms[0].number = qNumber;
	}

	string[] level2decisionValues = new string[] { "0.2", "0.5", "1", "2", "5" };
	void do_action(string val)
	{
		var f = document.forms[0];
		var current = f.action;

		f.BEGIN = val == "BEGIN";

		if (val == "StartLevel" || _game.level == 4)
		{
			string decision = "";
			switch (_game.level)
			{
				case 1:
					decision = decisionYesToggle.isOn ? "Yes" : "No";
					break;
				case 2:
					decision = level2decisionValues[decisionLevel2Dropdown.value];
					break;
				case 3:
					decision = decisionLevel3Dropdown.options[decisionLevel3Dropdown.value].text;
					break;
				case 4:
					break;
			}
			f.decision = decision;
		}

		//Check if the action has lower precidence than the current one
		if (
			current.IndexOf("BeAttacked") == -1 ||
			(val.IndexOf("Wait") == -1 && val.IndexOf("Eat") == -1 && val.IndexOf("Attack") == -1)
		  )
		{
			//Do the action
			f.action = val;
		}
		f.submit();
	}

	void switch_img(string img, string src)
	{
		document.images[img].src = src;
	}

	// Args: img,src, img,src, ...
	void switch_imgs(string[] arguments)
	{
		var args = arguments;
		for (int i = 0; i < args.Length; i += 2)
		{
			var img = args[i];
			var src = args[i + 1];
			document.images[img].src = src;
		}
	}

	// Args: src, src, ...
	void preload_images(string[] arguments)
	{
		if (document.images != null)
		{
			if (document.imgarray == null) document.imgarray = new List<JsImage>();
			var len = document.imgarray.Count;
			var args = arguments;
			for (int i = 0; i < args.Length; i++)
			{
				document.imgarray[len] = new JsImage();
				document.imgarray[len].src = args[i];
				len++;
			}
		}
	}
	#endregion
}
