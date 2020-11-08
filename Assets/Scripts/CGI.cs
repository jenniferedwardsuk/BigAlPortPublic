using System;
using static PerlInterface;

public class CGI
{
	internal static string escape(string next_action)
	{
		return next_action;
	}

	internal static string header(PerlCookie[] cookie, string expires)
	{
		return "";
	}
}
