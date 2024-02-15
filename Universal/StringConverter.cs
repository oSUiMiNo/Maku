using System.Text;
using System;

public static class StringConverter
{
    // Base64url => UTF8
    public static string Base64Url_To_UTF8(string base64Url)
    {
        int paddingNum = base64Url.Length % 4;
        if (paddingNum != 0) paddingNum = 4 - paddingNum;
        string utf8 = Encoding.UTF8.GetString(Convert.FromBase64String(
                            (base64Url + new string('=', paddingNum))    // パディングを追加
                            .Replace('-', '+')                              //「-」⇒「+」
                            .Replace('_', '/')));                           //「_」⇒「/」
        return utf8;
    }


    // UTF8 => Base64url
    public static string UTF8_To_Base64Url(string utf8)
    {
        string base64Url = Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8))
                                .TrimEnd('=')             // パディングを削除
                                .Replace('+', '-')        //「+」⇒「-」
                                .Replace('/', '_');       //「/」⇒「_」
        return base64Url;
    }

}
