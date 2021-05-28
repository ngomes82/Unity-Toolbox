using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtils
{
	/// <summary>
	/// Outputs the MD5 of a given string in hexadecimal string format.
	/// </summary>
	/// <param name="inputStr"></param>
	/// <returns></returns>
	public static string CalculateMD5Hash(string inputStr)
	{
		byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
		return CalculateMD5Hash(inputBytes);
	}

	/// <summary>
	/// Outputs the MD5 of the given bytes in hexadecimal string format.
	/// </summary>
	/// <param name="inputStr"></param>
	/// <returns></returns>
	public static string CalculateMD5Hash(byte[] bytes)
	{
		MD5 md5		= MD5.Create();
		byte[] hash = md5.ComputeHash(bytes);

		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			sb.Append(hash[i].ToString("X2"));
		}

		return sb.ToString();
	}
}
