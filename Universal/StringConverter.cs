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
                            (base64Url + new string('=', paddingNum))    // �p�f�B���O��ǉ�
                            .Replace('-', '+')                              //�u-�v�ˁu+�v
                            .Replace('_', '/')));                           //�u_�v�ˁu/�v
        return utf8;
    }


    // UTF8 => Base64url
    public static string UTF8_To_Base64Url(string utf8)
    {
        string base64Url = Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8))
                                .TrimEnd('=')             // �p�f�B���O���폜
                                .Replace('+', '-')        //�u+�v�ˁu-�v
                                .Replace('/', '_');       //�u/�v�ˁu_�v
        return base64Url;
    }

}
