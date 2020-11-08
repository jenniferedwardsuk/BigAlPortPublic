using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BigAl_Scores_pl
{

	//#!/usr/local/bin/perl

	//#########################################################################################
	//#
	//# bigal_scores.pl - Score processing for the Big Al game - LIVE VERSION
	//#
	//#########################################################################################

	string bigal_server="http://www.bbc.co.uk"; //#LIVE

	//#Enable/Disable Debugging
	//use vars qw($DEBUG);
	int DEBUG;

	void BEGIN()
	{
		DEBUG=0; //#Set to zero on release

		//#Modules required for debugging only
		if(DEBUG != 0)
		{
			//		require strict;
			//require CGI::Carp;
			//import CGI::Carp "fatalsToBrowser";
		}
	}

	//#Modules
	//use lib '/home/system/cgi-bin/lib';
	//use Merde;
	//use English;
	//use CGI qw(:standard);
	//use Fcntl qw(:flock);
	//#define names of constants
	//use TemplateParser;   #for HTML templating

	//#Directories - deduce from APACHE ENV VARS
	string dir = ENV.SCRIPT_FILENAME;
	string files_dir = "";
	string output_dir = "";

	//#URLs - deduce from APACHE ENV VARS
	string cgi_url = ENV.REQUEST_URI;
	string www_url = "";

	//# File locations
	string images_url = "";
	string bigal_url =  "";
	string scores_file = "";

	void PerlFileLocSetup()
	{
		//string[] matches = perlSubstituteReturningMatches(dir, @"(.*)\", @"(.*?\.pl)");
		//dir = matches[0];
		//files_dir = dir;
		//files_dir = perlSubstitute(files_dir, "cgi-bin", "data");
		//output_dir = dir;
		//output_dir = perlSubstitute(output_dir, "cgi-bin", "results");

		//matches = perlSubstituteReturningMatches(cgi_url, @"(.*)\", @".*?\.pl(\?.*) ?");
		//cgi_url = matches[0];
		//www_url = cgi_url;
		//www_url = perlSubstitute(www_url, @"cgi-bin\", "");

		//images_url = $"{www_url}/images";
		//bigal_url = $"{bigal_server}{cgi_url}/bigal.pl";
		//scores_file = $"{output_dir}/scores.dat";
	}

	//# Constants
	const string cookie_field_delimiter = "_";	//#character used to separate fields in cookie
	const int cookie_crypt = 3498578;			//#cookie decryption key for score
	const int max_scores = 10;					//#max number of scores to hold in file
	const int MAX_ATTEMPTS=10;                  //#max number of attempts to open file

	//#########################################################################################
	//# Main Program
	//#########################################################################################

	//my $dummy; #for list assignments [in place of undef for old perl versions]

	//print header();

	void main()
	{
		try
		{
			PerlFileLocSetup();
			//#Read
			//my @score_table;
			object[] score_table = null; //dummy
			ReadHighScores(score_table);//\@score_table);

			//# CGI parameters - apply Merde filter to name (details are not shown)
			//my $filter = new Merde;
			//my $name = filter.star(param("name"));
			//my $details = param("details");

			//#Update (possibly)
			if (true)//$name && $details)
			{
				//TRACE("Checking for update");
				//my($score, $matings, $moves) = ReadStateFromCookie();
				//TRACE("Data: ".join(",", score, $matings, $moves));
				//my $position = ComputePosition(\@score_table, $score,
				//$matings, $moves, $name, $details);
				//TRACE("Position: ".$position);
				if (true)//defined $position && $position <= $max_scores)
				{
					//TRACE("Updating");
					//UpdateHighScores(\@score_table, $score, $matings,
					//$moves, $name, $details);
					//WriteHighScores(\@score_table);
				}
			}

			//#Display
			DisplayScores(score_table);//\@score_table);
			Status("Done");
			//exit(0);

		}
		catch(Exception EVAL_ERROR)
		{ 
			//pprint("Unable to update scores - try reloading the page in a few minutes to submit your score.");
			//pprint($"bigal_scores.pl: {EVAL_ERROR}"); //#log error
			Status("Error");
			//exit(1);
		}
	}

	//#########################################################################################
	//# Subroutines
	//#########################################################################################

	string[] ReadStateFromCookie()
	{
		//var list = split(cookie_field_delimiter, cookie<string>("'state'"));
		//TRACE("Cookie :" + join(",", list));
		//string moves = list[0];
		//int.TryParse(list[9], out int score);
		//string matings = list[10];
		//score ^= cookie_crypt;
		return new string[] { };//score.ToString(), matings, moves };
	}

	int ComputePosition(string[] _)
	{
		//string[] ref_table; int score; string matings, moves, name, details;

		int count = 1;
		//string entry = $"{score}\t{matings}\t{moves}\t{name}\t{details}";
		//foreach (var tbl_row in ref_table)
		//{
		//	int.TryParse(split("/\t/",tbl_row)[0], out int tbl_score);
		//	if (tbl_row == entry) return 0; //#score already present - exit
		//	if (score > tbl_score) return count ;
		//	count++;
		//}
		return count;
	}

	void ReadHighScores(object dollar)//$)
	{
	//	my $table = shift;
	//	TRACE("Reading scores");

	//# Check the file exists - if not, create it
	//# system("touch $scores_file") unless(-e $scores_file);
	//	die("Unable to create $scores_file") unless(-e $scores_file);

	//# Try to open the file for reading
	//	my $is_open = 0;
	//	my $attempts = 0;
	//	local* LI;
	//	until($is_open || $attempts >$MAX_ATTEMPTS)
	//  {
	//    $attempts++;
	//		open(LI, "<$scores_file") and $is_open = 1;
	//		unless($is_open)

	//	{
	//			Status("Failed to open file (attempt $attempts) - file may be being written - will retry...");
	//			sleep(1); #wait before re-trying
	//    }
	//	};
	//	die("Unable to open $scores_file") unless($is_open);

	//	while (< LI >)
	//	{
	//		chomp;
	//		next if /^$/;
	//		push @$table, $_;
	//		TRACE("Reading $_");
	//	}
	//	close LI;

	//	TRACE("Read scores");
	}

	void UpdateHighScores()
	{
	//	my ($ref_table, $score, $matings, $moves, $name, $details) = @_;
	//  $details =~ s/\s/ /g; #remove tabs & newlines

	//  #insert the new entry in the right place
	//  my @new_scores;
	//	my $entry="$score\t$matings\t$moves\t$name\t$details";
	//  do
	//  {
	//		my $line = shift @$ref_table;
	//     $score = -1 if ($entry eq $line); #don't allow the same entry to be submitted more than once
	//     my ($row_score, dummy, dummy, dummy) = split(/\t/,$line);
	//     if ($score>$row_score)
	//     {
	//        TRACE("Adding $entry");
	//        push @new_scores, $entry;
	//        $score=-1;
	//     }
	//     push @new_scores, $line;
	//  }  while(@$ref_table);

	//  #if there are no scores, put this one in
	//  push (@new_scores, $entry) if($score>0);

	//  #shorten the list if necessary
	//  pop @new_scores while(scalar(@new_scores) > $max_scores);

	//  @$ref_table = @new_scores;
	}

	void WriteHighScores(object dollar)//$)
	{
	//  my $ref_scores = shift;

	//  #Try to open the file for writing
	//  local *LO;
	//  my $is_open=0;
	//  my $attempts=0;
	//  until($is_open || $attempts>$MAX_ATTEMPTS)
	//  {
	//    $attempts++;
	//    open (LO, ">$scores_file") and $is_open=1;
	//    unless($is_open)
	//    {
	//      Status("Failed to open file (attempt $attempts) - file may be being written - will retry...");
	//      sleep(1); #wait before re-trying
	//    }
	//  };
	//  die("Unable to open $scores_file") unless($is_open);

	//  #Lock and write the file
	//  flock LO, LOCK_EX;
	//  foreach my $line (@$ref_scores)
	//  {
	//  	 TRACE("Writing: $line");
	//    print LO "$line\n";
	//  }
	//  flock LO, LOCK_UN;
	//  close LO;
	}

	void DisplayScores(object dollar)//$)
	{
	//  my ($ref_table) = @_;

	//  #Generate table rows
	//  my $str;
	//  my $count=0;
	//  foreach my $line (@$ref_table)
	//  {
	//	my ($score, $matings, $moves, $name, $dummy) = split(/\t/, $line);
	//      if($name eq param("name"))
	//      {
	//        $name = "<B>$name</B>";
	//        $score = "<B>$score</B>";
	//        $matings = "<B>$matings</B>";
	//        $moves = "<B>$moves</B>";
	//      }
	//      my $color = ($count % 2)? "#003300":"#006600";
	//      $str.="<TR BGCOLOR=$color><TD>$name</TD>";
	//      $str.="<TD>$score</TD>";
	//      $str.="<TD>$matings</TD>\n";
	//		$str.="<TD>$moves</TD></TR>\n";
	//      $count++;
	//  }

	//  #Output template
	//  my $parser = new TemplateParser;
	//  $parser->input_file("$files_dir/templates/scores.html");
	//  $parser->fields({ScoreRows => $str, BigAlURL => $bigal_url, ImagesURL => $images_url, Javascript => &FileToStr("$files_dir/javascript.js")});
	//  $parser->print();
	}

	void FileToStr(object dollar)//$)
	{
	//  my $str;
	//  my $file=shift;
	//  local *LI;
	//  open LI, $file;
	//  $str.=$_ while <LI>;
	//  close LI;
	//  chomp $str;
	//  return $str;
	}

	void Status(string val)
	{
		//pprint("<SCRIPT LANGUAGE=JAVASCRIPT>window.status='{val}';</SCRIPT>\n");
	}

	void TRACE(string _)
	{
		//if (DEBUG != 0) pprint("<!-- TRACE: " + shift() + "-->\n");
	}

}
