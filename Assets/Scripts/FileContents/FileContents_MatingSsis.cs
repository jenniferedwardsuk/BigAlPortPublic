internal static partial class FileContents
{
	internal static string matingSsi = @"
<SCRIPT LANGUAGE=""JAVASCRIPT"">
<!--
function decision(val)
	{
		document.forms[0].decision.value = val;
		document.forms[0].action.value = 'Mate:Female Allosaurus';
		document.forms[0].submit();
		return true;
	}
//-->
</SCRIPT>
<INPUT TYPE = HIDDEN NAME=decision>

";

	internal static string matingCallsSsi = @"<!-- Mating Calls -->

<FONT FACE=""Arial, Helvetica, Helv"" SIZE=""1"">
<UL style = ""font-size: 8pt; text-indent: -10pt; margin-top: 0; margin-bottom: 0;"" >
<LI><A HREF=""javascript:decision('Rumble')"">
 make deep rumbling mating call
</A></LI>

<LI><A HREF = ""javascript:decision('Squeak');"" >
 make soft squeaking noise
</A></LI>


<LI><A HREF = ""javascript:decision('Wait');"" >
 wait for her to lift her tail
</A></LI>

<LI><A HREF = ""javascript:decision('Mount');"" >
 attempt to mount her
</A></LI>
</UL>
</FONT>
";

	internal static string matingMovesSsi = @"<!-- Mating moves -->

<FONT FACE=""Arial, Helvetica, Helv"" SIZE=""1"">
<UL style = ""font-size: 8pt; text-indent: -10pt; margin-top: 0; margin-bottom: 0;"" >
<LI><A HREF=""javascript:decision('Display')"">
 move in front and show off eye crests
</A></LI>

<LI><A HREF = ""javascript:decision('Circle')"" >
 move around her in circles
</A></LI>

<LI><A HREF = ""javascript:decision('Side')"" >
 move to her side and show off eye crests 
</A></LI>

<LI><A HREF = ""javascript:decision('Wait')"" >
 wait for her to make the next move
</A></LI>

<LI><A HREF = ""javascript:decision('Mount')"" >
 attempt to mount her
</A></LI>
</UL>
</FONT>

";
}