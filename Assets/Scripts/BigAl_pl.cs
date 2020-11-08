using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PerlInterface;

public partial class BigAl_pl
{
	//#!/usr/local/bin/perl

	//#########################################################################################
	//#
	//# bigal.pl - The Big Al Game - LIVE VERSION
	//#
	//# This perl CGI is called repeatedly for each move in the game.
	//# It maintains state using a cookie (subroutines in the state section read & write global variables to the cookie)
	//# The user interactions arrive in the form of the 'action' parameter.
	//#
	//# Version : 1.93
	//# Author  : John Alden
	//  C# Unity port : Jennifer Edwards
	//#
	//# Change History:
	//#
	//  V2.00
	//	JE	   Sep 2020	 C# conversion	
	//
	//# V1.93
	//# JA     Dec 2000  Fixed bug in movement.
	//#
	//# V1.92
	//# JA     Dec 2000  Trivial changes to text messages.
	//#
	//# V1.91
	//# JA     Dec 2000  Score is now trivially [XOR] encrypted in the cookie.
	//#
	//# V1.9
	//# JA     Dec 2000  Move number added to cookie to detect cheating.
	//#
	//# V1.8
	//# JA  29 Nov 2000  Logfile dropped in favour of STDERR=>Apache log as suggested by erik.
	//# River now has a timer for drowning.
	//# V1.7
	//# JA  13 Nov 2000  Template parser changed to use {{}} rather than <<>> - more tag-checker friendly
	//# Egg distribution & tracking added.  Javascript moved out to an external file.
	//#
	//# V1.6
	//# JA  1  Nov 2000  Connection speed cookie parameter added. Time stretch addded for slow connections.
	//#
	//# V1.5
	//# JA  30 Oct 2000  Modified for new-look output.  Templates modified accordingly.
	//# Suspend and resume feature added. Max no species added.
	//#
	//# V1.4
	//# JA  30 Sep 2000  Template-based output [TemplateParser.pm]; Bug fixed in species leaving; Size parameter added.
	//#
	//# V1.3
	//# JA  29 Sep 2000  Mating and sauropod hunting more-or-less in final form
	//# Some output routines modified to return strings rather than print directly.
	//# JA  22 Sep 2000  Javascript escaped with HTML comments for old browsers
	//#
	//# V1.2
	//# JA  19 Sep 2000  Resource files moved to 'standard' location /content/data/
	//# Cookie size reduced further by using unescaped delimiter and limiting floating point values to 4sf
	//#
	//# V1.1
	//# JA  15 Sep 2000  Logfile for errors; Added injuries in forest; new sauropod hunting strategy
	//# JA  13 Sep 2000  Uses 6-bit encoding of map to reduce cookie length [SixBitEnc.pm]
	//# Bugs in sauropod hunting fixed
	//# Skeleton implementation of mating
	//#
	//# V1.0 uploaded onto dev3 September 2000
	//##########################################################################################

	//# Enable/Disable Debugging
	//	use vars qw(DEMO DEBUG);
	internal int DEBUG;
	int DEMO;

	public void BEGIN()
	{
		perlCookie = new PerlCookie[0]; //added

		//DEBUG = 0; //#Set to zero on release
		DEMO = 0; //#Demo mode (zero for release)

		if (DEBUG > 0)
		{
			//# Modules required for debugging only
			//require strict;
			//require CGI::Carp;
			//import CGI::Carp qw(fatalsToBrowser);
		}

		PerlFileLocSetup();

		main(); //added
	}

	internal void exit(int v) //added
	{
		PerlInterface.exit(v, this);
	}

	//#Standard modules
	//use English;
	//use CGI qw(:cgi);
	//require Carp; //#used to dump call stack to logfile on error

	//#Custom modules
	//use TemplateParser; //#HTML templating module
	//use SixBitEnc;      //#Six-bit encoding of binary data to keep cookies short

	//#########################################################################################
	//# FILE LOCATIONS
	//#########################################################################################

	static string scores_server = "http://cgi.bbc.co.uk"; //#LIVE VERSION

	//#Directories - deduce from APACHE ENV VARS
	static string dir = ENV.SCRIPT_FILENAME;
	string own_filename = "";
	static string files_dir = "";
	
	//#URLs - deduce from APACHE ENV VARS
	static string cgi_url = ENV.REQUEST_URI;
	string www_url = "";

	//#Resource files
	string desc_file = "";
	string img_file = "";
	string js_file = "";
	string hint_file = "";
	string questions_file = "";

	//#URLs for links
	string scores_url = "";
	string fact_files_url = "http://www.bbc.co.uk/dinosaurs/fact_files"; //# URL for fact files
	string backgrounds_url = "";       //# habitat backgrounds
	string images_url = "";            //# misc images
	string species_images_url = "";    //# images of species
	string location_images_url = "";   //# images for map locations
	string map_images_url = "";			//# images for map squares
	string status_images_url = "";     //# images for Al's status
	string compass_images_url = "";    //# images for compass rollover

	//#Lock down cookies to big al game
	string cookie_path = "";
	string cookie_domain = ".bbc.co.uk";
	int cookie_crypt = 6474950; //#encrypt score

	void PerlFileLocSetup()
	{
		string[] matches = perlSubstituteReturningMatches(dir, @"(.*)\\", @"(.*?\\.pl)"); //todo: no matches
		dir = matches[0];
		own_filename = matches[1];
		files_dir = dir;
		files_dir = perlSubstitute(files_dir, "cgi-bin", "data");

		matches = perlSubstituteReturningMatches(cgi_url, @"(.*)\\", @".*?\\.pl(\?.*) ?");
		cgi_url = matches[0];
		www_url = cgi_url;
		www_url = perlSubstitute(www_url, @"cgi-bin\\", "");

		desc_file = $"{files_dir}" + /*/*/"map_descriptions.txt";
		img_file = $"{files_dir}" + /*/*/"map_images.txt";
		js_file = $"{files_dir}" + /*/*/"javascript.js";
		hint_file = $"{files_dir}" + /*/*/"hints.txt";
		questions_file = $"{files_dir}" + /*/*/"mating_questions.txt";

		//#URLs for links
		scores_url = $"{scores_server}{cgi_url}" + /*/*/"bigal_scores.pl";
		backgrounds_url = $"{www_url}" + /*/*/"backgrounds";       //# habitat backgrounds
		images_url = $"{www_url}" + /*/*/"images";            //# misc images
		species_images_url = $"{www_url}" + /*/*/"species_images";    //# images of species
		location_images_url = $"{www_url}" + /*/*/"location_images";   //# images for map locations
		map_images_url = $"{www_url}" + /*/*/"map_square_images"; //# images for map squares
		status_images_url = $"{www_url}" + /*/*/"status_images";     //# images for Al's status
		compass_images_url = $"{www_url}" + /*/*/"compass_images";    //# images for compass rollover

		//#Lock down cookies to big al game
		cookie_path = cgi_url;
		debug("cookies limited to " + cookie_domain + cookie_path);
	}

	//#########################################################################################
	//# GAME DEFINITION
	//#########################################################################################

