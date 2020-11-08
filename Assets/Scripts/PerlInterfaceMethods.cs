using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public partial class PerlInterface
{
	internal static PerlCookie[] perlCookie = new PerlCookie[0];
	internal static string cookie(string val)
	{
		foreach (var cookie in perlCookie)
		{
			if (cookie.name == val)
			{
				return cookie.GetString();
			}
		}
		return null;
	}
	
	internal static string paramString(string val)
	{
		if (val == "'action'")
		{
			return PerlWebForm.action;
		}
		else if (val == "'cxn_speed'")
		{
			return PerlWebForm.cxn_speed;
		}
		else if (val == "'level_data'")
		{
			return PerlWebForm.level_data;
		}
		else if (val == "'decision'")
		{
			return PerlWebForm.decision;
		}
		
		return "";
	}

	internal static bool paramBool(string val)
	{
		if (val == "BEGIN")
		{
			return PerlWebForm.BEGIN;
		}
		
		return default;
	}

	internal static int paramInt(string val)
	{
		if (val == "\"move_number\"")
		{
			return PerlWebForm.move_number;
		}
		else if (val == "'level'")
		{
			return PerlWebForm.level;
		}
		else if (val == "'number'")
		{
			return PerlWebForm.number;
		}

		return default;
	}

	internal static double paramDouble(string val)
	{
		if (val == "'weight'")
		{
			return PerlWebForm.weight;
		}
		
		return default;
	}

	internal static MatchCollection GetRegexMatches(string str, string pattern, bool ignoreCase = false)
	{
		var options = RegexOptions.Compiled;
		if (ignoreCase)
		{
			options = options | RegexOptions.IgnoreCase;
		}

		Regex rx = new Regex(pattern, options);

		MatchCollection matches = rx.Matches(str);

		return matches;
	}

	internal static bool perlRegex(string str, string pattern, bool ignoreCase = false)
	{
		if (str == null)
		{
			Logger.Log("WARNING: null str in perlRegex");
			return false;
		}

		MatchCollection matches = GetRegexMatches(str, pattern, ignoreCase);

		return matches.Count > 0;
	}

	internal static string perlSubstitute(string parentstr, string findsubstr, string newsubstr)
	{
		return parentstr.Replace(findsubstr, newsubstr);
	}

	internal static string[] perlSubstituteReturningMatches(string str, string pattern1, string pattern2)
	{
		var firstMatches = GetRegexMatches(str, pattern1);
		var secondMatches = GetRegexMatches(str, pattern2);

		string firstMatchvalue = firstMatches.Count > 0 ? firstMatches[0].Value : "";
		string secondMatchvalue = secondMatches.Count > 0 ? secondMatches[0].Value : "";

		if (firstMatchvalue == "" || secondMatchvalue == "")
		{
			Logger.Log("WARNING: no match in perlSubstituteReturningMatches");
		}

		string[] results = new string[] { firstMatchvalue, secondMatchvalue };

		return results;
	}

	internal static string[] perlRegexReturningMatches(string str, string pattern, bool ignoreCase = false)
	{
		var matches = GetRegexMatches(str, pattern, ignoreCase);

		List<string> matchvalues = new List<string>();
		for (int i = 0; i < matches.Count; i++)
		{
			matchvalues.Add(matches[i].Value);
		}

		if (matchvalues.Count == 0)
		{
			Logger.Log("WARNING: no match in perlRegexReturningMatches");
		}

		string[] results = matchvalues.ToArray();

		return results;
	}

	internal static string perlSubstituteGlobal(ref string str, string findsubstr, string newsubstr)
	{
		str = str.Replace(findsubstr, newsubstr);
		return str;
	}
	
	internal static double rand()
	{
		double num = UnityEngine.Random.Range(0.0f, 0.9999f);
		return num;
	}
	
	internal static void srand(int seed)
	{
		//UnityEngine.Random.InitState(seed);
	}

	internal static int scalar(string[] countable)
	{
		return countable.Length;
	}

	internal static int scalar(char[][] countable)
	{
		return countable.Length;
	}

	internal static int scalar(Dictionary<string, string>.KeyCollection countable)
	{
		return countable.Count;
	}

	internal static int scalar(List<string> countable)
	{
		return countable.Count;
	}

	internal static double abs(double v)
	{
		return Math.Abs(v);
	}

	internal static string sprintf(string str, double value)
	{
		if (str.Contains("%.3f"))
		{
			return str.Replace("%.3f", value.ToString("F3"));
		}
		else if (str.Contains("%.0f"))
		{
			return str.Replace("%.0f", value.ToString("F0"));
		}
		else if (str.Contains("%.0d%%"))
		{
			return str.Replace("%.0d%%", value.ToString("F0") + "%");
		}
		else if (str.Contains("%.2f"))
		{
			return str.Replace("%.2f", value.ToString());
		}
		Logger.LogError($"Unknown sprintf {str} on {value} = " + string.Format(str, value));
		return string.Format(str, value);
	}

	internal static string[] split(char[] delimiter, string action)
	{
		if (action == null)
		{
			Logger.Log("WARNING: null action str in split");
			return new string[] { action };
		}
		return action.Split(delimiter);
	}

	internal static string[] split(string delimiter, string action)
	{
		return action.Split(delimiter.ToCharArray());
	}

	internal static string join(string separator, string[] hintref)
	{
		return string.Join(separator, hintref);
	}

	internal static double log(double p)
	{
		return Math.Log(p);
	}

	internal static char chr(int v) //Returns the character represented by that NUMBER in the character set.
	{
		return (char)v;
	}

	internal static int ord(char v) //Returns the numeric value of the first character of EXPR. If EXPR is an empty string, returns 0.
	{
		return v;
	}

	internal static char uc(string v)
	{
		return v.ToUpper().ToCharArray()[0];
	}

	internal static void shift(ref string[] refList)
	{
		string[] originalList = new string[refList.Length];
		Array.Copy(refList, originalList, refList.Length);

		refList = new string[originalList.Length - 1];
		for (int i = 1; i < originalList.Length; i++)
		{
			refList[i - 1] = originalList[i];
		}
	}

	internal static string substr(string str, int start, int length)
	{
		return str.Substring(start, length);
	}

	static string printMessage = "";
	internal static void pprint(string message)
	{
		//Logger.Log(printMessage);
		printMessage += message;
	}

	internal static void pprintError(string message)
	{
		Logger.LogWarning(message);
	}

	internal static void perlException(string message)
	{
		Logger.LogError(message);
		throw new Exception(message);
	}

	internal static ScreenManager screenManager;
	internal static BigAlHTMLToUnityTranslator htmlTranslator;
	internal static void exit(int v, BigAl_pl game)
	{
		if (game.gameover)
		{
			screenManager.OnGameOver(game.gameoverDesc);
		}
		htmlTranslator.TranslateHTML(game, printMessage);
		printMessage = "";
		Logger.Log("EXIT");
		throw new Exception("exit"); //to force-quit execution
	}

	internal static bool open(out localFile lI, string filepath)
	{
		string[] fileText = new string[0];
		if (GetFileContent(filepath, ref fileText))
		{
			lI = new localFile(fileText);
			return true;
		}
		else
		{
			Logger.LogError("Unknown file content " + filepath);
			lI = null;
			return false;
		}
	}

	private static bool GetFileContent(string filepath, ref string[] fileContents)
	{
		if (filepath == "/templates/normal.html")
		{
			fileContents = FileContents.normalHtml.Split('\n');
			return true;
		}
		else if (filepath == /*"/*/"javascript.js")
		{
			fileContents = FileContents.javascriptJs.Split('\n');
			return true;
		}
		else if (filepath == /*"/*/"map_images.txt")
		{
			fileContents = FileContents.mapImagesTxt.Split('\n');
			return true;
		}
		else if (filepath == /*"/*/"map_descriptions.txt")
		{
			fileContents = FileContents.mapDescriptionsTxt.Split('\n');
			return true;
		}
		else if (filepath == "/level_1.html")
		{
			fileContents = FileContents.level1Html.Split('\n');
			return true;
		}
		else if (filepath == "/level_2.html")
		{
			fileContents = FileContents.level2Html.Split('\n');
			return true;
		}
		else if (filepath == "/level_3.html")
		{
			fileContents = FileContents.level3Html.Split('\n');
			return true;
		}
		else if (filepath == "/level_4.html")
		{
			fileContents = FileContents.level4Html.Split('\n');
			return true;
		}
		else if (filepath == "/templates/gameover.html")
		{
			fileContents = FileContents.gameoverHtml.Split('\n');
			return true;
		}
		else if (filepath == "/templates/quit.html")
		{
			fileContents = FileContents.quitHtml.Split('\n');
			return true;
		}
		else if (filepath == "/templates/scores.html")
		{
			fileContents = FileContents.scoresHtml.Split('\n');
			return true;
		}
		else if (filepath == "/map_key.ssi")
		{
			fileContents = FileContents.mapKeySsi.Split('\n');
			return true;
		}
		else if (filepath == "/mating.ssi")
		{
			fileContents = FileContents.matingSsi.Split('\n');
			return true;
		}
		else if (filepath == "/mating_calls.ssi")
		{
			fileContents = FileContents.matingCallsSsi.Split('\n');
			return true;
		}
		else if (filepath == "/mating_moves.ssi")
		{
			fileContents = FileContents.matingMovesSsi.Split('\n');
			return true;
		}
		else if (filepath == "mating_questions.txt")
		{
			fileContents = FileContents.matingQuestionsTxt.Split('\n');
			return true;
		}
		return false;
	}

	internal static void close(localFile lI)
	{

	}

	internal static void chomp(string str)
	{

	}
}
