
public class MyStrUtil
{
    //// �E���؂蔲��
    //public static string CropStr_LastR(string str, string splitter, bool containSplitter)
    //{
    //    int splitterPos = str.LastIndexOf(splitter);
    //    if (splitterPos < 0) return str;

    //    int a = splitter.Length;
    //    //if (containSplitter) a = 0;
    //    //else a = splitter.Length;

    //    if (containSplitter) return $"{splitter}{str.Substring(splitterPos + a)}";
    //    return str.Substring(splitterPos + a);
    //}


    //// String�̃g���~���O
    //// �E���؂蔲��
    //public static string CropStr_R(string str, string splitter, bool containSplitter = false)
    //{
    //    int splitterPos = str.IndexOf(splitter);
    //    if (splitterPos < 0) return str;

    //    int a = splitter.Length;
    //    //if (containSplitter) a = 0;
    //    //else a = splitter.Length;

    //    if (containSplitter) return $"{splitter}{str.Substring(splitterPos + a)}";
    //    return str.Substring(splitterPos + a);
    //}
    //// �E���؂蔲���@�����牽�Ԗڂ̃X�v���b�^�[���g�����w��ł���
    //public static string CropStr_R(string str, string splitter, int splitterNum, bool containSplitter = false)
    //{
    //    string result = str;
    //    for (int i = 0; i < splitterNum; i++)
    //    {
    //        result = CropStr_R(result, splitter, false);
    //    }
    //    if (containSplitter) return $"{splitter}{result}";
    //    return result;
    //}


    //// �E���؂藎�Ƃ�
    //public static string TrimStr_R(string str, string splitter, bool containSplitter = false)
    //{
    //    //var i = str.IndexOf(splitter);
    //    var i = str.LastIndexOf(splitter);
    //    if (i < 0) return str;

    //    int a = 0;
    //    //if (containSplitter) a = splitter.Length;
    //    //else a = 0;

    //    if (containSplitter) return $"{str.Substring(0, i + a)}{splitter}";
    //    return str.Substring(0, i + a);
    //}
    //// �E���؂藎�Ƃ��@�����牽�Ԗڂ̃X�v���b�^�[���g�����w��ł���
    //public static string TrimStr_R(string str, string splitter, int splitterNum, bool containSplitter = false)
    //{
    //    string result = str;
    //    for (int i = 0; i < splitterNum; i++)
    //    {
    //        result = TrimStr_R(result, splitter, false);
    //    }
    //    if (containSplitter) return $"{result}{splitter}";
    //    return result;
    //}
}
