using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class ENV
{
	internal static string SCRIPT_FILENAME = Application.dataPath;
	internal static string REQUEST_URI = Application.dataPath;
}

public partial class PerlInterface
{
	internal static class PerlWebForm
	{
		internal static BigAl_pl game = null;
		internal static bool BEGIN;
		internal static string action = "";
		internal static string decision = "";
		internal static string level_data = "";
		internal static string cxn_speed = "";
		internal static int number;
		internal static int level;
		internal static int move_number;
		internal static double weight;

		internal static void submit(string paramValue)
		{
			ResetParams();
		}

		internal static void ResetParams()
		{
			BEGIN = false;
			action = "";
			decision = "";
			level_data = "";
			cxn_speed = "";
			number = 0;
			level = 0;
			move_number = 0;
			weight = 0;
		}
	}

	internal class PerlCookie
	{
		internal string name;

		internal string GetString()
		{
			return value.stateString;
		}

		internal CookieValues value;
		internal string expires;
		internal string domain;
		internal string path;
	}

	internal class CookieValues
	{
		//STATE STRING: 
		//1_	move_number.ToString()
		//1_	level.ToString()
		//_		decision
		//_		level_data
		//F_	cxn_speed
		//3_	x.ToString()		//7_	y.ToString()		//3_	mx.ToString()		//7_	my.ToString()
		//6474950_	(score ^ cookie_crypt).ToString()
		//0_	matings.ToString()
		//100_	round_to_sf(fitness,4).ToString()		//0.2_	round_to_sf(weight,4).ToString()		//100_	round_to_sf(energy,4).ToString()
		//333333333333333333333333333333_	vis		//_		spec		//A5:3¼C1:4¼I6:5	eggs

		string move_number;
		string level;
		string decision;
		string level_data;
		string cxn_speed;
		string x; string y; string mx; string my;
		string scorecrypt;
		string matings;
		string fitness; string weight; string energy;
		string vis; string spec; string eggs;
		internal string stateString;

		public CookieValues(string move_number, string level, string decision, string level_data, string cxn_speed, 
			string x, string y, string mx, string my, 
			string scorecrypt, string matings, 
			string fitness, string weight, string energy, 
			string vis, string spec, string eggs,
			string stateString)
		{
			this.move_number = move_number;
			this.level = level;
			this.decision = decision;
			this.level_data = level_data;
			this.cxn_speed = cxn_speed;
			this.x = x;
			this.y = y;
			this.mx = mx;
			this.my = my;
			this.scorecrypt = scorecrypt;
			this.matings = matings;
			this.fitness = fitness;
			this.weight = weight;
			this.energy = energy;
			this.vis = vis;
			this.spec = spec;
			this.eggs = eggs;
			this.stateString = stateString;
		}
	}

	internal class speciesObj
	{
		public string URL;
		public string Image;
		public double Nutrition;
		public double Danger;
		public double Aggression;
		public double Difficulty;
		public string Injury;
		public string[] Enter;
		public string Attack;
		public string HTML;
		public double MinimumAge;
	}

	internal class colourscheme
	{
		public string BG;
		public string BGCOLOR;
		public string FGCOLOR;
		public string HLCOLOR;
		public string LLCOLOR;
	}

	internal class HintRefFields
	{
		public string SPECIES;
		public double FR;
		public double FITNESS;
		public double WEIGHT;
		public double ENERGY;
		public bool DROWN;
		public bool SINK;
		public bool MOTHER;
		public string MATING;
		public string ERROR;

		internal string[] ToStringArray()
		{
			List<string> list = new List<string>();

			if (SPECIES != null) list.Add(SPECIES);
			if (FR != 0) list.Add(FR.ToString());
			if (FITNESS != 0) list.Add(FITNESS.ToString());
			if (WEIGHT != 0) list.Add(WEIGHT.ToString());
			if (ENERGY != 0) list.Add(ENERGY.ToString());
			if (DROWN) list.Add(DROWN.ToString());
			if (SINK) list.Add(SINK.ToString());
			if (MOTHER) list.Add(MOTHER.ToString());
			if (MATING != null) list.Add(MATING.ToString());
			if (ERROR != null) list.Add(ERROR.ToString());

			string[] array = list.ToArray();
			return array;
		}
	}

