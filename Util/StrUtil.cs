using System.Text;
using System;


namespace Maku
{
    public static class StrUtil
    {
        //========================================
        // �����R�[�h�R���o�[�g
        // Base64url -> UTF8
        //========================================
        public static string Base64Url_To_UTF8(string base64Url)
        {
            int paddingNum = base64Url.Length % 4;
            if (paddingNum != 0) paddingNum = 4 - paddingNum;
            string utf8 = Encoding.UTF8.GetString(Convert.FromBase64String(
                                (base64Url + new string('=', paddingNum))    // �p�f�B���O��ǉ�
                                .Replace('-', '+')                           //�u-�v�ˁu+�v
                                .Replace('_', '/')));                        //�u_�v�ˁu/�v
            return utf8;
        }

        //========================================
        // �����R�[�h�R���o�[�g
        // UTF8 -> Base64url
        //========================================
        public static string UTF8_To_Base64Url(string utf8)
        {
            string base64Url = Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8))
                                    .TrimEnd('=')             // �p�f�B���O���폜
                                    .Replace('+', '-')        //�u+�v�ˁu-�v
                                    .Replace('/', '_');       //�u/�v�ˁu_�v
            return base64Url;
        }

        //========================================
        // �E���؂蔲��
        //========================================
        public static string CropStr_LastR(this string str, string splitter, bool containSplitter)
        {
            int splitterPos = str.LastIndexOf(splitter);
            if (splitterPos < 0) return str;

            int a = splitter.Length;
            //if (containSplitter) a = 0;
            //else a = splitter.Length;

            if (containSplitter) return $"{splitter}{str.Substring(splitterPos + a)}";
            return str.Substring(splitterPos + a);
        }

        //========================================
        // �E���؂蔲��
        //========================================
        public static string CropStr_R(this string str, string splitter, bool containSplitter = false)
        {
            int splitterPos = str.IndexOf(splitter);
            if (splitterPos < 0) return str;

            int a = splitter.Length;
            //if (containSplitter) a = 0;
            //else a = splitter.Length;

            if (containSplitter) return $"{splitter}{str.Substring(splitterPos + a)}";
            return str.Substring(splitterPos + a);
        }

        //========================================
        // �E���؂蔲���@�����牽�Ԗڂ̃X�v���b�^�[���g�����w��ł���
        //========================================
        public static string CropStr_R(this string str, string splitter, int splitterNum, bool containSplitter = false)
        {
            string result = str;
            for (int i = 0; i < splitterNum; i++)
            {
                result = CropStr_R(result, splitter, false);
            }
            if (containSplitter) return $"{splitter}{result}";
            return result;
        }

        //========================================
        // �E���؂藎�Ƃ�
        //========================================
        public static string TrimStr_R(this string str, string splitter, bool containSplitter = false)
        {
            //var i = str.IndexOf(splitter);
            var i = str.LastIndexOf(splitter);
            if (i < 0) return str;

            int a = 0;
            //if (containSplitter) a = splitter.Length;
            //else a = 0;

            if (containSplitter) return $"{str.Substring(0, i + a)}{splitter}";
            return str.Substring(0, i + a);
        }

        //========================================
        // �E���؂藎�Ƃ��@�����牽�Ԗڂ̃X�v���b�^�[���g�����w��ł���
        //========================================
        public static string TrimStr_R(this string str, string splitter, int splitterNum, bool containSplitter = false)
        {
            string result = str;
            for (int i = 0; i < splitterNum; i++)
            {
                result = TrimStr_R(result, splitter, false);
            }
            if (containSplitter) return $"{result}{splitter}";
            return result;
        }

        //========================================
        // ����
        //========================================
        public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
        public static string Large(this string str) => str.Size(16);
        public static string Medium(this string str) => str.Size(11);
        public static string Small(this string str) => str.Size(9);
        public static string Bold(this string str) => string.Format("<b>{0}</b>", str);
        public static string Italic(this string str) => string.Format("<i>{0}</i>", str);

        //========================================
        // �F
        //========================================
        public static string HexColor(this string str, string hexCode) => string.Format("<color={0}>{1}</color>", hexCode, str);
        public static string Red(this string str) => str.HexColor("red");
        public static string Green(this string str) => str.HexColor("green");
        public static string Blue(this string str) => str.HexColor("blue");
        public static string Black(this string str) => str.HexColor("black");
        public static string White(this string str) => str.HexColor("white");
        public static string Gray(this string str) => str.HexColor("#808080");
        public static string Yellow(this string str) => str.HexColor("yellow");
        public static string Magenta(this string str) => str.HexColor("#FF00FF");
        public static string Cyan(this string str) => str.HexColor("#00FFFF");
        public static string Orange(this string str) => str.HexColor("orange");
        public static string Purple(this string str) => str.HexColor("purple");
        public static string Brown(this string str) => str.HexColor("#A52A2A");
    }
}