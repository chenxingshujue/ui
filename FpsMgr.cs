using UnityEngine;
using System.Collections;

public class FpsMgr {
    private static FpsMgr _instance;

    public static FpsMgr Instance
    {
        get { 
            if(_instance == null)
            {
                _instance = new FpsMgr();
            }
            return FpsMgr._instance;
        }
    }

    public void ShowOrHideFps()
    {
        ShowFPS com = GetFpsComponent();
        com.enabled = !com.enabled;
    }

    public void ShowFps(bool isShow = true)
    {
        ShowFPS com = GetFpsComponent();
        com.enabled = isShow;
    }


    private ShowFPS GetFpsComponent()
    {
        GameObject go = GameObject.Find("FPS");
        if (go == null)
        {
            go = new GameObject("FPS");
            Object.DontDestroyOnLoad(go);
        }
        ShowFPS com = go.GetComponent<ShowFPS>();
        if (com == null)
        {
            com = go.AddComponent<ShowFPS>();
        }
        return com;
    }
}
