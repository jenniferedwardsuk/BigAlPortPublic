internal static partial class FileContents
{
	internal static string quitHtml = @"";

	internal static string scoresHtml = @"<!DOCTYPE HTML PUBLIC "" -//W3C//DTD HTML 3.2 Final//EN"">
<HTML>
<HEAD>

<META HTTP-EQUIV=""Content-Type"" CONTENT=""text/html; charset=iso8859-1"">
<META NAME = ""DESCRIPTION"" CONTENT=""BBC Online Walking with Dinosaurs site - BBC Online's brings you in-depth descriptions of over 60 dinosaurs and the eras in which they lived. Including sound, video, photographs and interactive games."">
<META NAME = ""KEYWORDS"" CONTENT=""Walking with Dinosaurs,Dinosaurs,Fossils,Palaeontology,Paleontology,Jurassic Park,Bones,Prehistoric,Monsters,Reptiles,New Blood,Time of the Titans,A cruel sea,Beneath a giant's wings,Shadows of the silent forest,Death of dynasty,BBC1,BBC,Animatronics,Dinosaur television,Computer animation,Computer animated dinosaurs,Animated dinosaurs,British Broadcasting Corporation,Dinosaur questions"">
<META NAME = ""VERSION"" CONTENT=""9 November 1999""> 

<TITLE>BBC Online - Walking with Dinosaurs</TITLE>

<SCRIPT LANGUAGE = ""JAVASCRIPT"" TYPE=""text/javascript"">

function popwin_ff(url)
	{
		window.open(url, 'dino', 'status=no,scrollbars=yes,resizable=yes,width=500,height=400');
	}

	function do_action(val)
	{
		document.forms[0].action.value = val;
		document.forms[0].submit();
	}

	function popmailwin(x, y) { window.open(x, y, 'status=no,scrollbars=yes,resizable=yes,width=350,height=400'); }

</SCRIPT>
</HEAD>

<BODY BGCOLOR = ""#003300"" MARGINHEIGHT = 0 MARGINWIDTH = 0 TOPMARGIN=0 LEFTMARGIN=0 LINK=""#ffffff"" TEXT=""#ffffff"" VLINK=""#ffffff"" BACKGROUND=""images/background.jpg"">



<table width = ""600"" border=0 cellpadding=0 cellspacing=0>
    <tr>
    <td colspan = ""3""><img src=""http://www.bbc.co.uk/dinosaurs/bigalgame/images/top_banner.gif"" width=""600"" height=""85"" alt=""Walking with Dinosaurs - Big Al Game"" vspace=""0"" hspace=""0"" border=""0""></td>
  </tr>
    <tr><td><img src = ""images/spacer.gif"" width=5 height=480></td>
	
    <td valign = ""top""> <font face=""Arial, Helvetica, sans-serif"" size=""5""><b><br>
      <font color = ""#FF0000"" size=""6"">High Scores</font></b></font><br>
      <br>
      <CENTER><font face = ""Arial, Helvetica, sans-serif"" size= ""2"">
  
		  <TABLE BORDER= ""0"" WIDTH= ""70%"">
  
			<TR BGCOLOR= ""#009900"">
  
			  <TD><B> Name </B></TD>
  
			  <TD><B> Score </B></TD>
  
			  <TD><B> Matings </B></TD>
  
			  <TD><B> Moves </B></TD>
  
			</TR>
  
		  { { ScoreRows} } 
        </TABLE></font>

<FORM ACTION = ""{{BigAlURL}}"" METHOD=""POST"">
<INPUT TYPE = HIDDEN NAME=""action"">
          <!--### Buttons ###--> <a href=""javascript:do_action('REINCARNATE')""><img src=""{{ImagesURL}}/level_1/1_restartlevel.gif"" width=""118"" height=""12"" border=""0"" alt=""Restart Level""></a><a href=""javascript:do_action('BEGIN')""><img src=""{{ImagesURL}}/level_1/1_restartgame.gif"" width=""118"" height=""12"" border=""0"" alt=""Restart Game""></a><A onClick=""popmailwin('/cgi-bin/navigation/mailto.pl?GO=1','Mailer')"" HREF=""/cgi-bin/navigation/mailto.pl?GO=1"" TARGET=""Mailer""><img src=""/dinosaurs/bigalgame/images/sendtoafriend.gif"" width=""118"" height=""12"" alt=""Send it to a friend"" border=""0""></a><a href=""javascript:self.close()""><img src=""{{ImagesURL}}/level_1/1_quit.gif"" width=""118"" height=""12"" border=""0"" alt=""End the Game""></a> 
        </FORM>

</CENTER>

    </td>
	<td><img src = ""images/spacer.gif"" width=5 height=480 alt=""""></td>
    </tr>
</table>
</BODY>
</HTML>";
}