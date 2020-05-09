using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EditorTools
{
    public class StrTools
    {
        /// <summary>
        /// 获取第一个匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public string GetFirstMatch(string str, string regexStr)
        {
            Match m = Regex.Match(str, regexStr);
            if (m != null)
            {
                return m.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有匹配,返回string[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public string[] GetAllMatchs(string str, string regexStr)
        {
            MatchCollection mc = Regex.Matches(str, regexStr);
            if (mc.Count == 0)
            {
                return null;
            }
            string[] matchs = new string[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                matchs[i] = mc[i].Value.ToString();
            }
            return matchs;
        }


        /// <summary>
        /// 格式化字符串，替代{数字}内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public string Format(string str, params string[] args)
        {
            string newStr = str;
            string rexStr = @"\{[0-9]+\}";
            MatchCollection mc = Regex.Matches(str, rexStr);
            if (mc.Count > 0)
            {
                for (int i = 0; i < mc.Count; i++)
                {
                    if (i < args.Length)
                    {
                        string subStr = "{" + i + "}";//mc[i].Value.ToString();
                        newStr = newStr.Replace(subStr, args[i]);
                    }

                }


            }
            return newStr;
        }
        /// <summary>
        /// 切割字符串，返回string[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        static public string[] Split(string str, string splitStr)
        {
            string[] newStrs = Regex.Split(str, splitStr);
            return newStrs;
        }
        /// <summary>
        /// 切割字符串，返回List<string>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        static public List<string> Split2(string str, string splitStr)
        {
            string[] newStrs = Regex.Split(str, splitStr);
            List<string> strs = new List<string>();

            for (int i = 0; i < newStrs.Length; i++)
            {
                strs.Add(newStrs[i]);
            }
            return strs;
        }

        /// <summary>
        /// 去掉换行符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string RemoveReturn(string str)
        {
            return StringTools.Replace(str, "\r\n|\r|\n", "");
        }

        /// <summary>
        /// 用正则表达式替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        static public string Replace(string str, string str1, string str2)
        {

            Regex rgx = new Regex(str1);
            string result = rgx.Replace(str, str2);
            return result;
        }



        /// <summary>
        /// 检查版本是否需要更新
        /// </summary>
        /// <param name="wwwVer"></param>
        /// <param name="localVer"></param>
        /// <returns></returns>
        static public bool isNeedUpdata(string wwwVer, string localVer)
        {
            if (localVer == "")
            {
                return true;
            }
            string[] wwwVers = wwwVer.Split('.');
            string[] localVers = localVer.Split('.');
            int len = wwwVers.Length > localVers.Length ? wwwVers.Length : localVers.Length;
            bool needUpdata = false;
            int wwwValue;
            int localValue;
            for (int i = 0; i < len; i++)
            {
                wwwValue = GetValue(wwwVers, i);
                localValue = GetValue(localVers, i);
                if (wwwValue > localValue)
                {
                    needUpdata = true;
                    break;
                }
                if (localValue > wwwValue)
                {
                    break;
                }
            }
            return needUpdata;
        }

        static private int GetValue(string[] strs, int ind)
        {
            if (strs.Length > ind)
            {
                int val = System.Int32.Parse(strs[ind]);
                return val;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取传入字符的类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string GetType(string value)
        {
            string type = "";
            if (IsNumber(value))
            {
                if (value.IndexOf(".") < 0)
                {
                    type = "int";
                }
                else
                {
                    type = "float";
                }
            }
            else if (IsBool(value))
            {
                type = "bool";
            }
            else
            {
                type = "string";
            }
            return type;
        }

        static private string getIsNumber = @"^\-?[0-9]+(\.[0-9]*)?$";
        /// <summary>
        /// 检查是否为数字
        /// </summary>
        static public bool IsNumber(string expression)
        {
            MatchCollection mc = Regex.Matches(expression, getIsNumber);
            if (mc.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 检查是否为布尔
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public bool IsBool(string str)
        {
            string st = str.ToLower();
            if (st == "true" || st == "false")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