	internal struct PageContent
	{
		internal string BGColor;
		internal string FGColor;
		internal string HLColor;
		internal string LLColor;
		internal string Status;
		internal string Map;
		internal string Key;
		internal string Compass;
		internal string Description;
		internal string View;
		internal string Species;
		internal string ImagesURL;

		internal string MapImagesURL;

		internal int Level;
		internal string Weight;
		internal string Energy;
		internal string Fitness;
		internal int Score;
		internal string WeightImg;
		internal string EnergyImg;
		internal string FitnessImg;
		internal string Text;
		internal string ScoresURL;

		internal string[] Keys
		{
			get
			{
				return new string[]
				{
					"BGColor",
					"FGColor",
					"HLColor",
					"LLColor",
					"Status",
					"Map",
					"Key",
					"Compass",
					"Description",
					"View",
					"Species",
					"ImagesURL",

					"MapImagesURL",

					"Level",
					"Weight",
					"Energy",
					"Fitness",
					"Score",
					"WeightImg",
					"EnergyImg",
					"FitnessImg",
					"Text",
					"ScoresURL",
				};
			}
		}

		internal string GetValue(string key)
		{
			switch (key)
			{
				case "BGColor": return BGColor;
				case "FGColor": return FGColor;
				case "HLColor": return HLColor;
				case "LLColor": return LLColor;
				case "Status": return Status;
				case "Map": return Map;
				case "Key": return Key;
				case "Compass": return Compass;
				case "Description": return Description;
				case "View": return View;
				case "Species": return Species;
				case "ImagesURL": return ImagesURL;

				case "MapImagesURL": return MapImagesURL;

				case "Level": return Level.ToString();
				case "Weight": return Weight;
				case "Energy": return Energy;
				case "Fitness": return Fitness;
				case "Score": return Score.ToString();
				case "WeightImg": return WeightImg;
				case "EnergyImg": return EnergyImg;
				case "FitnessImg": return FitnessImg;
				case "Text": return Text;
				case "ScoresURL": return ScoresURL;
			}
			return null;
		}

		internal bool HasValue(string key)
		{
			switch (key)
			{
				case "BGColor": return BGColor != null;
				case "FGColor": return FGColor != null;
				case "HLColor": return HLColor != null;
				case "LLColor": return LLColor != null;
				case "Status": return Status != null;
				case "Map": return Map != null;
				case "Key": return Key != null;
				case "Compass": return Compass != null;
				case "Description": return Description != null;
				case "View": return View != null;
				case "Species": return Species != null;
				case "ImagesURL": return ImagesURL != null;

				case "MapImagesURL": return MapImagesURL != null;

				case "Level": return Level != 0;
				case "Weight": return Weight != null;
				case "Energy": return Energy != null;
				case "Fitness": return Fitness != null;
				case "Score": return Score != 0;
				case "WeightImg": return WeightImg != null;
				case "EnergyImg": return EnergyImg != null;
				case "FitnessImg": return FitnessImg != null;
				case "Text": return Text != null;
				case "ScoresURL": return ScoresURL != null;
			}
			return false;
		}
	}

	internal class localFile
	{
		int currentLine = 0;
		internal string[] fileText { get; private set; }

		internal localFile(string[] fileContent)
		{
			fileText = fileContent;
		}

		internal string getLine()
		{
			if (currentLine < fileText.Length)
			{
				currentLine++;
				return fileText[currentLine - 1];
			}
			return null;
		}

		internal void chomp()
		{

		}
	}

	internal class Carp
	{
		internal static void confess(string v)
		{
			Logger.LogError(v);
		}
	}
}
