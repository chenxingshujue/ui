using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LuaInt64比较辅助方法
/// </summary>
public static class LuaInt64Tool
{

    #region 比较
    public static int Compare(string lsh, string rsh)
    {
        return Int64.Parse(lsh).CompareTo(Int64.Parse(rsh));
    }

    public static int Compare(int lsh, string rsh)
    {
        return lsh.CompareTo(Int64.Parse(rsh));
    }

    public static int Compare(string lsh, int rsh)
    {
        return Int64.Parse(lsh).CompareTo(rsh);
    }
    #endregion
    #region 比较 过时但不可删除
    //小于
    public static bool CompareLess(string lsh, string rsh)
    {
        return Int64.Parse(lsh) < Int64.Parse(rsh);
    }

    //小于
    public static bool CompareLess(string lsh, int rsh)
    {
        return Int64.Parse(lsh) < rsh;
    }
    //小于等于
    public static bool CompareLessEqu(string lsh, string rsh)
    {
        return Int64.Parse(lsh) <= Int64.Parse(rsh);
    }

    //小于等于
    public static bool CompareLessEqu(string lsh, int rsh)
    {
        return Int64.Parse(lsh) <= rsh;
    }
    //等于
    public static bool CompareEqu(string lsh, string rsh)
    {
        return Int64.Parse(lsh) == Int64.Parse(rsh);
    }

    //等于
    public static bool CompareEqu(string lsh, int rsh)
    {
        return Int64.Parse(lsh) == rsh;
    }

    //大于
    public static bool CompareMore(string lsh, string rsh)
    {
        return Int64.Parse(lsh) > Int64.Parse(rsh);
    }

    //大于
    public static bool CompareMore(string lsh, int rsh)
    {
        return Int64.Parse(lsh) > rsh;
    }
    //大于等于
    public static bool CompareMoreEqu(string lsh, string rsh)
    {
        return Int64.Parse(lsh) >= Int64.Parse(rsh);
    }

    //大于等于
    public static bool CompareMoreEqu(string lsh, int rsh)
    {
        return Int64.Parse(lsh) >= rsh;
    }
    #endregion

    #region 运算
    //加
    public static string Add(string lsh, int rsh)
    {
        Int64 result = Int64.Parse(lsh) + rsh;
        return result.ToString();
    }
    //加
    public static string Add(string lsh, string rsh)
    {
        Int64 result = Int64.Parse(lsh) + Int64.Parse(rsh);
        return result.ToString();
    }

    //减
    public static string Sub(string lsh, int rsh)
    {
        Int64 result = Int64.Parse(lsh) - rsh;
        return result.ToString();
    }

    //减
    public static string Sub(string lsh, string rsh)
    {
        Int64 result = Int64.Parse(lsh) - Int64.Parse(rsh);
        return result.ToString();
    }

    //乘
    public static string Multi(string lsh, int rsh)
    {
        Int64 result = Int64.Parse(lsh) * rsh;
        return result.ToString();
    }

    //乘
    public static string Multi(string lsh, string rsh)
    {
        Int64 result = Int64.Parse(lsh)* Int64.Parse(rsh);
        return result.ToString();
    }

    //除
    public static string Div(string lsh, int rsh)
    {
        double result = (double)Int64.Parse(lsh) / rsh;
        return result.ToString();
    }

    //除
    public static string Div(string lsh, string rsh)
    {
        double result = (double)Int64.Parse(lsh) / Int64.Parse(rsh);
        return result.ToString();
    }

    //取余
    public static string Mod(string lsh, int rsh)
    {
        Int64 result = Int64.Parse(lsh) % rsh;
        return result.ToString();
    }

    //取余
    public static string Mod(string lsh, string rsh)
    {
        Int64 result = Int64.Parse(lsh) % Int64.Parse(rsh);
        return result.ToString();
    }
    #endregion

}
