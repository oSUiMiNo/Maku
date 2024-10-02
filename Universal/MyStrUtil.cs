
public class MyStrUtil
{
    //// 右側切り抜き
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


    //// Stringのトリミング
    //// 右側切り抜き
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
    //// 右側切り抜き　左から何番目のスプリッターを使うか指定できる
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


    //// 右側切り落とし
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
    //// 右側切り落とし　左から何番目のスプリッターを使うか指定できる
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
