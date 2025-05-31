using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine;

public class HashUtil
{
    //==================================================
    // 
    //==================================================
    public static string Generate()
    {
        // ƒ‰ƒ“ƒ_ƒ€‚È•¶š—ñ‚ğ¶¬
        string input = Guid.NewGuid().ToString();

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
            return hashString;
        }
    }
}
