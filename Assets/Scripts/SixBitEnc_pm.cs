using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PerlInterface;

public class SixBitEnc_pm : MonoBehaviour
{
	//#########################################################################################  
	//# 
	//# SixBitEnc.pm  
	//# 
	//# 6 bit encoding - provide delimiter-safe encoding of binary data. 
	//# Values in the range 0-63 are converted to 0-9, A-Z, a-Z, ., * 
	//#  
	//# Author: John Alden  
	//#  
	//#########################################################################################  

	//	package SixBitEnc;

	public static string encode_binary_string(string binary_string)
	{
		string retstring = "";

		int val = 0;
		for (int ctr = 0; ctr < binary_string.Length; ctr++) 
		 {
			int bit = ctr % 6; //todo: check % operator
			val += binary_string.Substring(ctr, 1).ToCharArray()[0] << (5 - bit);
			if (bit == 5 || ctr == binary_string.Length - 1) 
		    {
				retstring += six_bit_enc(val); 
				val = 0;
			}
		}

		return retstring;
	}

	public static string decode_binary_string(string str)
	{
		string binary_string = "";

		for (int ctr = 0; ctr < str.Length; ctr++) 
		{
			char charVal = substr(str, ctr, 1).ToCharArray()[0];
			int val = six_bit_dec(charVal);

			//# convert to binary 
			string binary = ""; 
			binary += (val & 32) != 0 ? 1 : 0; 
			binary += (val & 16) != 0 ? 1 : 0; 
			binary += (val & 8) != 0 ? 1 : 0; 
			binary += (val & 4) != 0 ? 1 : 0; 
			binary += (val & 2) != 0 ? 1 : 0; 
			binary += (val & 1) != 0 ? 1 : 0;

			binary_string += binary;
		}

		return binary_string;
	}

	static int six_bit_dec(char charVal)//$)
	{
		//# Convert each character to a number 0-63 
		int val = -1;

		if (charVal == '*')
		{
			val = 63;
		}
		else if (charVal == '.')
		{
			val = 62;
		}
		else if (perlRegex(charVal.ToString(),"[a-z]")) //#36-61
		{
			val = ord(charVal) - ord('a') + 36;
		}
		else if (perlRegex(charVal.ToString(), "[A-Z]")) //#10-35 
		{ 
			val = ord(charVal) - 55;
		} 
		else //#0-9 
		{
			val = charVal;
		}

		return val;
	}

	static char six_bit_enc(int val)
	{
		//# Convert a number in the range 0-63 into a character 
		char charVal;

		if (val == 63)
		{
			charVal = '*';
		}
		else if(val == 62)
		{
			charVal = '.';
		}
		else if (val >= 36 && val <= 61)
		{
			charVal = chr(ord('a') + val - 36);
		}
		else if (val >= 10 && val <= 35)
		{
			charVal = chr(ord('A') + val - 10);
		}
		else 
		{
			charVal = val.ToString().ToCharArray()[0];
		}

		return charVal; //added
	}
}