	//# This is the key for the map
	//# Controls how the habitats will be displayed in terms of background image and colour scheme
	Dictionary<char, Dictionary<string, string>> habitats = new Dictionary<char, Dictionary<string, string>>()
	{
		{ 'X', new Dictionary<string, string>(){ { "FILE", "nest.html" },{ "BG", "nest_bg.jpg" }, { "MCOLOR", "#666600" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33" }, { "HL", "WHITE" }, { "LL", "#909090" },{ "TEXT", "WHITE" },{ "LINK", "#CCAA33" } } },
		{ 'N', new Dictionary<string, string>(){ { "FILE", "nest.html"},{ "BG", "nest_bg.jpg" },   { "MCOLOR", "#666600" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'F', new Dictionary<string, string>(){ { "FILE", "forest.html" }, { "BG", "forest_bg.jpg" }, { "MCOLOR", "#006600" }, { "BGCOLOR", "#003300" }, { "FGCOLOR", "#33CC99"}, {"HL", "WHITE"},{ "LL", "#114422" }, {"TEXT", "WHITE"},{ "LINK", "#33CC99"}} },
		{ 'E', new Dictionary<string, string>(){ { "FILE", "forest_edge.html" },{ "BG", "forest_bg.jpg" }, { "MCOLOR", "#00FF00" }, { "BGCOLOR", "#003300" }, { "FGCOLOR", "#33CC99"}, {"HL", "WHITE"},{ "LL", "#114422" }, {"TEXT", "WHITE"},{ "LINK", "#33CC99"}} },
		{ 'P', new Dictionary<string, string>(){ { "FILE", "plain.html" }, { "BG", "plains_bg.jpg" }, { "MCOLOR", "#666600" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'S', new Dictionary<string, string>(){ { "FILE", "sand.html" }, { "BG", "sand_bg.jpg" },   { "MCOLOR", "#CCCC00" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'Q', new Dictionary<string, string>(){ { "FILE", "quicksand.html" },  { "BG", "sand_bg.jpg" },   { "MCOLOR", "#CCCC00" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'H', new Dictionary<string, string>(){ { "FILE", "sauropod.html" },   { "BG", "plains_bg.jpg" }, { "MCOLOR", "#660000" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'D', new Dictionary<string, string>(){ { "FILE", "desert.html" }, { "BG", "desert_bg.jpg" }, { "MCOLOR", "#FFFF00" }, { "BGCOLOR", "#443300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'R', new Dictionary<string, string>(){ { "FILE", "river.html" }, {"BG", "river_bg.jpg" },  { "MCOLOR", "#0000FF" }, {"BGCOLOR", "#000033" }, {"FGCOLOR", "#3399CC"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#3399CC"}} },
		{ 'B', new Dictionary<string, string>(){ { "FILE", "riverbank.html"},  { "BG", "river_bg.jpg" },  { "MCOLOR", "#6666FF" },{ "BGCOLOR", "#000033" },{ "FGCOLOR", "#3399CC"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#3399CC"}} },
		{ 'C', new Dictionary<string, string>(){ { "FILE", "riverbank.html"},  { "BG", "nest_bg.jpg" },   { "MCOLOR", "#666600" }, { "BGCOLOR", "#333300" }, { "FGCOLOR", "#CCAA33"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#CCAA33"}} },
		{ 'W', new Dictionary<string, string>(){ { "FILE", "waterhole.html"},  { "BG", "river_bg.jpg" },  { "MCOLOR", "#0000FF" },{ "BGCOLOR", "#000033" },{ "FGCOLOR", "#3399CC"}, {"HL", "WHITE"},{ "LL", "#909090" }, { "TEXT", "WHITE"},{ "LINK", "#3399CC"}} },
	};

	//# The habitat map
	char[][] map = new char[][]
	{
				 //#  A    B    C    D    E    F    G    H    I    J    K    L    M    N    O    P    Q
		 new char[] {'F', 'F', 'N', 'F', 'F', 'E', 'S', 'Q', 'S', 'P', 'P', 'H', 'H', 'P', 'P', 'P', 'P' }, //#1
		 new char[] {'F', 'F', 'F', 'F', 'E', 'P', 'S', 'Q', 'S', 'P', 'P', 'H', 'H', 'P', 'P', 'P', 'P' }, //#2
		 new char[] {'F', 'F', 'F', 'F', 'E', 'P', 'S', 'Q', 'S', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' }, //#3
		 new char[] {'F', 'F', 'F', 'E', 'P', 'P', 'S', 'Q', 'S', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' }, //#4
		 new char[] {'N', 'F', 'F', 'E', 'P', 'P', 'B', 'R', 'B', 'P', 'P', 'P', 'D', 'D', 'D', 'D', 'P' }, //#5
		 new char[] {'F', 'F', 'F', 'E', 'P', 'P', 'B', 'R', 'C', 'P', 'P', 'D', 'D', 'D', 'D', 'D', 'D' }, //#6
		 new char[] {'F', 'F', 'F', 'E', 'P', 'P', 'B', 'R', 'B', 'P', 'P', 'D', 'D', 'W', 'W', 'D', 'D' }, //#7
		 new char[] {'F', 'F', 'F', 'X', 'P', 'P', 'B', 'R', 'B', 'P', 'P', 'D', 'D', 'W', 'W', 'D', 'D' }, //#8
		 new char[] {'F', 'F', 'E', 'P', 'P', 'P', 'B', 'R', 'B', 'P', 'P', 'P', 'D', 'D', 'D', 'D', 'D' }, //#9
		 new char[] {'F', 'E', 'P', 'P', 'P', 'P', 'B', 'R', 'B', 'P', 'P', 'P', 'P', 'D', 'D', 'D', 'D' }  //#10
	};

	//######################################################
	//# SPECIES
	//######################################################
	//# URL (for fact file)
	//# Image
	//# Nutrition (weight units)
	//# Danger (weight units)
	//# Aggression (probability: 0 to 1 - default=0)
	//# Difficulty (0 to 1 - default=0)
	//# Injury (description of how it hurts you)
	//# Enter (optional list of text to display when one appears)
	//# Attack (optional text to display when attacking)
	//######################################################

	internal static Dictionary<string, speciesObj> species = new Dictionary<string, speciesObj>()
	{
		{ "Dragonfly", new speciesObj()
			{
				URL = "plants_insects/dragonfly.shtml",
				Image = "dragonfly.jpg",
				Nutrition = 0.05,
				Difficulty = 0.7,
				Enter = new string[]{ "flying past","hovering","which has just landed","darting past","darting around" }
			}
		},
		{ "Centipede", new speciesObj()
			{
				URL = "plants_insects/centipede.shtml",
				Image = "centipede.jpg",
				Nutrition = 0.2,
				Difficulty = 0.3,
				Enter = new string[] { "under a leaf", "crossing your path", "in the undergrowth", "hunting through the leaf litter" }
			}
		},
		{ "Scorpion", new speciesObj()
			{
				URL = "plants_insects/scorpion.shtml",
				Image = "scorpion.jpg",
				Nutrition = 0.5,
				Difficulty = 0.4,
				Danger = 10,
				Injury = "stings you",
				Enter = new string[] { "moving slowly past you", "hidden beneath a rock", "underneath a leaf" }
			}
		},
		{ "Frog", new speciesObj()
			{
				URL = "bigal/amphibian_reptile/frog.shtml",
				Image = "frog.jpg",
				Nutrition = 0.5,
				Difficulty = 0.5f
			}
		},
		{ "Lizard", new speciesObj()
			{
				URL = "bigal/amphibian_reptile/lizard.shtml",
				Image = "lizard.jpg",
				Nutrition = 2,
				Difficulty = 0.5,
				Danger = 0.5,
				Injury = "bites you",
				Enter = new string[] { "darting past", "basking in the sun", "camouflaged in the undergrowth", "lying close to the bark on a branch" }
			}
		},
		{ "Sphenodontian", new speciesObj()
			{
				URL = "bigal/amphibian_reptile/tuatara.shtml",
				Image = "tuatara.jpg",
				Nutrition = 8,
				Difficulty = 0.3,
				Danger = 5,
				Injury = "bites you",
				Enter = new string[] { "basking in the sun", "camouflaged in the undergrowth", "lying motionless" }
			}
		},
		{ "Crocodile", new speciesObj()
			{
				URL = "bigal/amphibian_reptile/deinosuchus.shtml",
				Image = "crocodile.jpg",
				Aggression = 0.6,
				Nutrition = 5,
				Difficulty = 0.4,
				Danger = 1000,
				Injury = "drags you under the water, but you manage to break free",
				Enter = new string[] { "swimming towards you", "basking on the river bank", "sliding into the water" }
			}
		},
		{ "Pterosaur", new speciesObj()
			{
				URL = "bigal/pterodactylus.shtml",
				Image = "pterosaur.gif",
				Aggression = 0.7,
				Nutrition = 20,
				Difficulty = 0.6,
				Danger = 10,
				Injury = "snaps at you",
				Enter = new string[] { "soaring around your head", "flying past", "feeding on the ground" }
			}
		},
		{ "injured Pterosaur", new speciesObj()
			{
				URL = "bigal/pterodactylus.shtml",
				Image = "pterosaur.gif",
				Nutrition = 20,
				Danger = 0.2,
				Enter = new string[] { "floundering on the ground", "dragging its wing" }
			}
		},
		{ "Dryosaurus", new speciesObj()
			{
				URL = "bigal/dryosaurus.shtml",
				Image = "dryosaurus.gif",
				HTML = "<I>Dryosaurus</I>",
				Aggression = 0,
				Nutrition = 50,
				Danger = 20,
				Difficulty = 0.2,
				MinimumAge = 0.05,
				Injury = "kicks you",
				Enter = new string[] { "running by","crouching nervously" }
			}
		},
		{ "Othnielia", new speciesObj()
			{
				URL = "bigal/othnielia.shtml",
				Image = "othnielia.gif",
				HTML = "<I>Othnielia</I>",
				Aggression = 0,
				Nutrition = 50,
				Danger = 20,
				Difficulty = 0.2,
				MinimumAge = 0.05,
				Injury = "kicks you",
				Enter = new string[] { "running by","startled by your appearance" }
			}
		},
		{ "Ornitholestes", new speciesObj()
			{
				URL = "scrub/ornitholestes.shtml",
				Image = "ornitholestes.gif",
				HTML = "<I>Ornitholestes</I>",
				Aggression = 0.9,
				Nutrition = 50,
				Danger = 50,
				MinimumAge = 0.1,
				Injury = "bites you"
			}
		},
		{ "Stegosaurus", new speciesObj()
			{
				URL = "scrub/stegosaurus.shtml",
				HTML = "<I>Stegosaurus</I>",
				Image = "stegosaurus.gif",
				Nutrition = 500,
				Danger = 1300,
				MinimumAge = 0.05,
				Injury = "whacks you with its tail",
				Enter = new string[] { "grazing on a lush patch of vegetation","ambling past unperturbed" }
			}
		},
		{ "Diplodocus", new speciesObj()
			{
			  	URL = "bigal/diplodocus.shtml",
				HTML = "<I>Diplodocus</I>",
				Image = "diplodocus.jpg",
				Nutrition = 3500,
				Danger = 2000,
				Injury = "swipes at you with its tail",
				Attack = "You run the herd and manage to isolate one that is weaker than the others"
			}
		},
		{ "Juvenile Allosaurus", new speciesObj()
			{
				URL = "scrub/allosaurus.shtml",
				HTML = "Juvenile <I>Allosaurus</I>",
				Image = "allosaurus.gif",
				Aggression = 0.6,
				Nutrition = 20,
				Danger = 100,
				Difficulty = 0.7,
				MinimumAge = 0.5,
				Injury = "attacks you",
			}
		},
		{ "Male Allosaurus", new speciesObj()
			{
				URL = "scrub/allosaurus.shtml",
				HTML = "Male <I>Allosaurus</I>",
				Image = "allosaurus.gif",
				Aggression = 0.9,
				Nutrition = 200,
				Danger = 2000,
				Injury = "savages you",
			}
		},
		{ "Female Allosaurus", new speciesObj()
			{
				URL = "scrub/allosaurus.shtml",
				HTML = "Female <I>Allosaurus</I>",
				Image = "allosaurus.gif",
				Aggression = 0.9,
				Nutrition = 250,
				Danger = 2500,
				Injury = "savages you",
			}
		}
	};

	//# Species distribution
	Dictionary<char, Dictionary<string, double>> habitat_species_freq = new Dictionary<char, Dictionary<string, double>>()
	{
		{ 'X', new Dictionary<string, double>(){ { "Dragonfly", 0.25 }, { "Scorpion", 0.1 }, { "Centipede", 0.25 } } },
		{ 'N', new Dictionary<string, double>(){ { "Dragonfly", 0.25 }, { "Scorpion", 0.1 }, { "Centipede", 0.25 }, { "Ornitholestes", 0.7 } } },
		{ 'C', new Dictionary<string, double>(){ { "Crocodile", 0.5 }} },
	  { 'F', new Dictionary<string, double>(){ { "Dragonfly", 0.2 }, { "Scorpion", 0.15 }, { "Centipede", 0.2 },{ "Dryosaurus", 0.1 }, { "Othnielia", 0.1},
		  { "Ornitholestes", 0.1 }, {"Lizard", 0.2 }, {"Sphenodontian", 0.05 }, {"Juvenile Allosaurus", 0.1 } }},
	  { 'E', new Dictionary<string, double>(){ { "Dragonfly", 0.2 }, { "Scorpion", 0.15} , { "Centipede", 0.1} , { "Dryosaurus", 0.1} , { "Othnielia", 0.15 },
		  { "Ornitholestes", 0.1} , { "Stegosaurus", 0.05} , { "Lizard", 0.1} , { "Sphenodontian", 0.1} ,
		  { "Male Allosaurus", 0.1} , { "Female Allosaurus", 0.1} , { "Juvenile Allosaurus", 0.1} } },
	  { 'P', new Dictionary<string, double>(){ { "Scorpion", 0.05 }, { "Lizard", 0.1 }, { "Sphenodontian", 0.1 }, { "Pterosaur", 0.2 },
		  { "Stegosaurus", 0.10 },{ "Dryosaurus", 0.05} , { "Othnielia", 0.05 }, {"Male Allosaurus", 0.1 },
		  { "Female Allosaurus", 0.1 } } },
	  { 'D', new Dictionary<string, double>(){ { "Scorpion", 0.1 }, {"Sphenodontian", 0.05 }, {"Lizard", 0.05 }, {"injured Pterosaur", 0.05 },
		  { "Male Allosaurus", 0.05 }, {"Female Allosaurus", 0.05 } } },
	  { 'W', new Dictionary<string, double>(){ { "Dragonfly", 0.25 }, {"Frog", 0.25 }, {"Scorpion", 0.05 }, {"Lizard", 0.1 }, {"Sphenodontian", 0.1 }, {"Stegosaurus", 0.2 },
		  { "Othnielia", 0.1 }, {"Dryosaurus", 0.1 }, {"Juvenile Allosaurus", 0.1 },
		  { "Male Allosaurus", 0.1 }, {"Female Allosaurus", 0.8 } } },
	  { 'R', new Dictionary<string, double>(){ { "Dragonfly", 0.25 }, { "Frog", 0.25 }, { "Crocodile", 0.5 } } },
	  { 'B', new Dictionary<string, double>(){ { "Dragonfly", 0.35 }, {"Frog", 0.35 }, {"Crocodile", 0.4 }, {"Stegosaurus", 0.10 }, {"Dryosaurus", 0.05 },
		  { "Othnielia", 0.05 },{ "Male Allosaurus", 0.1 }, {"Female Allosaurus", 0.1 } } },
	  { 'H', new Dictionary<string, double>(){ { "Pterosaur", 0.2} } }
	};

	//# Egg distribution when the game starts
	Dictionary<string, int> initial_egg_distribution = new Dictionary<string, int>()
	{
		{ "A5", 3 },  { "C1", 4 },  { "I6", 5 }
	};

	//# Levels
	string[] levels = new string[] { "hatchling", "juvenile", "sub-adult", "adult" };
	int[] level_weight = new int[] { 0, 10, 500, 2000, 2500 }; //#[kg]
	internal Dictionary<int, colourscheme> level_colourscheme = new Dictionary<int, colourscheme>()
	{
		{ 1, new colourscheme(){ BG = "level_1/background.jpg", BGCOLOR = "#003300", FGCOLOR = "#009900", HLCOLOR = "#66CC33", LLCOLOR = "#006600"} },
		{  2, new colourscheme(){ BG = "level_2/background.jpg", BGCOLOR = "#333300", FGCOLOR = "#99CC00", HLCOLOR = "#CCFF33", LLCOLOR = "#669900"} },
		{  3, new colourscheme(){ BG = "level_3/background.jpg", BGCOLOR = "#663300", FGCOLOR = "#CC6600", HLCOLOR = "#FF9933", LLCOLOR = "#993300"}},
		{  4, new colourscheme(){ BG = "level_4/background.jpg", BGCOLOR = "#330000", FGCOLOR = "#CC3300", HLCOLOR = "#FF6600", LLCOLOR = "#993300"}},
		{ -1, new colourscheme(){ BG = "level_1/background.jpg", BGCOLOR = "#003300", FGCOLOR = "#009900", HLCOLOR = "#66CC33", LLCOLOR = "#006600"}}, //#DEAD
		{ 0, new colourscheme(){ BG = "level_1/background.jpg", BGCOLOR = "#003300", FGCOLOR = "#009900", HLCOLOR = "#66CC33", LLCOLOR = "#006600"}} //added: debug situation
	};

	//# Ages of other animals
	Dictionary<string, double> age_ranges = new Dictionary<string, double>()
	{
		{ "hatchling", 0.1 },	//# 0 - 10%
		{ "young", 0.5 },		//#10 - 50%
		{ "sub-adult", 0.7 }	//#50 - 70%
	};

	//# Biological constants
	const int max_weight = 2100;  //#maximum weight for Al
	const int wait_weight_loss=0;
	const int move_weight_loss = 0;  //# weight loss per move [percentage units]
	int wait_energy_loss=2;
	int move_energy_loss = 6;  //# energy loss per move [percentage units]
	const int healing_rate=2; //#how much fitness is replenished per move [percentage units]
	const double roaming_rate =0.5; //#how much species roam about [0-1]
	const double behaviour_randomness = 0.2;     //#20% of danger [0-1]
	const int energy_per_bodyweight = 10;     //#amount of energy per entire bodyweight (if you eat something half your bodyweight, your energy will go up by 50*this)
	const int min_weight_for_river = 500;     //#[kg]
	const int max_weight_for_quicksand = 250; //#[kg]
	const double forest_injury_prob = 0.25;       //#[0-1]
	const int rt_min=6; //#Minimum reaction time [sec]
	const int rt_var=10; //#Maximum variation in reaction time (added to minimum) [sec]
	const int mother_memory=5; //#How long Al can be away before she eats him [moves]
	const double mother_move_prob =0.6; //#How likely is she to wander off [0-1]
	const double mating_failure =0.2; //#How likely is Al to fail at mating (when inexperienced) [0-1]
	const double egg_nutrition =0.3; //#How much nutrition is an egg worth?
	const double sauropod_move_prob =0.5; //#How likely herd is to move

	//# Misc constants
	const int max_species=4;
	const int species_cols=2;
	const int map_sq_size=14; //#size in pixels
	const int map_mode=1; //#0=always, 1=as you go, 2=never
	const string cookie_lifetime="+1M"; //#Cookies last 1 month
	const string cookie_field_delimiter ="_"; //#character used to separate fields in cookie
	const int mating_bonus = 1000; //#number of points scored for a mating

	internal string[][] compass = new string[][] 
	{
		  new string[]{ "Northwest", "North", "Northeast" },
		  new string[]{ "West", "", "East" },
		  new string[]{ "Southwest", "South", "Southeast" }
	};

	internal string[][] compass_images = new string[][]
	{
		  new string[]{ "northwest", "north", "northeast" },
		  new string[]{ "west", "", "east" },
		  new string[]{ "southwest", "south", "southeast" }
	};

	//#########################################################################################


	//##############################################################
	//# Main Program
	//##############################################################

	//# Write HTTP header for debug (can output comments now)
	//if(DEBUG == 2) print("Content-type: text/html\n\n" ;

	//#State
	internal int[][] visited;
	public int move_number;
	internal int x; internal int y; //#my location				  
	internal int mx; internal int my; //#mother's / herd's location				
	internal int level; internal string decision = ""; internal double weight; internal double energy; internal double fitness; internal int matings; internal int score; internal string cxn_speed; //#cxn speed = F|M|S
	internal string level_data;
	internal Dictionary<string, string> species_present_prev = new Dictionary<string, string>(); //#species being transferred from the previous move
	internal Dictionary<string, string> species_present = new Dictionary<string, string>(); //#species which have appeared in this move
	internal Dictionary<string, int> egg_distribution = new Dictionary<string, int>();

	//#Transients
	internal int page_move;
	internal int ny => scalar(map);
	internal int nx =17; //#scalar(map[0]);
	internal int timer =0;
	internal int time_stretch =1;
	//double hiscore=0;
	internal bool have_mother = false;
	internal int old_mx; int old_my;
	internal double metabolic_rate = 1;
	internal double nutrition_metabolism_exponent =0.8; //#rate at which nutrition varies with metabolic rate
	internal int current_pack_size =1;
	internal List<string> species_leaving = new List<string>(); //#those that get out alive
	internal string next_action;     //#the next default action
	internal bool suspended;       //#is the game suspended
	internal string extra_desc;
	internal string debug_str;
	internal bool levellingUp; //added

	public void main()
	{
		ResetData(); //added
		//#
		//# TRY BLOCK
		//#
		try
		{
			srand(new System.DateTime().TimeOfDay.Milliseconds);

			//#------------------------------------------
			//# Get state from previous move
			//#------------------------------------------
			Logger.Log("main");
			level = 0; //#If state is not read, level will remain at 0
			ReadState();
			if (paramBool("BEGIN")|| perlRegex(paramString("action"), "begin", ignoreCase: true) || level<1)
			{
				InitialiseState();
			}

			//#------------------------------------------
			//# Security checks on incoming values
			//#------------------------------------------
			if (!(perlRegex(level.ToString(), @"^-?\d+$", ignoreCase: false) && level <= scalar(levels))) perlException("Invalid level");

			//#------------------------------------------
			//# Check for cheating: page vs cookie
			//#------------------------------------------
			bool cheating = false;
			if (move_number != page_move)
			{
				debug($"Trying to cheat! c:{move_number} vs p:{page_move}");
				if (DEBUG == 0)
				{
					extra_desc = "Trying to cheat eh?  We've thought of that, I'm afraid.  ";
					cheating = true;
				}
			}

			//  //#------------------------------------------
			//  //# Settings dependent on connection speed
			//  //#------------------------------------------
			if (paramString("'cxn_speed'") != "")
			{
				cxn_speed = paramString("'cxn_speed'"); //#rewrite cookie value from param
			}
			if (cxn_speed == "M") time_stretch = 3;
			if (cxn_speed == "S") time_stretch = 5;

			//  //#------------------------------------------
			//  //# This move
			//  //#------------------------------------------
			bool same_move = false;
			if(perlRegex(paramString("'action'"), "^(pause|resume|popup|mate)", ignoreCase: true)) same_move = true;
			if (!(same_move || cheating))
			{
				//#Modify environment based on decisions
				if (level == 2) System.Double.TryParse(decision, out metabolic_rate);
				have_mother = (level == 1 && decision == "Yes");
				if (level >= 3) int.TryParse(level_data, out current_pack_size);
				if (current_pack_size == 0) current_pack_size = 1;

				//#Healing
				fitness += healing_rate * metabolic_rate;
				if (fitness > 100) fitness = 100;

				//#Track our location
				old_mx = mx; old_my = my;
				visited[y][x] = 1;
			}

			if (!cheating)
			{
				//#Process action from last page - move, attack, mate etc
				DoAction();

				//# Move up to the next level if we've eaten enough - shortcuts the rest of the move
				levellingUp = weight > level_weight[level]; //added
				if (weight > level_weight[level]) NextLevel();
			}

			if (!(same_move || cheating))
			{
				//#Possible habitat-driven event
				CheckHabitat();

				//#Meet other animals?
				if (level == 3) CalcHuntingAllosaurs();
				if (have_mother) MeetMother();
				RandomEncounter();
				if (have_mother) MoveMother();
			}
			else
			{
				//#Copy species across from last move
				foreach (var item in species_present_prev.Keys)
				{
					species_present[item] = species_present_prev[item];
				}
			}

			//#------------------------------------------
			//# Output
			//#------------------------------------------
			WriteHeader(); //#Write new state here
			if (!suspended) WriteContent();
			if (suspended) WriteSuspended();
			WriteFooter();
			exit(0);
		}
		catch (Exception EVAL_ERROR)
		{
			//#
			//# Catch Block
			//#
			if (EVAL_ERROR != null && EVAL_ERROR.Message != "exit")
			{
				var err = EVAL_ERROR;

				//#Write to error log
				pprintError($"bigal.pl: {err}; state:" + WriteStateToDebug() + "\n");
				//Carp::cluck("bigal.pl: stack trace:"); //#append a stack trace

				//#If there is an error, report it if in debug mode
				//#If in release mode then cover up by killing off al
				if (DEBUG > 0)
				{
					WriteHeader();
					pprint($"An Error has occurred: {err} <BR>\n");
					pprint("Action: " + paramString("'action'") + "<BR>\n");
					pprint("State: " + WriteStateToDebug() + "<BR>\n");
					WriteFooter();
				}
				else
				{
					Die(new HintRefFields() { ERROR = err.ToString() }); //#Kill off Al with some nonsense about asteroids...
				}
			}
		}
	}

	private void ResetData() //added
	{
		gameover = false;
		popupURL = null;

		extra_desc = "";
		next_action = "";
		decision = "";
		suspended = false;
		timer = 0;

		wait_energy_loss = 2;
		move_energy_loss = 6;
		metabolic_rate = 1;

		species_present_prev.Clear();
		species_present.Clear();
		species_leaving.Clear();
		enemyBars.Clear();
	}

	//##############################################################
	//# End
	//##############################################################

	//##############################################################
	//# Alternative exit points
	//##############################################################

	void NextLevel()
	{
		level++; next_action=""; level_data="";
		if (level==4) {mx=11; my=0;}
		WriteHeader(1); //#Write new begin_level cookie
		WriteNextLevel();
		WriteFooter();
		exit(0);
	}

	void Quit()
	{
		WriteHeader();
		WriteQuit();
		WriteFooter();
		exit(0);
	}

	internal bool gameover; //added
	void Die(HintRefFields param)
	{
		HintRefFields hintref = param;
		level=-1; //#restart next time round
		timer=0;  //#wipe out any timer that may have been set
		debug("YOU'RE DEAD: reason:" + join(",", hintref.ToStringArray()));
		WriteHeader();
		WriteGameOver(GenerateGameOverText(hintref));
		WriteFooter();
		gameover = true;
		exit(0);
	}

	//##############################################################
	//##
	//## State Subroutines
	//##
	//##############################################################

	void InitialiseState()
	{
		debug("Initialising state");
		move_number=0; page_move = move_number;
		level=0; weight=0.2; fitness=100; energy=100; matings=0; score=0;
		mx=x=3; my=y=7;

		//#Zero the visited matrix
		visited = new int[ny][];
		for(int j=1; j<=ny; j++)
		{
			int[] list = new int[nx];
			for(int i=1; i<=nx; i++)
			{
				list[i - 1] = 0;
			}
			visited[j - 1] = list;
		}

		//#Set up the initial distribution of eggs
		egg_distribution = initial_egg_distribution;

		//#Only set the connection speed parameter if it is undefined
		if (string.IsNullOrEmpty(cxn_speed)) cxn_speed = "F";
	}

	void ReadState()
	{
		string action = paramString("'action'");
		page_move = paramInt("\"move_number\"");
		debug(2,"Reading state from cookie.");
		if (perlRegex(action, "reincarnate", ignoreCase: true))
		{
			debug("Reincarnating.");
			Logger.Log("cookie(\"begin_level\") " + cookie("begin_level"));
			ReadStateFromString(cookie("begin_level")); //#re-start from the beginning of the level
			level--; //#get to make decision again
			page_move = move_number;
		}
		else
		{
			Logger.Log("cookie(\"state\") " + cookie("state"));
			if (cookie("state") != null) ReadStateFromString(cookie("state"));
		}

		//#Allow state to be modified from CGI parameters in debug mode
		if(DEBUG > 0)
		{
			if (paramDouble("'weight'") != 0) weight = paramDouble("'weight'") ;
			if (paramInt("'level'") != 0) level = paramInt("'level'") ;
			if (paramString("'level_data'") != "") level_data = paramString("'level_data'") ;
		}

		//#Update state from parameters.
		if (paramString("'decision'") != "") decision = paramString("'decision'");
		if(level == 4 && !(perlRegex(paramString("'action'"),"mate", ignoreCase: true)))
		{
			decision=""; level_data=""; //#Reset mating parameters unless we're mating
		}
	}

	//#Convert all state variables to a delimited string
	CookieValues WriteStateToString()
	{
		string vis = WriteVisitedToString();
		string spec = WriteSpeciesToString();
		string eggs = WriteEggDistToString();
		CookieValues list = new CookieValues(move_number.ToString(),level.ToString(),decision,level_data,cxn_speed,x.ToString(),y.ToString(),mx.ToString(),my.ToString(),(score ^ cookie_crypt).ToString(),matings.ToString(),
	            round_to_sf(fitness,4).ToString(),round_to_sf(weight,4).ToString(),round_to_sf(energy,4).ToString(),vis,spec,eggs, 
				join(cookie_field_delimiter, new string[] {
					move_number.ToString(),level.ToString(),decision,level_data,cxn_speed,x.ToString(),y.ToString(),mx.ToString(),my.ToString(),(score ^ cookie_crypt).ToString(),matings.ToString(),
				round_to_sf(fitness,4).ToString(),round_to_sf(weight,4).ToString(),round_to_sf(energy,4).ToString(),vis,spec,eggs
				}));

		return list;// join(cookie_field_delimiter,list);
	}

	//#Parse state variables from delimited string
	void ReadStateFromString(string str)
	{
		string[] list= split(cookie_field_delimiter,str);

		string vis; string spec; string eggs;
		int.TryParse(list[0], out move_number);
		int.TryParse(list[1], out level);
		decision = list[2]; level_data = list[3]; cxn_speed = list[4];
		int.TryParse(list[5], out x);
		int.TryParse(list[6], out y);
		int.TryParse(list[7], out mx);
		int.TryParse(list[8], out my);
		int.TryParse(list[9], out score);
		int.TryParse(list[10], out matings);
		double.TryParse(list[11], out fitness);
		double.TryParse(list[12], out weight);
		double.TryParse(list[13], out energy);
		vis = list[14]; spec = list[15]; eggs = list[16];

		ReadVisitedFromString(vis);
		ReadSpeciesFromString(spec);
		ReadEggDistFromString(eggs);
		score ^= cookie_crypt; //#decrypt score
		debug(2,"Read state from string");
	}

	string WriteStateToDebug()
	{
		List<string> list= new List<string>(){$"Move={move_number}",$"X={x}",$"Y={y}", $"MX={mx}", $"MY={my}",
				$"Level={level}",$"Decision={decision}", $"Level Data={level_data}",
				$"Score={score}",$"Matings={matings}", $"Cxn speed={cxn_speed}",
				$"Fitness={fitness}",$"Weight={weight}",$"Energy={energy}",
				$"Species=[{WriteSpeciesToString()}]",
				$"Egg Locations=[{WriteEggDistToString()}]"};
		return string.Join(";", list);
	}

	//# Convert visited matrix into a string of numbers
	string WriteVisitedToString()
	{
		string str="";
	  foreach (var row in visited)
	  {
	    foreach (var col in row)
	    {
	      str+=col;
	    }
		}
		return str;// SixBitEnc_pm.encode_binary_string(str); //todo
	}

	//# Parse string => visited matrix
	void ReadVisitedFromString(string viz)
	{
		//viz = SixBitEnc_pm.decode_binary_string(viz); //todo
		int cnt=0;
		visited = new int[ny][];
		for (int j = 1; j <=ny; j++)
		{
			int[] list = new int[nx];
			for (int i = 1; i <=nx; i++)
			{
				char vizChar = viz.Substring(cnt, 1).ToCharArray()[0];
				int.TryParse(vizChar.ToString(), out list[i - 1]);
				cnt++;
			}
			visited[j - 1] = list;
		}
	}

	string WriteEggDistToString()
	{
		List<string> list = new List<string>();
		foreach (var loc in egg_distribution.Keys)
		{
			list.Add($"{loc}:{egg_distribution[loc]}");
		}
		return string.Join("¼", list);
	}

	void ReadEggDistFromString(string str)
	{
		string[] list = split("/¼/",str); //#locations are delimited with ¼
		foreach (var item in list)
		{
			string[] sublist = split(":", item);
			string loc = sublist[0];
			string value = sublist[1];
			int.TryParse(value, out int val);
			egg_distribution[loc]= val;
		}
	}

	//#Write out species currently present
	string WriteSpeciesToString()
	{
		List<string> list = new List<string>();
		foreach (var spec in species_present.Keys)
		{
			list.Add($"{spec}:{species_present[spec]}");
		}
		return string.Join("¼",list);	
	}

	//# Read into species previous present
	void ReadSpeciesFromString(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return;
		}

		string[] list= split("/¼/",str); //#species are delimited with ¼
		foreach (var item in list)
		{
			string[] sublist = split(":",item);
			if (sublist.Length < 2)
			{
				Logger.LogError("WARNING: ReadSpeciesFromString received empty/invalid item: " + item);
				continue;
			}
			string name = sublist[0];
			string value = sublist[1]; 
			species_present_prev[name] = value;
		}
	}

	//##############################################################
	//##
	//## Encounter
	//##
	//##############################################################

	void RandomEncounter()
	{
		try
		{
			char loc = map[y][x];
			if (habitat_species_freq.ContainsKey(loc))
			{
				MultiSpeciesEncounter(habitat_species_freq[loc]);
			}
			else
			{
				Logger.Log($"WARNING: location does not have species defined: {loc}");
			}
		}
		catch (Exception EVAL_ERROR)
		{
			if (EVAL_ERROR != null && EVAL_ERROR.Message != "exit")
			{
				perlException($"Encounter Error ({EVAL_ERROR})");
			}
			else if (EVAL_ERROR.Message == "exit")
			{
				throw new Exception("exit"); //to force-quit execution
			}
		}
	}

	void MultiSpeciesEncounter(Dictionary<string,double> hr)
	{
		Dictionary<string, double> distn = new Dictionary<string, double>();
		if (hr != null) distn = hr;
		var sp= distn.Keys;

		//# If we're not moving, transfer the old species
		//# Some may randomly roam off (predators less so)
		bool moving = perlRegex(paramString("'action'"),"Move", ignoreCase: true);
		if (!moving)
		{
			string[] keys = new string[species_present_prev.Keys.Count];
			species_present_prev.Keys.CopyTo(keys, 0);
			for (int i = 0; i < species_present_prev.Keys.Count; i++)
			{
				var spec = keys[i];
				double loss_rate = roaming_rate;
				var spec_size = SizeOfSpecies(spec, species_present_prev);
				double pr = CalcFiercenessRatio(spec, spec_size * species[spec].Aggression); //#predation likelihood
				if (pr > 1) loss_rate /= log10(pr) ; //#make predators hang around more
				debug($"Previous {spec}: data={species_present_prev[spec]}, size={spec_size}, predation={pr}. loss = {loss_rate}");

				if(! (perlRegex(species_present_prev[spec] ,"RunsAway", ignoreCase: false)
					|| (rand() <loss_rate)))
				{
					//# it comes with us
					debug($"=> {spec} stays");
					Encounter(spec, SizeOfSpecies(spec, species_present_prev)); //#Pass the previously-calculated size
				}
				else
				{
					//# it leaves
					species_leaving.Add(spec);
					species_present_prev[spec] += "|Leaves";
					debug($"=> {spec} leaves");
				}
			}
		}

		//# Encounter some new species (up to a maximum number)
		//# If we sit still some may randomly roam by
		foreach (var spec in sp)
		{
			if ((moving || rand() < roaming_rate) && scalar(species_present.Keys) < max_species)
			{
				if (rand() < distn[spec]) Encounter(spec) ;
			}
		}
	}

	//#
	//# The encounter
	//#
	void Encounter(string spec, double specsize = 0)
	{
		double min_age = species[spec].MinimumAge;
		min_age = min_age > 0 ? min_age : 1;
		var spec_size = specsize > 0 ? specsize : min_age + (1 - min_age)*rand(); //#Make up how big it is, unless told

		string sp_behaviour = "";
		string al_behaviour = next_action;

		//#
		//# Decide what it does next, and how fast based on the relative strengths and a bit of chance
		//#
		double fr = CalcFiercenessRatio(spec,spec_size,behaviour_randomness);
		debug($"Encounter: fr={fr}, min age = {min_age}; spec size = {spec_size}");
		if (fr == 0 || abs(log10(fr)) > 0.5) //#If you're not roughly a match for each other
		  {
			//#compute fierceness delta (orders of magnitude)
			var max_fd = 4;  //#4om spans Al's size range
			var fd = (fr == 0 ?max_fd: abs(log10(fr)));
			if (fd >max_fd) {fd =max_fd; }

			//#convert fierceness delta to reaction time
			var t = rt_min +rt_var * ((max_fd -fd)/max_fd);

			//#Outcome depends on ratio
			if (fr < 1)
		    {
		      sp_behaviour = "RunsAway"; //#You scare it off.
			}
		    else
		    {
				//#You are the weaker one - but nothing will mess with you if mum is around
				var sp_aggression = (have_mother && x == mx && y == my) ? 0 : species[spec].Aggression;
				var luck = rand();
				debug($"Encounter {spec}: aggression={sp_aggression}; luck={luck}");
				if (luck < sp_aggression)
				{
					//#It is aggressive and attacks you
					al_behaviour = $"BeAttacked:{spec}";
					debug($"Encounter {spec}: BE ATTACKED t={t}");
				}
		      else
				{
					t = 0; //#It just ignores you
				}
			}
			if (t > 0 && (timer == 0 || timer > t))
		    {
		       timer = (int)t; //#shorten the timer
		       next_action = al_behaviour; //#set the potenitially new behaviour
			}
		}

		  //#Add the species to the list of those present (record its size and behaviour)
		  species_present[spec]= sprintf("%.3f",spec_size) + "|" + sp_behaviour;
	}

	int SizeOfSpecies(string spec, Dictionary<string,string> refHash)
	{
		if (!refHash.ContainsKey(spec)) //added
		{
			if (!string.IsNullOrEmpty(spec))
			{
				Logger.LogWarning($"SizeOfSpecies asked for spec not in dictionary: {spec}");
			}
			return 1;
		}
		string data = refHash[spec];
		string[] size = split(@"/\|/", data);
		return size.Length > 0 ? size.Length : 1;
	}

	//##############################################################
	//##
	//## Event subroutines.  Events are in the form Action:Data
	//##
	//##############################################################

	void DoAction()
	{
		try
		{
			string action = paramString("'action'");
			string[] splitresult = split(":".ToCharArray(),action);
			string[] data = new string[splitresult.Length - 1];
			action = splitresult[0];
			for (int i = 1; i < splitresult.Length; i++)
			{
				data[i - 1] = splitresult[i];
			}

			 if (perlRegex(action, "^Move", ignoreCase: true)) Move(data);
			if (perlRegex(action,"^Attack", ignoreCase: true)) Attack(data);
			if (perlRegex(action,"^BeAttacked", ignoreCase: true)) BeAttacked(data[0]);
			if (perlRegex(action,"^EatEgg", ignoreCase: true)) EatEgg();
			if (perlRegex(action,"^Drown", ignoreCase: true)) Drown();
			if (perlRegex(action,"^Sink", ignoreCase: true)) Sink();
			if (perlRegex(action,"^Wait", ignoreCase: true)) Wait();
			if (perlRegex(action,"^Mate", ignoreCase: true)) Mate(data[0]);
			if (perlRegex(action,"^Quit", ignoreCase: true)) Quit();
			if (perlRegex(action,"^Cheat", ignoreCase: true) && (DEMO > 0 || DEBUG > 0)) Cheat(data); //#No cheating in release mode!
			if (perlRegex(action,"^Popup", ignoreCase: true)) Popup(data);
			if (perlRegex(action,"^Pause", ignoreCase: true)) suspended = true;
		}
		catch (Exception EVAL_ERROR)
		{
			if (EVAL_ERROR != null && EVAL_ERROR.Message != "exit")
			{
				perlException($"Error in DoAction: {EVAL_ERROR}");
			}
			else if (EVAL_ERROR.Message == "exit")
			{
				throw new Exception("exit"); //to force-quit execution
			}
		};
	}

	void Cheat(string[] param)
	{
		var how = param[0];

		string[] data = new string[param.Length - 1];
		for (int i = 1; i < param.Length; i++)
		{
			data[i - 1] = param[i];
		}

		if (perlRegex(how, "^Level", ignoreCase: true))
		{
			double.TryParse(data[0], out double data0);
			weight = level_weight[level] + 0.1; //modified to allow one jump for each level
		}
		if (perlRegex(how,"^Recharge", ignoreCase: true))
		{
			energy = 100; fitness = 100;
		}
		if (perlRegex(how, "^Score", ignoreCase: true))
		{
			score += round(rand() * 100);
		}
		if (perlRegex(how, "^Goto", ignoreCase: true) && perlRegex(data[0],"([A-Z]) ([0-9]+)", ignoreCase: true))
		{
			string[] matches = perlRegexReturningMatches(data[0], "([A-Z]) ([0-9]+)", ignoreCase: true);
			x = ord(uc(matches[0])) - ord('A');
			int.TryParse(matches[1], out int match1int);
			y = match1int - 1;
			visited[y][x] = 1;
		}
	}
	internal string popupURL; //added
	void Popup(string[] param)
	{
		var url = fact_files_url + "/" + param[0];
		extra_desc  = "<SCRIPT LANGUAGE=JAVASCRIPT>\n";
		extra_desc += "<!--\n";
		extra_desc += $"popwin_ff('{url}','_blank');\n";
		extra_desc += "//-->\n";
		extra_desc += "</SCRIPT>\n";
		suspended = true;
		popupURL = url; //added
	}

	void Wait()
	{
		  extra_desc += "You wait.  ";
		  weight -= wait_weight_loss * metabolic_rate * weight;
		  energy -= wait_energy_loss * metabolic_rate;
		if (weight <= 0) Die(new HintRefFields() { WEIGHT = 1 });
		if (energy <= 0) Die(new HintRefFields() { ENERGY = 1 });
	}

	void EatEgg()
	{
		string loc = chr(ord('A') + x) + (y + 1).ToString();
		if (egg_distribution[loc]> 1)
		{
			extra_desc += "You eat one of the eggs.  ";
		}
		else
		{
			extra_desc += "You eat the egg.  ";
		}
		energy = 100; //#eggs recharge your energy - don't put on weight
		egg_distribution[loc]--;
	}

	void Move(string[] param = null)
	{
		string dir = param != null ? param[0] : null;
		if (x > 0 && perlRegex(dir,"W")) x--;
		if (x < nx - 1 && perlRegex(dir, "E")) x++;
		if (y > 0 && perlRegex(dir, "N")) y--;
		if (y < ny - 1 && perlRegex(dir, "S")) y++;
		weight -= move_weight_loss * metabolic_rate * weight;
		energy -= move_energy_loss * metabolic_rate;
		if (weight <= 0) Die(new HintRefFields() { WEIGHT = 1 });
		if (energy <= 0) Die(new HintRefFields() { ENERGY = 1 });
		visited[y][x] = 1;
	}

	void Attack(string[] param)
	{
		var spec = param[0];
		int.TryParse(param[1], out int pack_size);
		var str = species[spec].Attack;
		if (str == null) str = "You attack the " + HTMLForSpecies(spec);
		extra_desc += $"{str}.  ";
		AttemptToKill(spec,pack_size);
		current_pack_size = 1;
	}

	void BeAttacked(string spec)
	{
		extra_desc += "The " + HTMLForSpecies(spec) + " attacks you.  ";
		AttemptToKill(spec);
	}

	void Drown()
	{
		Die(new HintRefFields() { DROWN = true });
	}

	void Sink()
	{
		Die(new HintRefFields() { SINK = true });
	}

	void Mate(string spec)
	{
		if (spec == "Female Allosaurus")
	  {
			Courtship();
			species_present_prev.Remove(spec); //#remove her from the 'normal' species
		}
	  else
	  {
	    //#Oops - wrong species
	    extra_desc += "  The " + spec + " doesn't take kindly to your advances.  ";
			var fr = CalcFiercenessRatio(spec, 1, behaviour_randomness);
			if (fr < 1) {species_present_prev[spec]= "RunsAway"; }
	    else { BeAttacked(spec); }
		}

		//#Burn some energy
		Move();
	}


	//#############################################################
	//##
	//## Habitat-driven events
	//##
	//##############################################################

	void CheckHabitat()
	{
		if (map[y][x] == 'F' && level >= 3) InForest();
		if (map[y][x] == 'R') WadeThroughRiver();
		if (map[y][x] == 'Q') CrossQuicksand();
		if (map[y][x] == 'D') CrossDesert();
		if (map[y][x] == 'H' && level >= 3)
		{
			MoveSauropodHerd();
			DisplayHuntingAllosaurs();
		}
	}

	void InForest()
	{
		extra_desc += "The undergrowth seems much thicker than when you were younger.  It's difficult to move without knocking into something.  ";
		if (rand() < weight /max_weight * forest_injury_prob)
		{
			extra_desc += "You catch your foot under a branch and fall.  You're too big for the forest these days.  A broken toe will make catching prey much more difficult.  ";
			fitness *= 0.5;
		}
	}

	void WadeThroughRiver()
	{
		if (weight < min_weight_for_river)
		{
			Die(new HintRefFields() { DROWN = true });
		}
		else
		{
		    extra_desc += "You wade through the river.  The current is quite strong...  ";
		    move_energy_loss *= 2; //#harder work moving through water
		    wait_energy_loss *= 2;
		    next_action = "Drown";
			if (!(timer > 0)) timer = 11 ;
		}
	}

	void CrossQuicksand()
	{
		if (weight >max_weight_for_quicksand)
		{
			Die(new HintRefFields() { SINK = true });
		}
		else
		{
			extra_desc += "It suddenly dawns on you that what you are crossing is quicksand - you break into a run...  ";
			next_action = "Sink";
			if(!(timer > 0)) timer = 12 ;
		}
	}

	void CrossDesert()
	{
	   //#harder work moving through hot desert
	   move_energy_loss *= 2;
	   wait_energy_loss *= 2;
	}


	//##############################################################
	//##
	//## Combat
	//##
	//##############################################################

	void AttemptToKill(string spec, int pack_size = 0)
	{
		var old_fitness = fitness; //#cache for die
		var ar = CalcAgilityRatio(spec,behaviour_randomness);
		var spec_size = SizeOfSpecies(spec, species_present_prev);
		var fr = CalcFiercenessRatio(spec,spec_size,behaviour_randomness,pack_size);
		debug($"Attack {spec}: size={spec_size}; fr={fr}; ar={ar}; pack={pack_size}");

		//#Outcomes:
		if (fr < 1)
		{
			if (ar < 1)
			{
				//#You kill it
				if (CalcInjuriesFromFighting(fr) > 10) extra_desc += "There is a struggle and you are injured.  " ;
				EatSpecies(spec,spec_size,pack_size); //#You win the fight
			}
			else
			{
			  //#It gets away
			  extra_desc += $"The {spec} was too fast for you - it escaped before you could kill it.  \n";
				Move(); //#Simulate energy loss by moving
			}
		}
		else
		{
			//#You lose the fight
			CalcInjuriesFromFighting(fr);
			ChasedAwayBy(spec);
		}
		if (fitness <= 0) Die(new HintRefFields() { SPECIES = spec, FR = fr, FITNESS = old_fitness });
	}

	void EatSpecies(string spec, int spec_size, int pack_size)
	{
		extra_desc += "You kill and eat the " + HTMLForSpecies(spec) + ".  ";
		Eat(species[spec].Nutrition * spec_size, pack_size);
	  debug($"Eating {spec} of size {spec_size}");
		species_present_prev.Remove(spec); //#no longer present!
	}

	void Eat(double nutrition_value, int pack_size)
	{
		//#nutritional value: absolute scale - in kilos
		//# pack_size shared amongst...
		if (pack_size == 0) pack_size =current_pack_size;

		//#Convert weight units to energy units
		var energy_value = nutrition_value /pack_size /weight * 100;
		energy_value *= energy_per_bodyweight * (Math.Pow(metabolic_rate, nutrition_metabolism_exponent));

		if (energy_value > 100 -energy)
	  {
			//#If the energy more than tops us up, put on some weight
			var energy_surplus = energy_value - (100 -energy);
			var weight_value = 0.01 * energy_surplus / energy_per_bodyweight * weight; //#convert energy back to weight
	    weight += weight_value;
			if (weight > max_weight) weight = max_weight ; //#don't put on any more weight if at max_weight
	    score += round(energy_per_bodyweight * weight_value / weight * 10);
	    energy = 100;
		}
	  else
	  {
	    energy += energy_value;
		}
	}

	void ChasedAwayBy(string spec)
	{
		extra_desc += $"  The {spec} " + species[spec].Injury + " and disappears before you can recover.  ";
		Move(); //#simulate energy loss through moving
		species_present.Remove(spec);
	}

	double CalcFiercenessRatio(string spec, double spec_size, double randomness = 0, int pack_size = -1) //#Calculate your chance of killing another species
	{
		if (pack_size == -1) pack_size = current_pack_size;
		if (pack_size == 0) pack_size = 1; //added: safety check to avoid 0 pack size
		var chance = (rand() * 2 - 1) * randomness + 1;
		var log_fitness = fitness > 0 ? log10(fitness) : 0;
		var fierceness = weight * (log_fitness / 2); //#in weight units
		double delta = 1e6;
		if (fierceness > 0)
		{
			delta = species[spec].Danger
			* (spec_size / pack_size) * (chance / fierceness);
		}
		//#debug("Calc FR: Fierceness of {spec}: size={spec_size}, chance={chance}, pack={pack_size}, ratio={delta}");
		return delta; //#ratio: it to me
	}

	double CalcAgilityRatio(string spec, double randomness = 0) //#Calculate your chance of catching another species
	{
		double chance = (rand() * 2 - 1) *randomness + 1;
		double agility = 1 - Math.Pow(((log10(weight) - log10(0.2)) / 5), 2); //#1 => 0
		double delta = species[spec].Difficulty *chance /agility;
		return delta; //#ratio: it to me
	}

	double CalcInjuriesFromFighting(double delta, double severity = 0) //#Calculate loss of fitness
	{
		severity = severity == 0 ? 1 : severity;
		var injuries = delta * 40; //#something 2.5x as fierce can kill you
		debug($"[injuries={injuries}]");
	  fitness -= injuries / severity;
		return injuries / severity;
	}

	//##############################################################
	//##
	//## Sauropod hunting
	//##
	//##############################################################

	//# Update the number of allosaurs hunting the sauropods
	void CalcHuntingAllosaurs()
	{
		int.TryParse(decision, out int decisionint);
		//#Don't need this if we go it alone
		if (decisionint == 1) return ;

		if (level_data == decision)
		{
			//#Others all leave you
			level_data = "0"; //#sentinel value
			if(!(timer > 0))timer = 14 ;
		}
		else
		{
			//#Make the pack size build up to the value you chose
			var deltaDouble = rand() * 2.49 - 1;
			var delta = round(deltaDouble);
			var others = current_pack_size - 1 + delta;
			if (others < 0)others = 0 ;
			if (others > (decisionint - 1)) others = (decisionint - 1) ;
			level_data = (others + 1).ToString();
			debug($"delta = {delta}; new pack size = {level_data}");
		}
	}

	//# Display the number of hunting Allosaurs if we're near the sauropod herd
	void DisplayHuntingAllosaurs()
	{
		int.TryParse(decision, out int decisionint);
		debug($"current pack size = {current_pack_size}; level_data={level_data}");

		//#Respond to sentinel value - make others disappear
		if (level_data == "0")
	  {
			if (decisionint > 1) extra_desc += "The other allosaurs have disappeared, they're stalking the sauropods elsewhere.  " ;
	    level_data = "1"; current_pack_size = 1;
		}

		//#Display what's here
		if (current_pack_size > 1) extra_desc += "There's " + (current_pack_size - 1) + " other <I>Allosaurus</I> here, waiting for enough to form a pack and attack the sauropods.  " ;
	}

	void MoveSauropodHerd()
	{
//# Meet?
		bool meet = (x ==mx && y ==my);

		//#Description
		if (meet)
		{
		   extra_desc += "A herd of sauropods is migrating through here now.  ";
			if (current_pack_size.ToString() == decision)
			{
				var spec = "Diplodocus";
			  extra_desc += ActionImgButton("m_attack", $"Attack the {spec}", $"Attack:{spec}:{current_pack_size}", "43");
			}
		}

//# New position
		if (rand() < sauropod_move_prob)
		{
			mx = 11 + round(rand());
			my = round(rand());
		}

		if (meet)
		{
//# compute direction
			var dx = (mx - x);
			var dy = (my - y);
			var dir = compass[dy + 1][dx + 1];
			debug($"mx = {mx}, my = {my}, dx = {dx}; dy = {dy}; dir={dir}");
			if (!(dx == 0 && dy == 0)) extra_desc += $"The herd moves off to the {dir}.  ";
		}
	}

	//##############################################################
	//##
	//## Mother
	//##
	//##############################################################

	void MeetMother()
	{
		int.TryParse(level_data, out int leveldataint);

		if (x ==old_mx && y ==old_my)
		{
			extra_desc += "Your mother is here.  ";

			//#Have we been separated too long?
			if (leveldataint > mother_memory) Die(new HintRefFields() { MOTHER = true });
			level_data = "0"; //#zero time from mother
		}
		else
		{
			level_data = (++leveldataint).ToString(); //#increase time from mother
		}
	}

	string ProtectedByMother()
	{
		if (x ==old_mx && y ==old_my)
		{
			//#Determine if there are any potential predators
			var fierce = 0;
			foreach (var spec in species_present.Keys)
	 		{
				var size = SizeOfSpecies(spec, species_present);
				var fr = CalcFiercenessRatio(spec, size) * species[spec].Aggression;
				debug($"Mother vs {spec}: size={size}; fr={fr}");
				if (fr > 2)
				{
					fierce = 1;
					break;
				}
			}
			if (fierce != 0) return "You're safe whilst your mother's around.  ";
			return "";
		}
		return null;
	}

	void MoveMother()
	{
		//#Compute Move
		var dmx = 0;
		var dmy = 0;
		if (rand() < mother_move_prob)
		{
			dmx = round(rand() * ((mx < 3) ? 2 : 1) - ((mx > 0) ? 1 : 0));
			dmy = round(rand() * ((my < ny - 1) ? 2 : 1) - ((my > 0) ? 1 : 0));
			if (!(timer != 0 || (dmx == 0 && dmy == 0))) timer = 10 ; //#Add a time for the mother to leave
		}

		//#If mother has moved from this square then hint
		if (x ==mx && y ==my && (dmx != 0 || dmy != 0))
		{
			extra_desc += $"Your mother begins to move off to the {compass[dmy+1][dmx+1]}.  ";
		}
		else
		{
			debug($"Mother's next move: {compass[dmy+1][dmx+1]}.");
		}

		//#Apply move
		mx += dmx;
		my += dmy;
	}

	//#############################################################
	//##
	//## Mating subroutines
	//##
	//##############################################################

	void Courtship()
	{
		//#Did you do the right thing?
		string female_response = null;
		double severity = InvalidCourtShipMove(ref female_response);
		if (severity != 0)
		{
			CourtshipFailed(female_response,severity);
		}
		else
		{
			if (level_data == "Ready to Mate" && decision == "Mount")
			{
				if (IsMatingSuccessful())
				{
					SuccessfulMating();
				}
				else
				{
					CourtshipFailed("You attempt to mount the female, but you're inexperienced and it all goes wrong.", 1);
				}
				matings++; //#increment our mating experience regardless of whether we succeed
			}
			else
			{
				NextCourtshipRound(female_response);
			}
		}
	}

	void NextFemaleState(ref string desc)
	{
		//#Initial state
		if (string.IsNullOrEmpty(level_data))
		{
			level_data = "Call";
			desc = "The female eyes you cautiously.";
		}

		//#Second state
		else if(level_data == "Call" && decision == "Rumble")
		{
			level_data = "Either";
			desc = "The female moves in for a closer look.";
		}
		else if(decision == "Wait" && level_data == "Either")
		{
			level_data = "Call";
			desc = "The female starts to lose interest.";
		}

		//#Third state
		else if(level_data == "Either" && decision == "Side")
		{
			level_data = "Circle";
			desc = "The female returns your display.";
		}
		else if(level_data == "Either" && decision == "Circle")
		{
			level_data = "Side";
			desc = "The female returns your display.";
		}
		else if(decision == "Wait" && (level_data == "Circle" || level_data == "Side"))
		{
			level_data = "Either";
			desc = "The female starts to lose interest.";
		}

		//#Final State
		else if(level_data == decision || level_data == "AnswerQuestion")
		{
			if (matings > 2 
				&& level_data != "AnswerQuestion")
	  		{
	  			level_data = "AnswerQuestion";
	  			desc = "The female turns round and asks you a question:";
			}
	  		else
	  		{
	      		level_data = "Ready to Mate";
	      		desc = "The female leans forward and lifts her tail.";
			}
		}
	}

	int InvalidCourtShipMove(ref string refDesc)
	{
		//var refDesc = _[0]; //#pass-by-reference

		//#Return value: 0=OK; 1=most severe attack, >1 decreasingly severe attack

		//#Waiting at the wrong time
		if (level_data == "Call" && decision == "Wait")
		{
			refDesc = "Since you do not acknowledge her, the female doesn't recognise you as a mate.";
			return 1;
		}

		if (level_data == "Ready to Mate" && decision == "Wait")
		{
			refDesc = "The female is ready to mate, but you're hanging about. She's getting frustrated.";
			return 2;
		}

		//#Wrong move
		if ((decision == "Side" && level_data == "Circle") || (decision == "Circle" && level_data == "Side"))
		{
			refDesc = "That wasn't the display she was looking for.";
			return 3;
		}

		//#Wrong answer to question
		if (level_data == "AnswerQuestion" && !(CorrectAnswer(paramInt("'number'"), decision, out refDesc) != 0))
		{
			return 1;
		}

		//#Ready to mate?
		if (decision == "Mount" && level_data != "Ready to Mate")
		{
			refDesc = "You attempt to mount the female, but she's not ready to mate.";
			return 1;
		}

		//#Stupid moves you get a kicking for
		if (decision == "Squeak")
		{
			refDesc = "You make a high-pitch squeaking sound. Unfortunately <I>Allosaurus</I> can't hear high frequencies.";
			return 2;
		}

		if (decision == "Display")
		{
			refDesc = "You move in front of her to display. Unfortunately <I>Allosaurus</I> can't see directly in front and this makes her nervous.";
			return 2;
		}

		//#default outcome - she doesn't attack you
		return 0;
	}

	void NextCourtshipRound(string female_response)
	{
	     //#Describe the response of the female to your last advance
	     extra_desc += female_response;

		//#Describe the behaviour of the female now
		string desc = null;
		NextFemaleState(ref desc);
	     extra_desc += desc;

	     //#Ask for a response
	     extra_desc += FileToStr($"{files_dir}/mating.ssi");
		if (level_data == "Call")
	     {
	   	  extra_desc += "  What now?";
	        extra_desc += FileToStr($"{files_dir}/mating_calls.ssi");
		}
		else if (level_data == "AnswerQuestion")

		 {
	     	  extra_desc += AskMatingQuestion();
		}
	     else
	     {
		     extra_desc += "  What now?";
	        extra_desc += FileToStr($"{files_dir}/mating_moves.ssi");
		}
	     next_action = paramString("'action'");
	}

	void CourtshipFailed(string female_response, double severity)
	{
	   //#Describe why you're about to get a kicking
	   extra_desc += female_response;

		//#Get beaten up
		var spec = "Female Allosaurus";
		var fr = CalcFiercenessRatio(spec, 1,behaviour_randomness,current_pack_size);
		CalcInjuriesFromFighting(fr,severity);

		//#Either get chased away or killed
		if (fitness < 0) Die(new HintRefFields() { SPECIES = spec, FITNESS = 1, MATING = female_response }); //added SPECIES
		ChasedAwayBy(spec);

	   decision = ""; level_data = ""; //#reset mating parameters
	}

	bool IsMatingSuccessful()
	{
		if (rand() < mating_failure / (matings + 1)) return false ;
	   return true;
	}

	void SuccessfulMating()
	{
	   extra_desc += "<P>You mate with the female and make your exit before she gets hungry.";
		var spec = "Female Allosaurus";
		var fr = CalcFiercenessRatio(spec, 1,behaviour_randomness,current_pack_size);
	   score += mating_bonus;      //#bonus now independent of fitness
	   decision = ""; level_data = ""; //#reset mating parameters
	}
	internal string matingQNumber;//added
	internal string matingQuestion;//added
	internal string[] matingAnswers = new string[3];//added
	string AskMatingQuestion()
	{
		string[] qDetails = GetRandomQuestion();
		string number = qDetails[0]; string question = qDetails[1];
		string a = qDetails[2];	string b = qDetails[3];	string c = qDetails[4];
		matingQNumber = number; //added
		matingQuestion = question; //added
		matingAnswers[0] = a; matingAnswers[1] = b; matingAnswers[2] = c; //added
		return $"  {question}<BR>\n" +

			  $"<LI><A HREF=\"javascript:decision('A');\">{a}</A>\n" +

			  $"<LI><A HREF=\"javascript:decision('B');\">{b}</A>\n" +

			  $"<LI><A HREF=\"javascript:decision('C');\">{c}</A>\n" +

			  $"<INPUT TYPE='HIDDEN' NAME='number' VALUE='{number}'>\n";
	}

	int CorrectAnswer(int number, string answer, out string refDesc)
	{
		string[] list = null;
		GetQuestions(ref list);
		string[] splits = split("\t", list[number]);
		string ques = splits[0];
		string a = splits[1];
		string b = splits[2];
		string c = splits[3];
		string correct = splits[4];
		string reason = splits[5];
		if (answer == correct)
	   {
	   	 refDesc = "Correct!  ";
		if (reason != null) refDesc += $"{reason}  ";
			return 1;
		}
	   else
	   {
	   	refDesc = "Wrong Answer!  ";
			//#refDesc = $"The correct answer is {correct}";
			//# if(reason) refDesc += $" because {reason}";
			//#refDesc += ".  ";
			return 0;
		}
	}

	//#Pick a question at random
	string[] GetRandomQuestion()
	{
		string[] list = new string[] { };
		GetQuestions(ref list);
		var number = round(rand()*(scalar(list) - 1) - 0.5); //added - 1 to exclude empty entry at end (EOF CRLF counts as newline in C#, so filetext split adds an extra row)
		List<string> listStrings = new List<string>() { number.ToString() };
		listStrings.AddRange(split("\t", list[number]));
		return listStrings.ToArray();
	}

	//#Load questions from file
	void GetQuestions(ref string[] refList)
	{
		localFile LI;
		if (!open(out LI, questions_file)) perlException("Unable to load mating questions");
		refList = LI.fileText;
		shift(ref refList); //#Headers
		close(LI);
	}

	//##############################################################
	//##
	//## Output Subroutines
	//##
	//##############################################################

	void WriteHeader(int start_level = 0)
	{
		//# Sort out cookie logistics
		move_number++;
		
		try
		{
			PerlCookie cookieObj = new PerlCookie() { name = "state", value = WriteStateToString(), expires = cookie_lifetime, domain = cookie_domain, path = cookie_path };
			PerlCookie backup_cookie = perlCookie.Length > 1 ? perlCookie[1] : null;
			if (start_level != 0)
			{
				backup_cookie = new PerlCookie() { name = "begin_level", value = WriteStateToString(), expires = cookie_lifetime, domain = cookie_domain, path = cookie_path };
			}
			perlCookie = new PerlCookie[] { cookieObj, backup_cookie };
		}
		catch (Exception e)
		{
			Logger.LogError("ERROR: Exception writing cookie: " + e.Message + " || " + e.StackTrace);
		};

		//# Get display parameters
		var bg = $"{images_url}/{level_colourscheme[level].BG}";
		var bg_color =level_colourscheme[level].BGCOLOR;
		var text_color = "white";
		var link_color = level_colourscheme[level].FGCOLOR;
		var real_time = round(timer * time_stretch);
		realtime = real_time; //added

		//#Begin Output
		pprint(CGI.header(cookie: perlCookie, expires: "-1s"));
		pprint("<HTML>\n");
		pprint("<HEAD>\n");
		pprint("<TITLE>BBC Online - Walking With Dinosaurs - Big Al Game</TITLE>");
		if (timer != 0) pprint($"<META HTTP-EQUIV=REFRESH CONTENT='{real_time};url={own_filename}?move_number={move_number}&action=" + CGI.escape(next_action) + "'>\n");
		if (timer != 0) timeExpiredAction = CGI.escape(next_action); else timeExpiredAction = ""; //added
		pprint("<STYLE TYPE='text/css'>\n");
		pprint("A {text-decoration: none};\n");
		pprint("</STYLE>");
		pprint("<SCRIPT LANGUAGE='JAVASCRIPT'>\n<!--\n");
		pprint(FileToStr(js_file));
		pprint("//-->\n</SCRIPT>\n");
		pprint("</HEAD>\n");
		pprint($"<BODY MARGINHEIGHT=0 MARGINWIDTH=0 TOPMARGIN=0 LEFTMARGIN=0 BACKGROUND='{bg}' BGCOLOR='{bg_color}' TEXT='{text_color}' LINK='{link_color}' ALINK='red' VLINK='{link_color}' Onload='" + GenerateOnload() + "'>\n");
		pprint($"<FORM ACTION='{own_filename}' METHOD=POST>\n");
		pprint($"<INPUT TYPE=HIDDEN NAME=action VALUE='{next_action}'>\n");
		pprint($"<INPUT TYPE=HIDDEN NAME=move_number VALUE='{move_number}'>\n");
	}
	internal int realtime; //added
	internal string timeExpiredAction; //added

	void WriteFooter()
	{
		//#Testing buttons
		if (DEBUG != 0 || DEMO != 0)
		{
			pprint("<BR CLEAR=ALL><TABLE><TR><TD WIDTH=600 ALIGN=CENTER>");
			pprint(BeginActionButtons());
			if (!(level == scalar(levels)) && level >= 0) pprint(ActionButton("Cheat: Next Level",$"Cheat:Level:{level_weight[level]}")) ;
			pprint(ActionButton("Cheat: Recharge","Cheat:Recharge"));
			pprint(ActionButton("Cheat: Score","Cheat:Score"));
			pprint(EndActionButtons());
			pprint("</TD></TR></TABLE>");
		}

		//#Debug output
		if (DEBUG != 0)
		{
			pprint("<BR CLEAR=ALL><HR><SMALL><CENTER>DEBUG INFO</CENTER><BR>STATE: " + WriteStateToDebug() + "<BR>\n");
			pprint("ACTION: " + paramString("'action'") + $" => {next_action}</SMALL><BR>\n");
			if (timer != 0) pprint($"<SMALL>TIME: {timer}; Stretch = {time_stretch}</SMALL><BR>\n");
			pprint($"{debug_str}<HR></P>\n");
		}

		pprint("</FORM>\n");
		pprint("</BODY>\n");
		pprint("</HTML>\n");
	}

	//#Write out the page content into the template
	void WriteContent()
 {
		PageContent fields = new PageContent()
		{
			BGColor = level_colourscheme[level].BGCOLOR,
			FGColor = level_colourscheme[level].FGCOLOR,
			HLColor = level_colourscheme[level].HLCOLOR,
			LLColor = level_colourscheme[level].LLCOLOR,
			Status = DrawStatus(),
			Map = DrawMap(),
			Key = GenerateHint(),
			Compass = DrawCompass(),
			Description = GenerateDescription(),
			View = GenerateView(),
			Species = "&nbsp;",
			ImagesURL = images_url
		  };

		if (species_present != null && species_present.Count > 0) fields.Species = DisplaySpeciesPresent();
		AddStatusToHash(ref fields);

		var parser = new TemplateParser_pm();
		  parser.input_file($"{files_dir}/templates/normal.html");
		  parser.fields(fields);
		  parser.parserprint();
	}

	//#Similar, but no compass and no description
	void WriteSuspended()
	{
		PageContent fields = new PageContent()
		{
			BGColor = level_colourscheme[level].BGCOLOR,
			FGColor = level_colourscheme[level].FGCOLOR,
			HLColor = level_colourscheme[level].HLCOLOR,
			LLColor = level_colourscheme[level].LLCOLOR,
			Status = DrawStatus(),
			Map = DrawMap(),
			Description = GenerateSuspendedDescription(),
			View = GenerateView(),
			Compass = "",
			Key = GenerateMapKey(),
			Species = "",
			ImagesURL = images_url
		};

		if (species_present != null && species_present.Count > 0) fields.Species = DisplaySpeciesPresent()  ;
		AddStatusToHash(ref fields);

		var parser = new TemplateParser_pm();
		parser.input_file($"{files_dir}/templates/normal.html");
		parser.fields(fields);
		parser.parserprint();
	}

	//#Populate the template for a new level
	void WriteNextLevel()
	{
		PageContent fields = new PageContent()
		{
			BGColor = level_colourscheme[level].BGCOLOR,
			FGColor = level_colourscheme[level].FGCOLOR,
			HLColor = level_colourscheme[level].HLCOLOR,
			LLColor = level_colourscheme[level].LLCOLOR,
			Status = DrawStatus(),
			Map = DrawMap(),
			Level = level,
			View = GenerateView(),
			Compass = "",
			Key = HintForLevelChange(),
			Description = GenerateDescription(),
			Species = LevelText(),
			ImagesURL = images_url
		};
		AddStatusToHash(ref fields);

		var parser = new TemplateParser_pm();
		parser.input_file($"{files_dir}/templates/normal.html");
		parser.fields(fields);
		parser.parserprint();
	}
	internal string gameoverDesc; //added
	//#Populate the "Game Over" template
	void WriteGameOver(string desc)
	{
		gameoverDesc = desc + $"\nYou scored {score} points.";
		PageContent fields = new PageContent()
		{
			BGColor = level_colourscheme[level].BGCOLOR,
	     FGColor = level_colourscheme[level].FGCOLOR,
	     HLColor = level_colourscheme[level].HLCOLOR,
	     LLColor = level_colourscheme[level].LLCOLOR,
	     Text = desc,
	     Score = score,
	     ImagesURL = images_url,
	     ScoresURL = scores_url
	   };
		AddStatusToHash(ref fields);

		var parser = new TemplateParser_pm();
	  parser.input_file($"{files_dir}/templates/gameover.html");
	  parser.fields(fields);
	  parser.parserprint();
	}

	//#Populate the "Quit" template
	void WriteQuit()
	{
		PageContent fields = new PageContent()
		{
			BGColor = level_colourscheme[level].BGCOLOR,
			FGColor = level_colourscheme[level].FGCOLOR,
			HLColor = level_colourscheme[level].HLCOLOR,
			LLColor = level_colourscheme[level].LLCOLOR,
			ImagesURL = images_url
		};

		var parser = new TemplateParser_pm();
	  parser.input_file($"{files_dir}/templates/quit.html");
	  parser.fields(fields);
	  parser.parserprint();
	}

	//##############################################################
	//# Text Generation Subroutines
	//##############################################################

	string GenerateOnload()
 {
		List<string> list = new List<string>();
		foreach (var line in compass_images)
 	   {
			foreach (var loc in line)
	   	{
				if (loc != null)
				{
					list.Add($"{compass_images_url}/level_{level}/{loc}_on.gif");
					list.Add($"{compass_images_url}/level_{level}/wait_on_{loc}.gif");
				}
			}
		}
		return "preload_images(\"" + join("\",\"",list.ToArray()) + "\");";
	}
	
	string GenerateGameOverText(HintRefFields hintref)
	{
		HintRefFields hints = hintref;
		var spec_html = HTMLForSpecies(hints.SPECIES);
		string die_desc = "";
		if (hints.SPECIES != null)
		{
			if (perlRegex(paramString("'action'"),"^attack", ignoreCase: true)) {die_desc += $"You attacked the {spec_html} but it was too strong and killed you"; }
			else { die_desc += $"The {spec_html} attacked and killed you"; }
			if (hints.FR > 2)
			{
				die_desc += sprintf(", probably because it was %.0f times more fierce than you.  Your fierceness increases as you get bigger. ",round_to_sf(hints.FR,1));
				if (perlRegex(paramString("'action'"),"^attack", ignoreCase: true)) { die_desc += "Try being a bit less ambitious with your prey."; }
				else { die_desc += "Try sticking to safer habitats until you are more of a match for the predators."; }
			}
	    else
	    {
			if (hints.FITNESS != 0 && hints.FITNESS< 30) {die_desc += ", probably because your fitness was very low. Your fitness gradually increases as your wounds heal.  Try avoiding large prey if you get injured."; }
			else { die_desc += "."; }
		}
	}
	else
	{
		if (hints.WEIGHT != 0 || hints.ENERGY != 0) die_desc += "You starved to death.";
		if (hints.DROWN) die_desc += "You wade into the river, but the current is too strong and sweeps you away.";
		if (hints.SINK) die_desc += "You run over the quicksand, but you are too heavy and you sink into oblivion.";
		if (hints.MOTHER) die_desc += "Your mother didn't recognise you as her own and ate you.";
		if (hints.MATING != null) die_desc += hints.MATING + $"  The wounds from the {spec_html} were fatal.  You can't afford to make mistakes in courtship.";
		if (hints.ERROR != null) die_desc += $"A large asteroid hits the sea nearby and you are drowned by a tidal wave. <!-- Error was: {hints.ERROR} -->";
	}
	return die_desc;
}

	string LevelText()
 {

		//#DMY 2000-12-22 finding var level is secure isn't easy, it's used all over, it
		//#appears to be just a \d+ but at one point it's fed in a (list,form) from @list
		//# where I gave up looking to add this:

		//if (!perlRegex(level.ToString(), @"^\d +$")) perlException(); //commented: returns false, but type is enforced in c# anyway

		return PadInTable(
				 FileToStr($"{files_dir}/level_{level}.html") +
				 "<P><CENTER>" +
				   ActionImgButton("playlevel", $"Play level {level}", "StartLevel", "115", 30) + 
				 "</CENTER>"
			   );
	}

	string GenerateView()
	{
		return $"<IMG BORDER=0 ALIGN=LEFT WIDTH=238 HEIGHT=158 VSPACE=0 HSPACE=0 SRC='{location_images_url}/" + LoadStringForLocation(img_file) + "' ALT=''>";
	}
	internal string currentDescription = ""; //added
	string GenerateDescription()
	{
		var str = "<FONT FACE='Arial,Helvetica,Helv' SIZE=2 style=\"font-size: 8pt;\">";

		//#Describe where we are, unless we've already been told...
		if (perlRegex(paramString("'action'"),"^(Move|StartLevel)", ignoreCase: true))
		{
			str += LoadStringForLocation(desc_file);
			if (!perlRegex(str, @".*[\.\?\!]$")) str += "." ; //#only add a full stop if required
			str += "\n";
		}

	  //#Add information specific to this move
	  str += $"{extra_desc}\n";

		//#Give a run-down of what else is here, unless we're mating or the level is changing
		if(!perlRegex(paramString("'action'"),"^(Mate|NextLevel)", ignoreCase: true))
		{
	    str += ListSpeciesLeaving() +
				ListSpeciesPresent();
			if (have_mother) str += ProtectedByMother();
	    str += ListEggsPresent();
		}
		currentDescription = str + "</FONT>\n";
		return str + "</FONT>\n";
	}

	string GenerateSuspendedDescription()
	{
		extra_desc += "<CENTER><H3>GAME SUSPENDED</H3>\n";
		extra_desc += ActionImgButton("resume", "Resume the suspended game", "Resume");
		return $"{extra_desc}\n"; //added
	}

	string GenerateMapKey()
	{
		var parser = new TemplateParser_pm();
		parser.input_file($"{files_dir}/map_key.ssi");
		parser.fields(new PageContent() { ImagesURL = images_url, MapImagesURL = map_images_url });
		return parser.to_string();
	}

	string ListEggsPresent()
	{
		var loc = chr(ord('A') +x).ToString() + (y + 1);
		string str = ""; string intact = "";

		//added
		if (!egg_distribution.ContainsKey(loc))
		{
			return str;
		}

		if (egg_distribution[loc] < initial_egg_distribution[loc])
		{
			if (perlRegex(paramString("'action'"),"(Move)", ignoreCase: true)) str += "The nest has been raided - the ground is scattered with broken eggshell.  " ;
			intact = "undamaged ";
		}
		if (egg_distribution[loc]> 1)
		{
			str += $"There are {egg_distribution[loc]} {intact}eggs here.  " + ActionImgButton("eategg", "Eat one of the eggs", "EatEgg");
		}
		else if (egg_distribution[loc] == 1)
		{
			str += $"There is an {intact}egg here.  " + ActionImgButton("eategg", "Eat the egg", "EatEgg");
		}
		return str;
	}

	//# Text description of the species present
	string ListSpeciesPresent()
	{
		string desc = "";
		var list = species_present.Keys;
		int i = -1;
		foreach (var key in list) //(var i = 0; i < scalar(list); i++)
		{
			i++;
			var spec = key;
			var entry = HTMLForSpecies(spec);

			//#Select one of the entry quotes
			var e =species[spec].Enter;
			if (e != null && e.Length > 1)
			{
				var j = (int)(rand() * scalar(e));     //#pick one from the list at random
				if (e[j] != null) entry += " " + e[j]; //#allow for blank entries
			}
			else if (e != null)
			{
				entry += $" {e}"; //#single entry
			}

			//#Generate the string for this species
			if (species_present_prev.ContainsKey(spec) && //added: allow for spec not there previously
				(perlRegex(species_present_prev[spec],"RunsAway") || perlRegex(species_present_prev[spec],"Leaves")))
			{
				//#This is a new one
				var new_size = DescribeSize(SizeOfSpecies(spec, species_present));
				var old_size = DescribeSize(SizeOfSpecies(spec, species_present_prev));
				if (new_size == old_size) entry = new_size + entry;
				entry = $"another {entry}";
			}
			else
			{
				entry = DescribeSize(SizeOfSpecies(spec, species_present)) + entry;
				if (species_present_prev.ContainsKey(spec) && species_present_prev[spec] != null)
				{
					//#This one's from before
					entry = $"the {entry}";
				}
				else
				{
					entry = a_or_an(entry);
				}
			}

			//#Join onto the description
			if (i > 0 && i < scalar(list) - 1) desc += ", " ;
			if (i > 0 && i == scalar(list) - 1) desc += " and " ;
			desc += entry;
		}
		if (list.Count > 0) return $"You can see {desc}.  " ;
		return ""; //#if nothing is here
	}

	string HTMLForSpecies(string spec)
	{
		if(spec == null)
		{
			Logger.LogError("WARNING: NULL SPEC IN HTMLForSpecies");
			return "";
		}
		else if (!species.ContainsKey(spec))
		{
			Logger.LogError($"WARNING: UNKNOWN SPEC {spec} IN HTMLForSpecies");
			return "";
		}

		//#Link to fact file if it has one
		var html = species[spec].HTML != null ? species[spec].HTML : spec;
		//if (species[spec].URL != null) html = $"<A HREF='javascript:do_action(\"Popup:{species[spec].URL}\")'>{html}</A>" ; //Commented: defunct, BBC pages are offline
		return html;
	}

	string DescribeSize(int size)
	{
		double smallest = 1;
		string desc = ""; //#Default is Adult -> no description
		foreach (var key in age_ranges.Keys)
  		{
			var ub = age_ranges[key];
			//#Find minimum ub which is greater than size
			if (ub > size && ub < smallest)
	    {
	      smallest = ub;
	      desc = $"{key} ";
			}
		}
		return desc;
	}

	string ListSpeciesLeaving()
	{
		var list = species_leaving;
		if (list.Count == 0) return null;

		string desc = "The ";
		for (var i = 0; i < scalar(list); i++)
		{
			if (i > 0 && i < scalar(list) - 1) desc += ", " ;
			if (i > 0 && i == scalar(list) - 1) desc += " and " ;
			desc += HTMLForSpecies(list[i]);
		}
		desc += (scalar(list) > 1 ? " have" : " has") + " gone now.";
		return $"{desc} ";
	}

	string DisplaySpeciesPresent()
	{
		string str = "<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0>";
		var count = 0;
		foreach (var spec in species_present.Keys)
  		{
			count++;
			if (count % species_cols == 1 || species_cols == 1) str += "<TR>" ;
			str += "<TD>" + SpeciesInfo(spec) + "</TD>"; //#picture & fact file
			if (count % species_cols > 0) str += "<TD WIDTH=10><FONT SIZE=1>&nbsp;</FONT></TD>";
			if (count % species_cols == 0) str += "</TR>";
		}
		return str += "</TABLE>";
	}
	internal string currentLocationImage; //added
	string LoadStringForLocation(string file)
		{
		//#create key from x & y => A1, B2 etc.
		string key = chr(ord('A') + x).ToString() + (y + 1);
		//#search file looking for key
		localFile LI;
		open(out LI, file);
		string line;
		int debugSafety = 0;
		while ((line = LI.getLine()) != null && debugSafety < 9999)
		{
			LI.chomp();
			//if (false /* /^$/ */) continue; //Commented: unnecessary
			if (!line.Contains(":")) continue;
			string[] splitstring = split(":", line);
			string fkey = splitstring[0]; string desc = splitstring[1];
			if (fkey == key)
			{
				if (file == img_file)
				{
					currentLocationImage = desc;
				}
				return desc;
			}
			debugSafety++;
		}
		if (debugSafety == 9999) Logger.LogError("WARNING: INFINITE WHILE BROKEN");
		close(LI);
		return "";
}

	string GenerateHint()
 {
		string hint = null;
		var h = map[y][x];

		if (level == 1 && move_number < 5) hint = "Eat to grow - grow to move on to Level 2" ;
		if ((h == 'E' || h == 'P') && level == 1 && rand() < 0.5) hint = "Beware the dangers of the plains!" ;
		if (move_number == 1) hint = "Beware animals with a red 'fierceness' warning" ;

		if (level >= 3 && h == 'H')
		{
			double.TryParse(decision, out double decisionnum);
			if (current_pack_size < decisionnum) hint = "You can't attack until your pack is big enough" ;
			if (x != mx && y != my) hint = "You need a pack and the herd to make a kill";
		}
		else if (level == 3 && h != 'H')
		{
			if (rand() < 0.3) hint = "Find the sauropod migration area";
		}

		if (h == 'B' && level < 3 && rand() < 0.5) hint = "Are you big enough to brave the current?" ;
		if (energy < 60) hint = "You need to eat something soon";
		if (energy < 35) hint = "You <I>really</I> need to eat something soon";

		currentHint = ""; //added
		if (hint != null) currentHint = "Hint:\n" + hint; //added
		if (hint != null) return PadInTable("<FONT FACE='Arial,Helvetica,Helv' SIZE=2 style=\"font-size: 8pt;\">Hint:<BR>" + hint + "</FONT>");
		return GenerateMapKey();
	}
	internal string currentHint; //added
	string HintForLevelChange()
	{
		string hint = null;
		if (level == 1) hint = "Mother protects you - as long as she remembers you're not lunch";
		if (level == 2) hint = "Growing fast means you need to eat a lot.";
		if (level == 3) hint = "Sauropods are dangerous but will you get a big pack together?";
		if (level == 4) hint = "Stay alive and mate as many times as you can to score points";
		currentHint = "Hint:\n" + hint; //added
		if (hint != null) return PadInTable("<FONT FACE='Arial,Helvetica,Helv' SIZE=2 style=\"font-size: 8pt;\">Hint:<BR>" + hint + "</FONT>");
		return hint;
	}

	string FileToStr(string file)
	{
		string str = "";
		localFile LI;
		open(out LI, file);
		string line = "";
		int debugSafety = 0;
		while ((
			line = LI.getLine())
			!= null
			&& debugSafety < 9999)
		{
			str += line;
			debugSafety++;
		}
		if (debugSafety == 9999) Logger.LogError("WARNING: INFINITE WHILE BROKEN");
		close(LI);
		chomp(str);
		return str;
	}

	string PadInTable(string str)
	{
		string pad = "3";
		return $"<TABLE BORDER=\"0\" CELLSPACING=\"0\" CELLPADDING=\"{pad}\"><TR><TD>{str}</TD></TR></TABLE>";
	}

	string DrawPane(object inner_html, string title, double pad, string width)
		{
		if(pad == 0) pad = 3 ;
		if(width == null) width = "100%" ;
		var box_bg_color = level_colourscheme[level].BGCOLOR;
		var box_fg_color = level_colourscheme[level].FGCOLOR;
		var fgdot = $"<IMG SRC='{images_url}/xdot.gif' WIDTH=1 HEIGHT=1 ALT=''>";
		var bgdot = $"<IMG SRC='{images_url}/xdot.gif' WIDTH={pad} HEIGHT={pad} ALT=''>";
		var fgcell = $"<TD WIDTH=1 BGCOLOR='{box_fg_color}'>{fgdot}</TD>";
		var bgcell = $"<TD WIDTH={pad} BGCOLOR='{box_bg_color}'>{bgdot}</TD>";
		var fgline = $"<TR><TD HEIGHT=1 COLSPAN=5 BGCOLOR='{box_fg_color}'>{fgdot}</TD></TR>";
		var bgline = $"<TR>{fgcell}<TD HEIGHT={pad} COLSPAN=3 BGCOLOR='{box_bg_color}'>{bgdot}</TD>{fgcell}</TR>";

		string str = "";
	  str += "\n<!--Pane-->\n";
	  str += $"<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0 WIDTH='{width}'>\n";
	  str += $"{fgline}\n{bgline}\n";
	  str += $"<TR BGCOLOR='{box_bg_color}'>{fgcell}{bgcell}<TD>{inner_html}</TD>{bgcell}{fgcell}</TR>\n";
	  str += $"{bgline}\n{fgline}\n";
	  str += "</TABLE>\n";
		if (title != null)
	  {
	    str += $"<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=2 WIDTH='{width}'>\n";
	    str += $"<TR><TD BGCOLOR='{box_fg_color}' WIDTH=120 ALIGN=LEFT><FONT COLOR=BLACK FACE='Arial, Helvetica'><B>{title}</B></FONT></TD><TD></TD><TD><FONT SIZE=1>&nbsp;</FONT></TD></TR>\n";
	    str += "</TABLE>\n";
		}
		return str;
	}

	string DrawCompass()
 {
		string str = "";
	  str += $"\n<!-- Compass: x={x}; nx={nx}; y={y}; ny={ny} -->\n";
	  str += "<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0><TR>\n";
	  str += CompassCell("northwest", (x > 0 && y > 0), "Move:NW", 39, 35);
	  str += CompassCell("north", (y > 0), "Move:N", 19, 35);
	  str += CompassCell("northeast", (x <nx - 1 && y > 0), "Move:NE", 44, 35);
	  str += "</TR></TABLE>\n";
	  str += "<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0><TR>\n";
	  str += CompassCell("west", (x > 0), "Move:W", 29, 32);
	  str += CompassCell("wait", true, "Wait", 45, 32);
	  str += CompassCell("east", (x <nx - 1), "Move:E", 28, 32);
	  str += "</TR></TABLE>\n";
	  str += "<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0><TR>\n";
	  str += CompassCell("southwest", (x > 0 && y <ny - 1), "Move:SW", 41, 35);
	  str += CompassCell("south", (y <ny - 1), "Move:S", 18, 35);
	  str += CompassCell("southeast", (x <nx - 1 && y <ny - 1), "Move:SE", 43, 35);
	  str += "</TR></TABLE>\n";
		return str;
	}

	string CompassCell(string dir, bool valid, string action, double width, double height)
		{
		var off = $"\"{compass_images_url}/level_{level}/" + dir + "_off.gif\"";
		var on = $"\"{compass_images_url}/level_{level}/" + dir + "_on.gif\"";
		var wait_on = $"\"{compass_images_url}/level_{level}/wait_on_" + dir + ".gif\"";
		var wait_off = $"\"{compass_images_url}/level_{level}/wait_off.gif\"";
		var img = $"<IMG NAME='{dir}' BORDER=0 SRC={off}>";
		if (valid)
	  {
			var mouseover = (dir == "wait")? $"switch_img(\"{dir}\",{on})"  : $"switch_imgs(\"{dir}\",{on},\"wait\",{wait_on})";
			var mouseout = (dir == "wait")? $"switch_img(\"{dir}\",{off})" : $"switch_imgs(\"{dir}\",{off},\"wait\",{wait_off})";
			var act = (dir == "wait")? "wait here" : $"move {dir}";
	  	 mouseover += $"; self.status=\"{act}\"; return true;";
	  	 mouseout += "; self.status=\"\"; return true;";
	    img = $"<A HREF='javascript:do_action(\"{action}\")' OnMouseOver='{mouseover}' OnMouseOut='{mouseout}'>{img}</A>";
		}
		return $" <TD ALIGN=CENTER>{img}</TD>";
	}

	string BeginActionButtons()
 {
	  return "<TABLE BORDER=1 CELLSPACING=0><TR>";
}

	string ActionButton(string desc, string action, string width = null)
		{
		if (width != null) width = $"WIDTH='{width}'";
		var bg =level_colourscheme[level].BGCOLOR;
		var fg =level_colourscheme[level].FGCOLOR;
	  if (action == null) action = desc;
	  //#return "<INPUT TYPE=BUTTON VALUE='{desc}' Onclick='do_action(\"{action}\"); return true;'>";
	  return $"<TD CLASS='ActionButton' {width} ALIGN=CENTER BGCOLOR='{fg}'><A HREF='javascript:do_action(\"{action}\");'>"
	         + $"<FONT SIZE=1 FACE='Arial, Helvetica, Helv' COLOR=BLACK>&nbsp;{desc}&nbsp;</FONT></A>";
}

	string ActionImgButton(string img, string desc, string action, string width = "", int height = 0)
 {
	if (!(height != 0)) height = 18;
		if (width != "0")  width = $"WIDTH='{width}' HEIGHT='{height}'"  ;
	  var src=$"{images_url}/level_{level}/{level}_{img}.gif";
	  return $"<A OnMouseOver='self.status=\"{desc}\"; return true;' HREF='javascript:do_action(\"{action}\");'><IMG BORDER=0 ALIGN='BASELINE' SRC='{src}' ALT='{desc}' {width}></A>";
}

	string EndActionButtons()
 {
	  return "</TD></TR></TABLE>";
}

	string DrawMap()
	{
		var i = 0;
		var j = 0;
		var w =map_sq_size *nx;
		var h =map_sq_size *ny;
		string str = "";

		str += "\n<!--Map-->\n";
		str += $"<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0 WIDTH={w} HEIGHT={h}>\n";
		foreach (var row in map)
  		{
			i = 0;
			str += "<TR>\n";
			foreach (var col in row)
			{
				//#create key for image from i & j => a1, b2 etc.
				string key = chr(ord('a') + i).ToString() + (j + 1);

				//#Select image for Al, mum or both
				var image = (i ==x && j ==y) ? $"{images_url}/al.gif" : $"{images_url}/blank.gif";
				if (have_mother && i ==old_mx && j ==old_my)
				{
					image = (i ==x && j ==y)? $"{images_url}/mum_and_al.gif":$"{images_url}/mum.gif";
				}

				string img;
				if (visited[j][i] >=map_mode)
				{
					img = $"{map_images_url}/{key}.gif";
				}
				else
				{
					img = $"{map_images_url}/blank.gif";
				}

				str += $" <TD WIDTH='{map_sq_size}' HEIGHT='{map_sq_size}'";
				str += $" BACKGROUND='{img}'";
				if (visited[j][i] >= map_mode) str += $" BGCOLOR='{habitats[map[j][i]]["MCOLOR"]}'" ;
				str += $"><IMG SRC='{image}' WIDTH='{map_sq_size}' HEIGHT='{map_sq_size}' ALT=''></TD>\n";
				i++;
			}
			str += "</TR>\n";
			j++;
		}
		str += "</TABLE>\n";
		return str;
	}

	string DrawStatus()
	{
		var pctwt = (level >= 1 && level <= 4)? weight /level_weight[level] * 100 : 0;
		string str = "";
		str += DrawNameValBox("Level", levels[level - 1]);
		str += ImageForStatus(pctwt);
		str += DrawNameValBox("Weight", round_to_sf(weight, 3) + "kg");
		str += ImageForStatus(energy);
		str += DrawNameValBox("Energy", sprintf("%.0d%%",energy));
		str += ImageForStatus(fitness);
		str += DrawNameValBox("Fitness", sprintf("%.0d%%",fitness));
		str += DrawNameValBox("Score", score.ToString());
		return str;
	}

	void AddStatusToHash(ref PageContent hr)
	{
	  	hr.Level   = level;
		hr.Weight  = round_to_sf(weight, 3) + "kg";
		hr.Energy  = sprintf("%.0d%%",energy);
		hr.Fitness = sprintf("%.0d%%",fitness);
		hr.Score   = score;
		hr.WeightImg = ImageForWeight();
		hr.EnergyImg = ImageForStatus(energy);
		hr.FitnessImg = ImageForStatus(fitness);
	}

	internal string currentEnergySprite; //added
	internal string currentFitnessSprite; //added
	string ImageForStatus(double val)
	{
		//#round value to nearest 10
		val = round(val / 10) * 10;
		currentEnergySprite = level < 0 ? "1_100.gif" : $"{level}_{round(energy / 10) * 10}.gif";
		currentFitnessSprite = level < 0 ? "1_100.gif" : $"{level}_{round(fitness / 10) * 10}.gif";
		return $"<IMG WIDTH=78 HEIGHT=105 VSPACE=0 HSPACE=0 ALIGN=TOP BORDER=0 SRC='{status_images_url}/{level}_{val}.gif' ALT=''>";
	}

	internal string currentWeightSprite; //added
	string ImageForWeight()
	{
		//#round percentage weight to nearest 10
		var pctwt = level >= 0 ? weight / level_weight[level] * 100 : 0; //added: allow for level -1
		var val = round(pctwt / 10) * 10;
		currentWeightSprite = level < 0 ? "1w_0.gif" : $"{level}w_{val}.gif";
		return $"<IMG WIDTH=78 HEIGHT=105 VSPACE=0 HSPACE=0 ALIGN=TOP BORDER=0 SRC='{status_images_url}/{level}w_{val}.gif' ALT=''>";
	}

	string ColourCode(double val, double t1, double t2)
		{
		if (val >t2) return "green" ;
		if (val <t1) return "red" ;
		return "yellow";
	}

	string DrawNameValBox(string name, string val)
 {
		var bgcolor = level_colourscheme[level].FGCOLOR;
		return $"<TABLE BGCOLOR='{bgcolor}' CELLPADDING=1><TR><TD WIDTH=80 ALIGN=CENTER><FONT SIZE=1 COLOR=WHITE FACE='Arial, Helvetica'>{name} - {val}</FONT></TD></TR></TABLE><BR>";
	}

	string DrawBar(string name, double val, double lbl, string color, int max_width, ref string infoForUnity) //added ref string
	{
		//#Compute width (integer value)
		var width =val;
		if (max_width != 0) width *= max_width / 100.0 ;
	  width = round(width);

		//#Generate test label
		string lblstring = lbl.ToString();	
		if (lbl.ToString() == "0") lblstring = sprintf("%.0d%%", val);
		if (lbl > 5000) lblstring = "Huge" ;

		if(color == null) color = "green" ;
		if (val == 0) val = 1 ;

		infoForUnity = $"{color}|{width}|{lblstring}";
		return $"<TR VALIGN=MIDDLE><TD WIDTH=2><FONT SIZE=1>&nbsp;</FONT></TD><TD><FONT FACE='Arial, Helvetica' SIZE=1>{name}</FONT></TD><TD WIDTH=2><FONT SIZE=1>&nbsp;</FONT></TD><TD><FONT COLOR=BLACK><IMG BORDER=1 SRC='{images_url}/{color}.gif' ALT='{name}: {lblstring}' WIDTH='{width}' HEIGHT='10' ALIGN=MIDDLE></FONT><FONT FACE='Arial, Helvetica' SIZE=1>&nbsp;{lblstring}</FONT></TD></TR>\n";
	}
	internal List<string[]> enemyBars = new List<string[]>(); //added
	//#
	//# Species Information box
	//# Report true values with no chance factor
	//#
	string SpeciesInfo(string spec)
	{
		var spec_size = SizeOfSpecies(spec, species_present);
		var fiercenessStr = sprintf("%.2f", CalcFiercenessRatio(spec,spec_size));
		double.TryParse(fiercenessStr, out double fierceness);
		var agilityStr = sprintf("%.2f", CalcAgilityRatio(spec));
		double.TryParse(agilityStr, out double agility);
		var nutrition = round(species[spec].Nutrition / weight * energy_per_bodyweight * 100);
		var eb = species[spec].Nutrition / weight * 50;
		if (eb > 100) eb = 100 ; //#bar for energy
		var image =species[spec].Image;
		string border = perlRegex(image, @"\\.gif") ? "0" : "1"; //#don't border gifs
		var img = $"<IMG BORDER={border} WIDTH=100 ALT='{spec}' SRC='{species_images_url}/{image}'>";
		string fiercenessString = ""; //added
		string agilityString = ""; //added
		string energyString = ""; //added
		string str =
	$"<P><TABLE BORDER = 0 CELLSPACING = 0 CELLPADDING = 0 ><TR><TD ALIGN = CENTER VALIGN = MIDDLE WIDTH = 100 >{img}</TD><TD>";
	   str += "<TABLE BORDER=0 CELLSPACING=0 CELLPADDING=0>";
		str += "<TR><TD WIDTH=2><FONT SIZE=1>&nbsp;</FONT></TD><TD COLSPAN=3><FONT style='font-size:8pt' SIZE=1 FACE='Arial, Helvetica, Helv'>" + HTMLForSpecies(spec) + "</FONT></TD></TR>";
		str += RatioBar("fierceness", round_to_sf(fierceness, 3), 50, ref fiercenessString);
		str += RatioBar("agility", round_to_sf(agility, 3), 50, ref agilityString);
		str += DrawBar("energy",eb, round_to_sf(nutrition, 3), null, 50, ref energyString);
		str += "<TR><TD WIDTH=2><FONT SIZE=1>&nbsp;</FONT></TD><TD COLSPAN=3>";
		enemyBars.Add(new string[] { fiercenessString, agilityString, energyString }); //added
		if (!suspended)
	   {
	      str += ActionImgButton("m_attack", $"Attack the {spec}", $"Attack:{spec}:{current_pack_size}", "43");
			if (level == 4) str += ActionImgButton("m_mate", $"Mate with the {spec}", $"Mate:{spec}", "30") ;
		}
	   str += ActionImgButton("m_factfile", $"Facts about the {spec}", $"Popup:{species[spec].URL}", "43"); //#Fact file enabled even when suspended
		str += "</TD></TR></TABLE>\n" +
	"</TD></TR></TABLE>";
	  return str;
	}

	string RatioBar(string name, double val, int max_width, ref string infoForUnity) //added ref string
	{
		var dval = (val != 0) ? log10(val) + 5 : 0; //#log scale
		string color;
		if (val < 1 - behaviour_randomness)    { color = "green"; }
	  else if (val > 1 + behaviour_randomness) { color = "red"; }
	  else { color = "yellow"; }
		if (dval > 10) dval = 10 ;
		if (dval < 0) dval = 0 ;
		return DrawBar(name,dval * 10,val,color,max_width, ref infoForUnity);
	}

	//##############################################################
	//##
	//## Misc
	//##
	//##############################################################

	string a_or_an(string word, object uc = null)
 {
		var substring = substr(html_to_text(word), 0, 1);
		if (perlRegex("aeiou", $"{substring}", ignoreCase: true)) { return uc != null ? $"An {word}" : $"an {word}"; }
	   else { return uc != null ? $"A {word}" : $"a {word}"; }
	}

	string html_to_text(string str)
	{
	  perlSubstituteGlobal(ref str, "<.*?>", "");
	  return str;
	}

	double log10(double underscore)
	{
		double rv = 0;
		try
		{
			rv = log(underscore) / log(10);
		}
		catch (Exception at)
		{
			Carp.confess($"Invalid Logarithm {at}");
		};
		return rv;
	}

	//#Round a floating-point value to an integer
	int round(double val)
	{
		return (int)System.Math.Round(val);
	}

	//#Round a floating-point value to a given order of magnitude
	double round_to_sf(double val, double sf)
	{
		if(val == 0) return 0 ;
		int om = (int)System.Math.Floor(log10(abs(val)));
		double factor = System.Math.Pow(10, (om - sf + 1));
		return round(val / factor) * factor;
	}

	//# Write out debug info
	//# Usage : debug("xxx")
	//#         debug(2,"xxx")
	void debug(string debugString)
	{
		debug(1, debugString);
	}
	void debug(int num, string debugString)
	{
		Logger.Log("PERL DEBUG: " + debugString);
		string val = debugString; //#nessary 2nd parameter
		int debug_level = num; //#optional 1st parameter
		if(DEBUG > 0 && DEBUG >= debug_level)
		{
			debug_str+=$"{val}<BR>";
		}
	}
}