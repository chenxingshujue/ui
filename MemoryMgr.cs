using UnityEngine;
using System.Collections;

namespace Tools
{
    public class MemoryMgr
    {
        static public void Recover()
        {
            UnloadUnusedAsset();
            GC();
        }

        static public void UnloadUnusedAsset()
        {
            Resources.UnloadUnusedAssets();
        }

        static public void GC()
        {
            //暂时把GC屏蔽掉,测清楚问题再开启回来
            //System.GC.Collect();
        }


    }

}
