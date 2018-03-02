
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public enum LanguageType
{
    GBA, UTF8, English, Char, Num
}

public class StringUtil
{

    /// <summary>
    /// 将指定长度的字母变为小写
    /// </summary>
    public static string ToLower(string str, int index, int count)
    {
        string s = str.Substring(index, count);
        str = str.Remove(index, count);
        return str.Insert(index, s.ToLower());
    }
    /// <summary>
    /// 将指定长度的字母变为大写
    /// </summary>
    public static string ToUpper(string str, int index, int count)
    {
        string s = str.Substring(index, count);
        str = str.Remove(index, count);
        return str.Insert(index, s.ToUpper());
    }
    /// <summary>
    /// 判断是否在两个数中间
    /// </summary>
    /// <param name="a">要判断的数</param>
    /// <param name="startb"></param>
    /// <param name="endc"></param>
    /// <returns></returns>
    public static bool IsIn(string a,string startb,string endc)
    {
        int aa = int.Parse(a);
        int b = int.Parse(startb);
        int c = int.Parse(endc);
        if (aa>b &&aa<c )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 判断是否在两个数中间
    /// </summary>
    /// <param name="a">要判断的数</param>
    /// <param name="startb"></param>
    /// <param name="endc"></param>
    /// <returns></returns>
    public static bool IsIn(string a, int startb, int endc)
    {
      //  Debug.Log(a);
        int aa = int.Parse(a);
        if (aa > startb && aa < endc)return true;
        return false;    
    }
    /// <summary>
    /// 判断是否在两个数中间
    /// </summary>
    /// <param name="a">要判断的数</param>
    /// <param name="startb"></param>
    /// <param name="endc"></param>
    /// <returns></returns>
    public static bool IsIn(int a, int startb, int endc)
    {
        if (a > startb && a < endc)return true;
        return false;   
    }
    /// <summary>
	/// 验证是IP
	/// </summary>
	public static bool ValidationIsIPAddress(string strIP)
    {
        string num = "25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d";
        return System.Text.RegularExpressions.Regex.IsMatch(strIP, ("^" + num + "\\." + num + "\\." + num + "\\." + num + "$"));
    }
    /// <summary>
	/// 验证网址
	/// </summary>
    public static bool ValidationIsUrl(string strurl)
    {
        string regex = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        return System.Text.RegularExpressions.Regex.IsMatch(strurl, regex);
    }
    /// <summary>
    /// 验证用户名，最小到最大位数start-end
    /// </summary>
    /// <param name="strAccount"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string ValidationIsUserAccount(string strAccount,int start,int end)
    {
        if (!IsIn(strAccount.Length,start,end ))
        {
            return "账号长度不符合要求";
        }
        else
        {
            string regex = "^[0-9\\.@]";
            if (System.Text.RegularExpressions.Regex.IsMatch(strAccount, regex))
            {
                return null;
            }
            else
            {
                return "账号格式不正确";
            }
        }
        // string regex = "^[a-zA-Z0-9\\.@]{6,16}$";
     
      
    }
    /// <summary>
    /// 验证密码,最小到最大位数index-count
    /// </summary>
    /// <param name="strpwd"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string ValidationPWDString(string strpwd, int start, int end)
    {
        if (!IsIn(strpwd.Length , start, end))
        {
            return "密码长度不符合要求";
        }
        else
        {
            string regex = "^[a-zA-Z0-9\\.@]";
            if (System.Text.RegularExpressions.Regex.IsMatch(strpwd, regex))
            {
                return null;
            }
            else
            {
                return "密码格式不正确";
            }
        }
       // string regex = @"^[a-zA-Z0-9]{6,16}$";
    }
    /// <summary>
    /// 验证是否是邮箱
    /// </summary>
    public static bool ValidationIsEmail(string strEmail)
    {
        string regex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        return System.Text.RegularExpressions.Regex.IsMatch(strEmail, regex);
    }
    /// <summary>
    /// 替换指定字符串
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <param name="index"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public static string StrReplace(string oldValue, string newValue, int index, int endIndex)
    {
        if (endIndex > oldValue.Length) endIndex = oldValue.Length;
        string s = oldValue.Substring(index, endIndex);
        return oldValue.Replace(s, newValue);
    }
    /// <summary>
    /// 删除字符串
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string StrRemove(string oldValue, int index, int endIndex)
    {
        if (endIndex > oldValue.Length) endIndex = oldValue.Length;
        string s = oldValue.Remove(index, endIndex);
        return s;
    }
    /// <summary>
    /// 字符串拼接
    /// </summary>
    /// <param name="OldValue"></param>
    /// <param name="newValue"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string StrConcat(string OldValue, string newValue, string symbol = "")
    {
        string newStr = string.Concat(OldValue, symbol, newValue);
        return newStr;
    }
    /// <summary>
    /// 判断字串是否为空,空返回true
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty(string value)
    {
        string trim = Regex.Replace(value, @"\s", "");
        if (trim.Length > 0)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 判断是否是数字
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumeric(string value)
    {
        if (value == null || value.Length == 0)
            return false;
        bool ret = Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        if (!ret)
        {
          //  Debug.Log(value + "IsNumber:" + ret);
        }
        return ret;
    }
    /// <summary>
    /// 判断是否都是整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsInt(string value)
    {
        if (value == null || value.Length == 0)
            return false;

        bool ret = Regex.IsMatch(value, @"^[+-]?\d*$");

        if (!ret)
        {
          //  Debug.Log(value + "IsInt:" + ret);
        }
        return ret;
    }
    /// <summary>
    /// 判断是都是符号
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsUnsign(string value)
    {
        if (value == null || value.Length == 0)
            return false;
        return Regex.IsMatch(value, @"^\d*[.]?\d*$");
    }
    public static string[] Params2Array(string param)
    {
        string[] arr = new string[] { };

        if (param == null || param == string.Empty)
            return arr;

        string str = param.Trim();
        if (str.Length < 3) return arr;
        if (str.Substring(str.Length - 1, 1) == ";")
        {
            str = str.Substring(0, str.Length - 1);
        }

        arr = str.Split(new char[] { '=', ';' });
        if (arr.Length % 2 == 1)
        {
            arr = new string[] { };
        }
        return arr;
    }
    /// <summary>
    /// 字符截取
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="value">Value.</param>
    /// <param name="s">S.</param>
    /// <param name="e">E.</param>
    public static string SubString(string value, int maxNum, int index = 0)
    {
        if (maxNum>=value.Length) maxNum = value.Length;
             value = value.Substring(index, maxNum);
        return value;
    }
    /// <summary>
    /// 获取字符串去前后空格后的长度
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetStrLength(string value)
    {
        string trim = Regex.Replace(value, @"\s", "");
        return (trim.Trim()).Length;
    }
    /// <summary>
    /// 分隔字符串
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="str">String.</param>
    /// <param name="c">C.</param>
    public static List<string> SplitString(string str, char c)
    {
        List<string> temp = new List<string>();
        string[] strArrTmp;
        if (str.Contains(c.ToString()))
        {
            strArrTmp = str.Split(c);
            foreach (string element in strArrTmp)
            {
                temp.Add(element);
            }
        }
        else {
            if (!str.Equals(""))
            {
                temp.Add(str);
            }
        }
        return temp;
    }
    /// <summary>
    /// 获取key 和value 对应字符串
    /// </summary>
    /// <param name="keyValue"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="split"></param>
    /// <returns></returns>
    public static bool KeyValue(string keyValue, out string key, out string value, char split = '|')
    {
        bool ret = false;
        int mark = 0;
        for (int i = 0; i < keyValue.Length; ++i)
        {
            if (keyValue[i] == split)
            {
                mark = i;
                ret = true;
                break;
            }
        }
        key = keyValue.Substring(0, mark);
        value = keyValue.Substring(mark + 1, keyValue.Length - mark - 1);
        return ret;
    }
    public static string PathToString(List<Vector3> path)
    {
        StringBuilder stringBuild = new StringBuilder();

        for (int i = 0; i < path.Count; ++i)
        {
            stringBuild.Append(path[i].x); stringBuild.Append(',');
            stringBuild.Append(path[i].y); stringBuild.Append(',');
            stringBuild.Append(path[i].z); stringBuild.Append(',');
        }

        return stringBuild.ToString();
    }
    /// <summary>
    /// 是否是标示符
    /// </summary>
    /// <returns></returns>
    static public bool IsLegal(string value)
    {
        if (value == null || value.Length == 0)
            return false;
        bool ret = true;
        for (int i = 0; i < value.Length; ++i)
        {
            if (!(char.IsLetterOrDigit(value[i]) || value[i] == '_'))
                return false;
        }
        return ret;
    }

    /// <summary>
    /// 计算字符串的长度,包括字母数字汉字等
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetStringLength(string str, LanguageType type)
    {
        if (str.Length == 0) return 0;

        ASCIIEncoding ascii = new ASCIIEncoding();
        int tempLen = 0;
        byte[] s = ascii.GetBytes(str);
        for (int i = 0; i < s.Length; i++)
        {
            if ((int)s[i] == 63)
            {
                if (type == LanguageType.GBA)
                {
                    tempLen += 2;
                }
                else if (type == LanguageType.UTF8)
                {
                    tempLen += 3;
                }

            }
            else
            {
                tempLen += 1;
            }
        }

        return tempLen;
    }
    /// <summary>
    /// 计算字符串的长度,包括字母数字汉字等
    /// </summary>
    public static int GetStringLength(string str)
    {
        return Encoding.Default.GetByteCount(str);
    }
}
