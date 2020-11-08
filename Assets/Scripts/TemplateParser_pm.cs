using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PerlInterface;

public class TemplateParser_pm
{
	//#########################################################################################  
	//#  
	//# TemplateParser.pm 
	//# 
	//# Generic template parser
	//# name, value pairs are supplied in a hash
	//# fields with those names found in an input file are replaced with the values
	//# default field delimiting is {{field}} but this may be changed
	//# The parser can print,write output to a file or return a string
	//#
	//# Author  : John Alden  
	//#
	//#########################################################################################  

	//	package TemplateParser;

	//#########################################################################################  
	//# Object Interface
	//#########################################################################################  

	string start_field;
	string end_field;
	string infile;
	string cache;

	public TemplateParser_pm()
	{
		//var pclass = shift; 
		//%$self = @_; //#store options in self

		this.start_field = "{{";
		this.end_field = "}}";

		//bless self, pclass; 
		//return self; 
	}

	public void input_file(string filepath)
	{
		this.infile = filepath;
		this.cache = "";

		localFile LI;
		if (!open(out LI, this.infile)) perlException("Unable to open " + this.infile);
		string line;
		int debugSafety = 0;
		while ((line = LI.getLine()) != null
			&& debugSafety < 9999)
		{
			this.cache += line;
			debugSafety++;
		}
		if (debugSafety == 9999) Logger.LogError("WARNING: INFINITE WHILE BROKEN");
		close(LI);
	}

	PageContent refFields;
	internal void fields(PageContent self)
	{
		refFields = self;
	}

	//void delimiters()
	//{
	//	//my $self=shift;
	//	//$self->{ 'stat_field'} = shift;
	//	//$self->{ 'end_field'} = shift;
	//}

	public void parserprint()
	{
		Action<string> printer = (string value) => { pprint(value); };
		_parse(printer);
	}

	public string to_string()
	{
		string str = null;
		Action<string> catter = (string value) => {str += value; };
		_parse(catter);
		return str;
	}

	//#########################################################################################  
	//# Subroutines
	//#########################################################################################  

	void _parse(Action<string> refSub)
	{
		string sstring= this.cache;
		_replace_fields(ref sstring);
		refSub(sstring);
	}

//# Symbol replacer
	void _replace_fields(ref string refLine)//$$)
	{
		var refFields = this.refFields;

//# Perform if operations first
		var if_start = this.start_field
		+ @"IF (\\w+)" + this.end_field;
		var if_end = this.start_field + "ENDIF"+ this.end_field;
		var find = if_start + "(.*?)" + if_end;
		string[] matches = perlRegexReturningMatches(refLine, find);
		for (int i = 0; i < matches.Length - 1; i += 2)
		{
			var replace = "";
			if (refFields.HasValue(matches[i])) replace = matches[i + 1];
			refLine = perlSubstitute(refLine, find, replace);
		}

//# Perform substitutions
		foreach (var key in refFields.Keys)
		{
//# replace all instances of {{key}} with value
			find = this.start_field + key + this.end_field;
			var replace = refFields.GetValue(key);
			refLine = perlSubstituteGlobal(ref refLine, find, replace);
		}
	}
}
