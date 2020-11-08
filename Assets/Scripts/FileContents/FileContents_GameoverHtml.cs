internal static partial class FileContents
{
	internal static string gameoverHtml = @"<!-------------------------------------->
<!--    Template for Game Over page   -->
<!-------------------------------------->
</FORM>

<SCRIPT LANGUAGE=""JAVASCRIPT"">
function validate()
	{
		if (document.forms[1].name.value == """")
		{
			window.alert(""Please enter your name first"");
			return false;
		}

		if (document.forms[1].details.value == """")
		{
			window.alert(""Please enter your contact details first"");
			return false;
		}

		document.forms[1].submit();
		return false;
	}

	function popmailwin(x, y) { window.open(x, y, 'status=no,scrollbars=yes,resizable=yes,width=350,height=400'); }

</SCRIPT>

<FORM ACTION = ""{{ScoresURL}}"" METHOD=""POST"">

<TABLE BORDER = 0 CELLSPACING=0 CELLPADDING=0 BGCOLOR=""{{BGColor}}"" WIDTH=""100%"">

<!---------><TR><TD HEIGHT = 1 COLSPAN=5 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>

<!--Title-->
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD HEIGHT = 1 COLSPAN=3 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD WIDTH = 1 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=10 border=""0""></TD>

    <TD ALIGN = ""CENTER"" VALIGN=""CENTER""><FONT FACE = 'Arial,Helvetica,Helv' SIZE=5><b>GAME
		  OVER</b></FONT><br>
      <br></TD>

    <TD WIDTH = 10 BGCOLOR= ""{{BGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 10 HEIGHT= 10 border= ""0""></TD>
	
		<TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
	

	<!---------><TR><TD HEIGHT= 1 COLSPAN= 5 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
	

	<!--Box-->
	<TR><TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD>
	
		<TD HEIGHT= 1 COLSPAN= 3 BGCOLOR= ""{{BGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 10 HEIGHT= 1 border= ""0""></TD>
	
		<TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
	<TR><TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD>
	
		<TD WIDTH= 10 BGCOLOR= ""{{BGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 10 HEIGHT= 1 border= ""0""></TD>
	<TD ALIGN= ""CENTER"" VALIGN= ""CENTER"" HEIGHT= 100>
	

	   <FONT FACE= 'Arial,Helvetica,Helv' SIZE= 3>{{Text}}</FONT><br>
   <FONT FACE = 'Arial,Helvetica,Helv' SIZE=4>You scored {{Score}}
	points.</FONT><br>

</TD>
    <TD WIDTH = 10 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD HEIGHT = 4 COLSPAN=3 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>

<!---------> <TR><TD HEIGHT = 1 COLSPAN=5 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>

<!--Subtitle-->
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD HEIGHT = 4 COLSPAN=3 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD WIDTH = 10 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD ALIGN = CENTER> <font face=""Arial,Helvetica,Helv"" size=""5""><b>High Score

	  competition</b></font> <br>
      <br>
    </TD>
    <TD WIDTH = 10 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>

<!---------><TR><TD HEIGHT = 1 COLSPAN=5 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>

<!--Box-->
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD HEIGHT = 1 COLSPAN=3 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>
    <TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD></TR>
<TR><TD WIDTH = 1 BGCOLOR=""{{FGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=1 HEIGHT=1></TD>
    <TD WIDTH = 10 BGCOLOR=""{{BGColor}}""><IMG SRC = ""/furniture/tiny.gif"" WIDTH=10 HEIGHT=1 border=""0""></TD>

<TD ALIGN = CENTER HEIGHT=200>
     Enter your name: &nbsp; <INPUT NAME = name TYPE=TEXT><br>
     Enter your contact details (email address, phone #, postal address) so 
        we can get in contact if you win the competition:<br>
        <TEXTAREA NAME = details ROWS= 5 COLS= 40></TEXTAREA>

		<br>

		<SMALL> The BBC will not use these details for any purpose other than to
		contact you if you're a winner</SMALL><br>
      <br>
     <INPUT TYPE = BUTTON Onclick= ""validate()"" VALUE= ""Submit my score to the competition""><br>
</TD>

	<TD WIDTH= 10 BGCOLOR= ""{{BGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 10 HEIGHT= 1 border= ""0""></TD>

	<TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
<TR><TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD>

	<TD HEIGHT= 1 COLSPAN= 3 BGCOLOR= ""{{BGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 10 HEIGHT= 1 border= ""0""></TD>

	<TD WIDTH= 1 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
<TR><TD HEIGHT= 1 COLSPAN= 5 BGCOLOR= ""{{FGColor}}""><IMG SRC= ""/furniture/tiny.gif"" WIDTH= 1 HEIGHT= 1></TD></TR>
</TABLE>


<!--Buttons--> <br>
<CENTER>

	<a href= ""javascript:do_action('REINCARNATE')""><img src= ""/dinosaurs/bigalgame/images/level_1/1_restartlevel.gif"" width= ""118"" height= ""12"" border= ""0"" alt= ""Restart Level""></a><a href= ""javascript:do_action('BEGIN')""><img src= ""/dinosaurs/bigalgame/images/level_1/1_restartgame.gif"" width= ""118"" height= ""12"" border= ""0"" alt= ""Restart from beginning""></a><A onClick= ""popmailwin('/cgi-bin/navigation/mailto.pl?GO=1','Mailer')"" HREF= ""/cgi-bin/navigation/mailto.pl?GO=1"" TARGET= ""Mailer""><img src= ""/dinosaurs/bigalgame/images/sendtoafriend.gif"" width= ""118"" height= ""12"" alt= ""Send it to a friend"" border= ""0""></a><a href= ""javascript:self.close()""><img src= ""/dinosaurs/bigalgame/images/level_1/1_quit.gif"" width= ""118"" height= ""12"" border= ""0"" alt= ""Quit""></a>

	<br>
  </CENTER>
";
}
